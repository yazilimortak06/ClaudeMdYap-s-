import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

import { LoginRequest, LoginResponse, TokenPayload } from '../models/auth.model';
import { ApiResponse } from '../models/api-response.model';
import { StorageService } from './storage.service';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    private storage: StorageService,
    private jwtHelper: JwtHelperService
  ) {}

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(
        `${environment.authUrl}/auth/login`,
        request
      )
      .pipe(
        tap((response) => {
          if (response.success && response.data) {
            this.storage.setItem(this.TOKEN_KEY, response.data.accessToken);
            this.storage.setItem(
              this.REFRESH_TOKEN_KEY,
              response.data.refreshToken
            );
            this.isAuthenticatedSubject.next(true);
          }
        })
      );
  }

  logout(): void {
    this.storage.removeItem(this.TOKEN_KEY);
    this.storage.removeItem(this.REFRESH_TOKEN_KEY);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return this.storage.getItem(this.TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }
    return !this.jwtHelper.isTokenExpired(token);
  }

  getTokenPayload(): TokenPayload | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    return this.jwtHelper.decodeToken<TokenPayload>(token);
  }

  checkTokenValidity(): void {
    const authenticated = this.isAuthenticated();
    this.isAuthenticatedSubject.next(authenticated);
  }

  refreshToken(): Observable<ApiResponse<LoginResponse>> {
    const refreshToken = this.storage.getItem(this.REFRESH_TOKEN_KEY);
    return this.http
      .post<ApiResponse<LoginResponse>>(
        `${environment.authUrl}/auth/refresh-token`,
        { refreshToken }
      )
      .pipe(
        tap((response) => {
          if (response.success && response.data) {
            this.storage.setItem(this.TOKEN_KEY, response.data.accessToken);
            this.storage.setItem(
              this.REFRESH_TOKEN_KEY,
              response.data.refreshToken
            );
          }
        })
      );
  }
}
