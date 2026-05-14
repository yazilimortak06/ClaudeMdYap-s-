import { Component, OnInit, AfterViewInit, Input } from '@angular/core';
import { BehaviorSubject, first, Observable, Subscription } from 'rxjs';
import KTLayoutQuickSearch from '../../../../assets/js/layout/extended/quick-search';
import KTLayoutQuickNotifications from '../../../../assets/js/layout/extended/quick-notifications';
import KTLayoutQuickActions from '../../../../assets/js/layout/extended/quick-actions';
import KTLayoutQuickCartPanel from '../../../../assets/js/layout/extended/quick-cart';
import KTLayoutQuickPanel from '../../../../assets/js/layout/extended/quick-panel';
import KTLayoutQuickUser from '../../../../assets/js/layout/extended/quick-user';
import KTLayoutHeaderTopbar from '../../../../assets/js/layout/base/header-topbar';
import { KTUtil } from '../../../../assets/js/components/util';
//import { summaryFileName } from '@angular/compiler/src/aot/util';
import { LayoutService } from '../layout/layout.service';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { SupportNotificationItemModel } from 'src/app/evtech/models/support/support-notification-item-model';
import { PanelAdminUserType } from 'src/app/evtech/enums/authentication/panel-admin-user-type-enum';

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss'],
})

export class TopbarComponent implements OnInit, AfterViewInit {

  @Input() supportNotificationList$: BehaviorSubject<SupportNotificationItemModel[]>;
  // tobbar extras
  extrasQuickPanelDisplay: boolean;
  extrasUserDisplay: boolean;
  extrasUserLayout: 'offcanvas' | 'dropdown';
  admin: AuthenticationModel;
  subscriptions: Subscription[] = [];
  adminUserType = PanelAdminUserType;
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
  }
  ngAfterViewInit(): void {
    KTUtil.ready(() => {
      KTLayoutQuickPanel.init('support_notification_panel');
      KTLayoutQuickUser.init('user_partial');
      KTLayoutHeaderTopbar.init('kt_header_mobile_topbar_toggle');
    });
  }
}
