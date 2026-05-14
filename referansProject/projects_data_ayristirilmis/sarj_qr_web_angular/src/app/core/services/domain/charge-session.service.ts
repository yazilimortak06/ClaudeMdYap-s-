// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\core\services\domain\charge-session.service.ts
import { Injectable } from '@angular/core';

// ============================================================
// ChargeSessionService
// localStorage'daki chargeSessionToken yonetimi (domain logic)
// ============================================================
@Injectable({ providedIn: 'root' })
export class ChargeSessionService {
  private storageKey = 'chargeSessionToken';

  setToken(token: string): void {
    localStorage.setItem(this.storageKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.storageKey);
  }

  clearToken(): void {
    localStorage.removeItem(this.storageKey);
  }

  hasActiveSession(): boolean {
    return !!this.getToken();
  }
}
