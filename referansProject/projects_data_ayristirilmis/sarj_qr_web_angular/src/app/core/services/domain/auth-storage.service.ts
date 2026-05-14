// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\core\services\domain\auth-storage.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { AuthToken } from 'src/app/core/models/auth';

// ============================================================
// AuthStorageService
// Auth token yonetimi - localStorage + BehaviorSubject (domain logic)
// ============================================================
@Injectable({ providedIn: 'root' })
export class AuthStorageService {
  public userSubject = new BehaviorSubject<AuthToken | null>(null);
  public isLoadingSubject = new BehaviorSubject<boolean>(false);

  private storageKey = 'auth_token';

  get userValue(): AuthToken | null {
    return this.userSubject.value;
  }

  get isLoggedIn(): boolean {
    return !!this.userValue?.token;
  }

  setAuth(auth: AuthToken): boolean {
    if (auth && auth.token) {
      localStorage.setItem(this.storageKey, JSON.stringify(auth));
      this.userSubject.next(auth);
      return true;
    }
    return false;
  }

  getAuth(): AuthToken | null {
    try {
      const raw = localStorage.getItem(this.storageKey);
      return raw ? (JSON.parse(raw) as AuthToken) : null;
    } catch {
      this.clearAuth();
      return null;
    }
  }

  clearAuth(): void {
    localStorage.removeItem(this.storageKey);
    this.userSubject.next(null);
  }

  // APP_INITIALIZER tarafindan cagirilir
  restoreFromStorage(): Observable<AuthToken | null> {
    const auth = this.getAuth();
    if (auth) {
      this.userSubject.next(auth);
    }
    return of(auth);
  }
}
