import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.checkAuth(state.url);
  }

  private checkAuth(redirectUrl: string): boolean | UrlTree {
    const token = localStorage.getItem('token');

    if (!token) {
      return this.router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: redirectUrl },
      });
    }

    return true;
  }
}
