// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\core\guards\auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthStorageService } from 'src/app/core/services/domain/auth-storage.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authStorageService: AuthStorageService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const currentUser = this.authStorageService.userValue;
    if (currentUser && this.authStorageService.getAuth()) {
      return true;
    }
    // returnUrl'i sakla, login sonrasi geri donulecek
    this.router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }
}
