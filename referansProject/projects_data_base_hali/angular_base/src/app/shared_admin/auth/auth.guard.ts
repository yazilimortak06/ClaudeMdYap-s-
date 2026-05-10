// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\auth\auth.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthenticationService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const currentUser = this.authService.adminValue;
    if (currentUser && this.authService.getAuthFromLocalStorage()) {
      return true;
    }
    this.authService.logout();
    return false;
  }
}
