// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\interseptors\http-event-interseptor.ts
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest, HttpResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ProgressSpinnerService } from '../../partials/dialogs/progress-spinner/progress-spinner.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';

@Injectable()
export class HttpEventInterceptor implements HttpInterceptor {

    private authLocalStorageToken = "auth_token";

    constructor(
        private authService: AuthenticationService,
        public srvProgressSpinner: ProgressSpinnerService
    ) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const authenticationModel: AuthenticationModel = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
        if (authenticationModel) {
            request = this.addTokenHeader(request, authenticationModel);
        }
        return next.handle(request)
            .pipe(
                tap((event: HttpEvent<any>) => {
                    if (event instanceof HttpResponse) {
                        this.srvProgressSpinner.hide();
                    }
                }, (error: any) => {
                    if (error.status === 400) {
                        this.srvProgressSpinner.hide();
                    } else if (error instanceof HttpErrorResponse && error.status === 401) {
                        return this.handle401Error(request, next);
                    } else {
                        this.srvProgressSpinner.hide();
                    }
                })
            );
    }

    private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
        const authenticationModel: AuthenticationModel = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
        if (authenticationModel) {
            this.authService.clearTokenFromLocalStorage();
            this.authService.logout();
        } else {
            this.srvProgressSpinner.hide();
            this.authService.logout();
        }
    }

    private addTokenHeader(request: HttpRequest<any>, authenticationModel: AuthenticationModel) {
        if (this.authService.adminSubject !== undefined) {
            try {
                const headers = new HttpHeaders()
                    .set('Authorization', 'Bearer ' + authenticationModel.token.token);
                return request = request.clone({ headers });
            } catch {
                this.authService.logout();
            }
        } else {
            const headers = new HttpHeaders()
                .set('Authorization', 'Bearer ' + authenticationModel.token.token);
            return request = request.clone({ headers });
        }
    }
}

export const httpEventInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: HttpEventInterceptor,
    multi: true
};
