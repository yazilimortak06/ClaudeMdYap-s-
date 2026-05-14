// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\core\services\api\auth-api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  PrepareLoginFormRequestDto, PrepareLoginFormResponseDto,
  LoginRequestDto, LoginResponseDto,
  ValidateSmsCodeRequestDto, ValidateSmsCodeResponseDto,
} from 'src/app/core/models/auth';
import {
  StartChargeFromDeviceRequestDto, StartChargeFromDeviceResponseDto,
} from 'src/app/core/models/charge';
import { SocketStatusType } from 'src/app/core/enums';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

// ============================================================
// AuthApiService
// Authentication ile ilgili backend API istekleri
// ============================================================
@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = (environment.apiUrl || '');
  }

  // TODO: Backend endpoint hazir olunca guncelle
  prepareLoginForm(dto: PrepareLoginFormRequestDto): Observable<PrepareLoginFormResponseDto> {
    // return this.http.post<PrepareLoginFormResponseDto>(this.srvUrl + 'prepareLoginForm', dto, httpOptions);
    console.log('[MOCK] prepareLoginForm:', dto);
    return of({ loginFormKey: 'lfk-' + Date.now() }).pipe(delay(500));
  }

  // TODO: Backend endpoint hazir olunca guncelle
  login(dto: LoginRequestDto): Observable<LoginResponseDto> {
    // return this.http.post<LoginResponseDto>(this.srvUrl + 'login', dto, httpOptions);
    console.log('[MOCK] login:', dto);
    return of({}).pipe(delay(1000));
  }

  // TODO: Backend endpoint hazir olunca guncelle
  validateSmsCode(dto: ValidateSmsCodeRequestDto): Observable<ValidateSmsCodeResponseDto> {
    // return this.http.post<ValidateSmsCodeResponseDto>(this.srvUrl + 'validateSmsCode', dto, httpOptions);
    console.log('[MOCK] validateSmsCode:', dto);
    return of({
      token: 'mock-jwt-' + Date.now(),
      refreshToken: 'mock-refresh-' + Date.now(),
      expiresIn: 3600,
    }).pipe(delay(1000));
  }

  // TODO: Backend endpoint hazir olunca guncelle
  startChargeFromDevice(dto: StartChargeFromDeviceRequestDto): Observable<StartChargeFromDeviceResponseDto> {
    // return this.http.post<StartChargeFromDeviceResponseDto>(this.srvUrl + 'startChargeFromDevice', dto, httpOptions);
    console.log('[MOCK] startChargeFromDevice:', dto);
    return of({
      isMultipleSocket: true,
      tempChargeSession: 'tcs-' + Date.now(),
      deviceName: 'Rotawatt DC Fast Charger',
      deviceSerialNo: 'R0026',
      deviceLocation: 'Istanbul, Kadikoy',
      sockets: [
        { socketId: 'skt-1', socketName: 'R0026-1', socketType: 'DC CSS', power: 120, pricePerKwh: 7.79, status: SocketStatusType.Available },
        { socketId: 'skt-2', socketName: 'R0026-2', socketType: 'DC CSS', power: 120, pricePerKwh: 7.79, status: SocketStatusType.PluggedIn },
      ],
    }).pipe(delay(1500));
  }
}
