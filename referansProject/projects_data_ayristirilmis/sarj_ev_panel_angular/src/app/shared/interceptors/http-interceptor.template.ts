import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class HttpEventInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    // 1. Token'ı localStorage'dan al
    const token = localStorage.getItem('token');

    // 2. Token varsa Authorization header ekle
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }

    // 3. Progress spinner başlat (isteğe bağlı)
    // SpinnerService.show();

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        // 4. 401 Unauthorized — kullanıcıyı logout et
        if (error.status === 401) {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          this.router.navigate(['/auth/login']);
        }

        // 5. 403 Forbidden
        if (error.status === 403) {
          this.router.navigate(['/error/403']);
        }

        return throwError(() => error);
      }),
      finalize(() => {
        // 6. Progress spinner durdur
        // SpinnerService.hide();
      })
    );
  }
}
