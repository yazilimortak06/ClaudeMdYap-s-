// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\auth\authentication-service.ts
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, finalize, map, Observable, of } from "rxjs";
import { environment } from "src/environments/environment";
import { AuthenticationModel } from "../../evtech/models/authentication/authentication-model";
import { LoginRequestModel } from "../../evtech/models/authentication/login-request-model";
import { LoginResponseModel } from "../../evtech/models/authentication/login-response-model";
import { LogoutRequestModel } from "../../evtech/models/authentication/logout-request-model";
import { PrepareLoginFormRequestModel } from "../../evtech/models/authentication/prepare-login-form-request-model";
import { PrepareLoginFormResponseModel } from "../../evtech/models/authentication/prepare-login-form-response-model";
import { PanelAdminModel } from "../../evtech/models/panelAdmin/panelAdmin-model";

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class AuthenticationService {

    public adminSubject: BehaviorSubject<AuthenticationModel> = new BehaviorSubject<AuthenticationModel>(null);
    isLoadingSubject: BehaviorSubject<boolean>;
    private authLocalStorageToken = "auth_token";
    private authUserInfo = "user_info";
    private srvUrl: string;

    constructor(private http: HttpClient, private router: Router) {
        this.adminSubject = new BehaviorSubject<AuthenticationModel>(null);
        this.isLoadingSubject = new BehaviorSubject<boolean>(false);
        this.srvUrl = environment.apiUrl + "Authentication/";
    }

    public get adminValue(): AuthenticationModel {
        return this.adminSubject.value;
    }

    public setAuthFromLocalStorage(auth: AuthenticationModel): boolean {
        if (auth && auth.token) {
            localStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
            return true;
        }
        return false;
    }

    public getAuthFromLocalStorage(): Observable<AuthenticationModel> {
        try {
            const authData: AuthenticationModel = JSON.parse(
                localStorage.getItem(this.authLocalStorageToken)
            );
            return of(authData);
        } catch (error) {
            this.logout();
            return of(undefined);
        }
    }

    logout() {
        if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
            this.userLogout().subscribe(auth => { });
        }
        localStorage.removeItem(this.authLocalStorageToken);
        localStorage.removeItem(this.authUserInfo);
        this.adminSubject = new BehaviorSubject<AuthenticationModel>(undefined);
        this.router.navigate(['/auth/login']);
    }

    getAdminByToken(): Observable<AuthenticationModel> {
        const auth = this.getAuthFromLocalStorage();
        if (!auth) { return of(undefined); }
        this.isLoadingSubject.next(true);
        return this.getAuthFromLocalStorage().pipe(
            map((admin: AuthenticationModel) => {
                if (admin) {
                    this.adminSubject = new BehaviorSubject<AuthenticationModel>(admin);
                } else {
                    this.logout();
                }
                return admin;
            }),
            finalize(() => this.isLoadingSubject.next(false))
        );
    }

    public getAdminInfoFromLocalStorage(): Observable<PanelAdminModel> {
        try {
            const authData: PanelAdminModel = JSON.parse(
                localStorage.getItem(this.authUserInfo)
            );
            return of(authData);
        } catch (error) {
            this.logout();
            return of(undefined);
        }
    }

    getNewTokenWithRefreshToken() {
        if (this.authLocalStorageToken != null && this.authLocalStorageToken != "auth_token") {
            const token: AuthenticationModel = JSON.parse(
                localStorage.getItem(this.authLocalStorageToken)
            );
            this.adminLoginRefreshToken(token).subscribe(auth => {
                this.setAuthFromLocalStorage(auth);
            });
        }
    }

    clearTokenFromLocalStorage() {
        localStorage.removeItem(this.authLocalStorageToken);
    }

    prepareLoginForm(loginRequest: PrepareLoginFormRequestModel) {
        return this.http.post<PrepareLoginFormResponseModel>(this.srvUrl + "LoginForm", loginRequest, httpOptions);
    }

    logIn(loginRequest: LoginRequestModel) {
        return this.http.post<LoginResponseModel>(this.srvUrl + "Login", loginRequest, httpOptions);
    }

    userLogout() {
        return this.http.post<any>(this.srvUrl + "LogOut", null, httpOptions);
    }

    adminLoginRefreshToken(refreshToken: AuthenticationModel): Observable<AuthenticationModel> {
        return this.http.post<any>(this.srvUrl + "RefreshTokenLogin", refreshToken, httpOptions);
    }

    userLoginRefreshToken(refreshToken: AuthenticationModel): Observable<AuthenticationModel> {
        return this.http.post<any>("RefreshTokenLogin", refreshToken, httpOptions);
    }
}
