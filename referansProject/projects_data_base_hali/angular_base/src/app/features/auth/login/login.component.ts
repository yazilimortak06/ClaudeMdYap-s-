import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  template: `
    <div class="login-container">
      <mat-card class="login-card">
        <mat-card-header>
          <mat-card-title>Giriş Yap</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Kullanıcı Adı</mat-label>
              <input matInput formControlName="username" placeholder="Kullanıcı adınızı girin" />
              <mat-error *ngIf="loginForm.get('username')?.hasError('required')">
                Kullanıcı adı zorunludur.
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Şifre</mat-label>
              <input
                matInput
                [type]="hidePassword ? 'password' : 'text'"
                formControlName="password"
                placeholder="Şifrenizi girin"
              />
              <button mat-icon-button matSuffix type="button" (click)="hidePassword = !hidePassword">
                <mat-icon>{{ hidePassword ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
              <mat-error *ngIf="loginForm.get('password')?.hasError('required')">
                Şifre zorunludur.
              </mat-error>
            </mat-form-field>

            <div *ngIf="errorMessage" class="error-message">
              {{ errorMessage }}
            </div>

            <button
              mat-raised-button
              color="primary"
              type="submit"
              class="full-width"
              [disabled]="loginForm.invalid || isLoading"
            >
              <mat-spinner diameter="20" *ngIf="isLoading"></mat-spinner>
              <span *ngIf="!isLoading">Giriş Yap</span>
            </button>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background-color: #f5f5f5;
    }
    .login-card {
      width: 100%;
      max-width: 400px;
      padding: 16px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }
    .error-message {
      color: red;
      margin-bottom: 16px;
      font-size: 14px;
    }
  `]
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;
  hidePassword = true;
  errorMessage = '';
  returnUrl = '/';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    if (this.authService.isAuthenticated()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const { username, password } = this.loginForm.value;

    this.authService.login({ username, password }).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigate([this.returnUrl]);
        } else {
          this.errorMessage = response.message || 'Giriş başarısız.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage =
          err?.error?.message || 'Bir hata oluştu. Lütfen tekrar deneyin.';
      }
    });
  }
}
