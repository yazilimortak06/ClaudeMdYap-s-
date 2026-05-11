import { Component, OnInit } from '@angular/core';
import { first, Observable, Subscription } from 'rxjs'
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { LayoutService } from 'src/app/shared_admin/partials/layout/layout.service';


@Component({
  selector: 'app-user-partial',
  templateUrl: './user-partial.component.html',
  styleUrls: ['./user-partial.component.scss'],
})
export class UserPartialComponent implements OnInit {
  direction = 'offcanvas-right';
  admin: AuthenticationModel;
  subscriptions: Subscription[] = [];
  constructor(private layout: LayoutService,
    private srvAuthService: AuthenticationService
    ) {

  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
  ngOnInit(): void {
    const sb = this.srvAuthService.adminSubject.asObservable().pipe(
      first(admin => !!admin)
    ).subscribe(admin => {
      this.admin = Object.assign({}, admin);
    });
    this.subscriptions.push(sb);
    console.log(this.admin);
    this.direction = `offcanvas-${this.layout.getProp(
      'extras.user.offcanvas.direction'
    )}`;
  }
  logout() {
    this.srvAuthService.logout();
    document.location.reload();
  }
}
