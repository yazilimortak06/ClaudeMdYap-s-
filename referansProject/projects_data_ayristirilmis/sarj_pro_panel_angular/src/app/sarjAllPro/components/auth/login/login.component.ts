// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\sarjAllPro\components\auth\login\login.component.ts

import { ChangeDetectorRef, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { BehaviorSubject, finalize, Observable, Subject, Subscription, takeUntil, tap } from 'rxjs';
import { SplashScreenService } from 'src/app/shared_admin/template/splash-screen/splash-screen.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { PrepareLoginFormRequestModel } from 'src/app/sarjAllPro/models/authentication/prepare-login-form-request-model';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { PrepareLoginFormResponseModel } from 'src/app/sarjAllPro/models/authentication/prepare-login-form-response-model';
import { LoginRequestModel } from 'src/app/sarjAllPro/models/authentication/login-request-model';
import { LoginResponseModel } from 'src/app/sarjAllPro/models/authentication/login-response-model';
import { AuthenticationModel } from 'src/app/sarjAllPro/models/authentication/authentication-model';
import { PanelAdminModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-model';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  //#region login formunu hazırlamak için request model oluşturuluyor
  prepareLoginFormRequestModel: PrepareLoginFormRequestModel = new PrepareLoginFormRequestModel();
  //#endregion
  //#region login request model oluşturuluyor
  loginRequestModel: LoginRequestModel = new LoginRequestModel();
  //#endregion
  private unsubscribe: Subscription[] = [];
  // Public params
  loginForm: FormGroup;
  errors: any = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvAuthService: AuthenticationService,
    private srvUtils: UtilsService,
    private router: Router,
    private splashScreenService: SplashScreenService,
  ) {
    if (this.srvAuthService.adminValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    this.splashScreenService.hide();
    this.initForm();
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }

  initForm() {
    this.loginForm = this.fb.group({
      userName: [null, Validators.compose([Validators.required])],
      password: [null, Validators.compose([Validators.required])]
    });
    this.prepareLoginForm();
  }

  prepareLoginForm() {
    this.srvProgressSpinner.show();
    this.srvAuthService.prepareLoginForm(this.prepareLoginFormRequestModel).subscribe((response: PrepareLoginFormResponseModel) => {
      this.loginRequestModel.loginFormKey = response.loginFormKey;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorPrepareLoginForm(error);
    });
  }

  onErrorPrepareLoginForm(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  login() {
    this.srvProgressSpinner.show();
    this.srvAuthService.isLoadingSubject.next(true);
    const controls = this.loginForm.controls;
    /** check form */
    if (this.loginForm.invalid) {
      Object.keys(controls).forEach(controlName =>
        controls[controlName].markAsTouched()
      );
      this.srvProgressSpinner.hide();
      return;
    }
    const loginSubscr = this.srvAuthService
      .logIn(this.loginRequestModel).pipe()
      .subscribe((auth: LoginResponseModel) => {
        if (auth.accessToken) {
          //#region authentication model oluşturuluyor
          var authenticationModel = new AuthenticationModel();
          authenticationModel.token = auth.accessToken;
          authenticationModel.name = auth.name;
          authenticationModel.surname = auth.surname;
          authenticationModel.phone = auth.phone;
          authenticationModel.mail = auth.mail;
          authenticationModel.connectionId = auth.connectionId;
          authenticationModel.companyName = auth.companyName;
          authenticationModel.companyId = auth.companyId;
          authenticationModel.panelAdminUserType = auth.panelAdminUserType;
          //#endregion
          this.srvAuthService.setAuthFromLocalStorage(authenticationModel);
          this.srvAuthService.adminSubject = new BehaviorSubject<AuthenticationModel>(authenticationModel);
          this.router.navigate(['']);
        }
        this.srvAuthService.isLoadingSubject.next(false);
      },
        (error) => {
          this.onErrorLogin(error);
          this.srvAuthService.isLoadingSubject.next(false);
          this.srvProgressSpinner.hide();
        });
    this.unsubscribe.push(loginSubscr);
  }

  onErrorLogin(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    console.log(errorData.errorMessage);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  isControlHasError(controlName: string, validationType: string): boolean {
    const control = this.loginForm.controls[controlName];
    if (!control) { return false; }
    const result = control.hasError(validationType) && (control.dirty || control.touched);
    return result;
  }
}
