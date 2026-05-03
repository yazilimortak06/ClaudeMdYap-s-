import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { UserProfile } from '../../core/models/user.model';

@Component({
  selector: 'app-dashboard',
  template: `
    <div class="dashboard-container">
      <h1>Dashboard</h1>
      <p>Hoşgeldiniz! Bu ana sayfa placeholder bileşenidir.</p>
      <button mat-raised-button color="warn" (click)="logout()">Çıkış Yap</button>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 24px;
    }
  `]
})
export class DashboardComponent implements OnInit {
  currentUser: UserProfile | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // Load current user data if needed
  }

  logout(): void {
    this.authService.logout();
  }
}
