// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\core\services\api\charge-api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  CheckActiveChargeRequestDto,
  CheckActiveChargeResponseDto,
  ChargeStatusDto,
  StopChargeRequestDto,
  StopChargeResponseDto,
} from 'src/app/core/models/charge';
import { ChargeStatusType } from 'src/app/core/enums';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

// ============================================================
// ChargeApiService
// Sarj ile ilgili backend API istekleri
// ============================================================
@Injectable({ providedIn: 'root' })
export class ChargeApiService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = (environment.stationApiUrl || environment.apiUrl || '') + 'Charge/';
  }

  // TODO: Backend endpoint hazir olunca guncelle
  // POST /Charge/checkActiveCharge
  checkActiveCharge(dto: CheckActiveChargeRequestDto): Observable<CheckActiveChargeResponseDto> {
    // return this.http.post<CheckActiveChargeResponseDto>(this.srvUrl + 'checkActiveCharge', dto, httpOptions);
    console.log('[MOCK] checkActiveCharge:', dto);
    const mockStatus: ChargeStatusDto = {
      sessionId: 'session-mock-123',
      socketName: 'R0026-1',
      status: ChargeStatusType.Charging,
      chargePercent: 67,
      chargePower: 19.2,
      chargeEnergy: 6.4,
      startTime: '2025-06-23 13:59',
      elapsedMinutes: 20,
      totalPrice: 44.8,
    };
    return of({ hasActiveCharge: true, chargeStatus: mockStatus }).pipe(delay(1000));
  }

  // TODO: Backend endpoint hazir olunca guncelle
  // POST /Charge/stopCharge
  stopCharge(dto: StopChargeRequestDto): Observable<StopChargeResponseDto> {
    // return this.http.post<StopChargeResponseDto>(this.srvUrl + 'stopCharge', dto, httpOptions);
    console.log('[MOCK] stopCharge:', dto);
    const finalStatus: ChargeStatusDto = {
      sessionId: dto.sessionId,
      socketName: 'R0026-1',
      status: ChargeStatusType.Completed,
      chargePercent: 67, chargePower: 0, chargeEnergy: 6.4,
      startTime: '2025-06-23 13:59', elapsedMinutes: 20, totalPrice: 44.8,
    };
    return of({ success: true, finalStatus }).pipe(delay(1500));
  }

  // TODO: SignalR ile degistirilecek
  getChargeStatus(sessionId: string): Observable<ChargeStatusDto> {
    console.log('[MOCK] getChargeStatus:', sessionId);
    const mock: ChargeStatusDto = {
      sessionId, socketName: 'R0026-1', status: ChargeStatusType.Charging,
      chargePercent: 67, chargePower: 19.2, chargeEnergy: 6.4,
      startTime: '2025-06-23 13:59', elapsedMinutes: 20, totalPrice: 44.8,
    };
    return of(mock).pipe(delay(500));
  }
}
