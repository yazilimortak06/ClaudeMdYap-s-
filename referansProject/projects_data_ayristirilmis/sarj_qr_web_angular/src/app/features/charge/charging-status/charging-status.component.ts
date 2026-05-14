// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\features\charge\charging-status\charging-status.component.ts
import { Component, OnInit, OnDestroy, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SessionApiService } from 'src/app/core/services/api/session-api.service';
import { QrWebSecurityService } from 'src/app/core/services/domain/qr-web-security.service';
import { ChargeApiService } from 'src/app/core/services/api/charge-api.service';
import { ChargeSessionService } from 'src/app/core/services/domain/charge-session.service';
import { ChargeProcessingResponseDto } from 'src/app/core/models/session';
import { ChargeStatusDto } from 'src/app/core/models/charge';
import { ChargeStatusType, ChargeAmountType, ChargeState } from 'src/app/core/enums';

// ============================================================
// ChargingStatusComponent
// "Sarj Oluyor" sayfasi - aktif sarj durumunu gosterir
// Lottie animasyon ile dairesel progress
// SignalR ile gercek zamanli guncellenecek (su an mock)
// ============================================================
@Component({
  selector: 'app-charging-status',
  templateUrl: './charging-status.component.html',
  styleUrls: ['./charging-status.component.scss'],
})
export class ChargingStatusComponent implements OnInit, OnDestroy, AfterViewInit {
  // Enum referansi template icin
  ChargeAmountType = ChargeAmountType;

  // Lottie animasyon container referansi
  @ViewChild('lottieContainer', { static: false }) lottieContainer!: ElementRef;
  private lottieAnim: any;

  // Sarj durumu verileri (mevcut template ile uyumlu)
  chargeStatus: ChargeStatusDto | null = null;

  // ChargeProcessing response'tan gelen ek alanlar
  tempPaidAmount: number | null = null;
  estimatedRefundAmount: number | null = null;
  chargeState: ChargeState | null = null;

  // ============================================================
  // Hedef bilgisi - kullanicinin odeme sayfasinda sectigi miktar
  // ============================================================
  targetType: ChargeAmountType = ChargeAmountType.Kwh;
  targetAmount: number = 10; // mock: 10 kWh secilmis

  // Hedefe gore ilerleme yuzdesi
  get targetProgress(): number {
    if (!this.targetAmount || !this.chargeStatus) return 0;
    const current = this.currentTargetValue;
    return Math.min(100, (current / this.targetAmount) * 100);
  }

  // Su anki deger (hedefe gore kWh veya TL)
  get currentTargetValue(): number {
    if (!this.chargeStatus) return 0;
    if (this.targetType === ChargeAmountType.Kwh) {
      return this.chargeStatus.chargeEnergy || 0;
    }
    return this.chargeStatus.totalPrice || 0;
  }

  // Kalan miktar
  get remainingTargetValue(): number {
    const remaining = this.targetAmount - this.currentTargetValue;
    return Math.max(0, +remaining.toFixed(1));
  }

  // Tahmini kalan sure (dakika)
  get estimatedRemainingMinutes(): number {
    if (!this.chargeStatus?.chargePower || !this.chargeStatus?.chargeEnergy || !this.chargeStatus?.elapsedMinutes) return 0;
    const remainingKwh = this.targetType === ChargeAmountType.Kwh
      ? this.remainingTargetValue
      : this.remainingTargetValue / (this.chargeStatus.totalPrice && this.chargeStatus.chargeEnergy
          ? (this.chargeStatus.totalPrice / this.chargeStatus.chargeEnergy)
          : 7.79);
    const minutes = (remainingKwh / this.chargeStatus.chargePower) * 60;
    return Math.max(0, Math.round(minutes));
  }

  // Sayfa durumu
  loading = true;
  stopping = false;
  errorMessage = '';

  // Mock: periyodik guncelleme icin interval
  private mockInterval: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private sessionApiService: SessionApiService,
    private qrWebSecurityService: QrWebSecurityService,
    private chargeApiService: ChargeApiService,
    private chargeSessionService: ChargeSessionService
  ) {}

  ngOnInit(): void {
    // ChargeProcessing API cagir
    this.loadChargeProcessing();
  }

  ngAfterViewInit(): void {
    // Lottie animasyonu baslatmak icin
    // chargeStatus yuklendikten sonra initLottie cagirilacak
  }

  // ============================================================
  // Lottie animasyonu baslatir (lottie-web direkt kullanim)
  // ============================================================
  private initLottie(): void {
    if (!this.lottieContainer?.nativeElement) return;
    import('lottie-web').then((lottie: any) => {
      const lottieLib = lottie.default || lottie;
      this.lottieAnim = lottieLib.loadAnimation({
        container: this.lottieContainer.nativeElement,
        renderer: 'svg',
        loop: true,
        autoplay: true,
        path: 'assets/animations/chargeanimation.json',
      });
    });
  }

  ngOnDestroy(): void {
    if (this.lottieAnim) {
      this.lottieAnim.destroy();
    }
    if (this.mockInterval) {
      clearInterval(this.mockInterval);
    }
  }

  // ============================================================
  // ChargeProcessing API'den sarj durumunu al
  // ============================================================
  private loadChargeProcessing(): void {
    this.sessionApiService.chargeProcessing({
      qrWebUserId: this.qrWebSecurityService.getQrWebUserId(),
    }).subscribe({
      next: (res: ChargeProcessingResponseDto) => {
        // Response'u mevcut chargeStatus yapisiyla eslestir
        this.chargeStatus = {
          sessionId: String(res.chargeId),
          status: this.mapChargeStateToStatusType(res.chargeState),
          chargePercent: res.chargePercentage || 0,
          chargeEnergy: res.loadedKw || 0,
          totalPrice: res.calculatedPrice || 0,
          chargePower: 0, // ChargeProcessing response'ta yok, mock'tan gelecek
          startTime: res.startTime || '',
          elapsedMinutes: 0, // Hesaplanabilir
        };

        // Ek alanlar
        this.tempPaidAmount = res.tempPaidAmount || null;
        this.estimatedRefundAmount = res.estimatedRefundAmount || null;
        this.chargeState = res.chargeState;

        this.loading = false;

        // Lottie animasyonu baslat
        setTimeout(() => this.initLottie(), 100);

        // Mock: periyodik guncelleme baslat
        this.startMockUpdates();
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Sarj durumu alinamadi.';
      },
    });
  }

  // ChargeState enum'unu ChargeStatusType'a donustur
  private mapChargeStateToStatusType(chargeState: ChargeState): ChargeStatusType {
    switch (chargeState) {
      case ChargeState.ProcessStarting:
        return ChargeStatusType.Starting;
      case ChargeState.ProcessStart:
        return ChargeStatusType.Charging;
      case ChargeState.ProcessEnding:
        return ChargeStatusType.Stopping;
      case ChargeState.Completed:
        return ChargeStatusType.Completed;
      case ChargeState.Failed:
      case ChargeState.PaymentFail:
        return ChargeStatusType.Error;
      default:
        return ChargeStatusType.Charging;
    }
  }

  // ============================================================
  // Mock: Periyodik guncelleme (SignalR yerine gecici)
  // ============================================================
  private startMockUpdates(): void {
    this.mockInterval = setInterval(() => {
      if (this.chargeStatus && this.chargeStatus.status === ChargeStatusType.Charging) {
        this.chargeStatus = {
          ...this.chargeStatus,
          chargePercent: Math.min(100, (this.chargeStatus.chargePercent || 0) + 1),
          chargeEnergy: +(((this.chargeStatus.chargeEnergy || 0) + 0.1).toFixed(1)),
          elapsedMinutes: (this.chargeStatus.elapsedMinutes || 0) + 1,
          totalPrice: +(((this.chargeStatus.totalPrice || 0) + 0.78).toFixed(1)),
          chargePower: +(18 + Math.random() * 4).toFixed(1),
        };
      }
    }, 3000);
  }

  // ============================================================
  // Sarj durdur
  // ============================================================
  stopCharge(): void {
    if (!this.chargeStatus?.sessionId) return;

    this.stopping = true;

    this.chargeApiService.stopCharge({
      chargeSessionToken: this.chargeSessionService.getToken() || '',
      sessionId: this.chargeStatus.sessionId,
    }).subscribe({
      next: (res) => {
        this.stopping = false;
        if (res.success && res.finalStatus) {
          this.chargeStatus = res.finalStatus;
          if (this.mockInterval) {
            clearInterval(this.mockInterval);
          }
          this.chargeSessionService.clearToken();
        }
      },
      error: () => {
        this.stopping = false;
        this.errorMessage = 'Sarj durdurulamadi. Tekrar deneyiniz.';
      },
    });
  }

  // ============================================================
  // Yardimci metodlar
  // ============================================================

  getStatusText(): string {
    switch (this.chargeStatus?.status) {
      case ChargeStatusType.Starting: return 'Başlatılıyor';
      case ChargeStatusType.Charging: return 'Şarj Oluyor';
      case ChargeStatusType.Stopping: return 'Durduruluyor';
      case ChargeStatusType.Completed: return 'Tamamlandı';
      case ChargeStatusType.Error: return 'Hata';
      default: return '';
    }
  }

  isCharging(): boolean {
    return this.chargeStatus?.status === ChargeStatusType.Charging;
  }

  goBack(): void {
    window.history.back();
  }
}
