// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\layout\layout.component.ts
import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { SupportItemModel } from 'src/app/evtech/models/support/support-item-model';
import { SupportRequestModel } from 'src/app/evtech/models/support/support-request-model';
import { SupportService } from 'src/app/evtech/services/support/support-service';
import KTLayoutContent from '../../../../assets/js/layout/base/content';
import { LayoutInitService } from './layout-init.service';
import { LayoutService } from './layout.service';
import { SupportNotificationItemModel } from 'src/app/evtech/models/support/support-notification-item-model';
import { BehaviorSubject } from 'rxjs';
import { GetSupportNotificationItemResponseModel } from 'src/app/evtech/models/support/get-support-notification-item-response-model';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { PanelAdminUserType } from 'src/app/evtech/enums/authentication/panel-admin-user-type-enum';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit, AfterViewInit {
  supportNotificationList$: BehaviorSubject<SupportNotificationItemModel[]> = new BehaviorSubject<SupportNotificationItemModel[]>([]);

  selfLayout = 'default';
  asideSelfDisplay: true;
  asideMenuStatic: false;
  contentClasses = '';
  contentContainerClasses = '';
  subheaderDisplay = true;
  contentExtended: false;
  asideCSSClasses: string;
  asideHTMLAttributes: any = {};
  headerMobileClasses = '';
  headerMobileAttributes = {};
  footerDisplay: boolean;
  footerCSSClasses: string;
  headerCSSClasses: string;
  headerHTMLAttributes: any = {};
  userPartialDisplay = true;

  @ViewChild('ktAside', { static: true }) ktAside: ElementRef;
  @ViewChild('ktHeaderMobile', { static: true }) ktHeaderMobile: ElementRef;
  @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

  supportList$ = new BehaviorSubject<SupportItemModel[]>([]);
  offset: number = 0;
  searchCriterOtherSupport: SupportRequestModel = new SupportRequestModel();
  admin: AuthenticationModel;

  constructor(
    private initService: LayoutInitService,
    public layout: LayoutService,
    private srvSupportService: SupportService,
  ) {
    this.initService.init();
  }

  public updateSupport(oldSupport: SupportItemModel, newSupport: SupportNotificationItemModel) {
    oldSupport.state = newSupport.state;
    oldSupport.lastUpdateDate = newSupport.lastUpdateDate;
    return oldSupport;
  }

  ngOnInit(): void {
    this.selfLayout = this.layout.getProp('self.layout');
    this.asideSelfDisplay = this.layout.getProp('aside.self.display');
    this.asideMenuStatic = this.layout.getProp('aside.menu.static');
    this.subheaderDisplay = this.layout.getProp('subheader.display');
    this.contentClasses = this.layout.getStringCSSClasses('content');
    this.contentContainerClasses = this.layout.getStringCSSClasses('content_container-fluid');
    this.contentExtended = this.layout.getProp('content.extended');
    this.asideHTMLAttributes = this.layout.getHTMLAttributes('aside');
    this.asideCSSClasses = this.layout.getStringCSSClasses('aside');
    this.headerMobileClasses = this.layout.getStringCSSClasses('header_mobile');
    this.headerMobileAttributes = this.layout.getHTMLAttributes('header_mobile');
    this.footerDisplay = this.layout.getProp('footer.display');
    this.footerCSSClasses = this.layout.getStringCSSClasses('footer');
    this.headerCSSClasses = this.layout.getStringCSSClasses('header');
    this.headerHTMLAttributes = this.layout.getHTMLAttributes('header');
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
    if (this.admin.panelAdminUserType == PanelAdminUserType.ADMIN_USER || this.admin.panelAdminUserType == PanelAdminUserType.ROOT_ADMIN_USER) {
      this.supportNotificationList$ = this.layout.supportNotificationList$;
      this.layout.supportNotification$.subscribe(support => {
        this.layout.supportNotificationsSubscribeCallback(support);
      });
      this.initSupportNotification();
    }
  }

  initSupportNotification() {
    return new Promise<void>((resolve, reject) => {
      this.srvSupportService.listForNotification(this.searchCriterOtherSupport).subscribe((response: GetSupportNotificationItemResponseModel) => {
        this.layout.supportNotificationList$.next(response.supports.sort((a, b) => (a.lastUpdateDate > b.lastUpdateDate ? -1 : 1)));
        this.layout.startConnectionSupportNotification();
        this.layout.addListenerSupport();
        resolve();
      }, (error) => { reject(); });
    });
  }

  ngOnDestroy() {
    this.layout.supportNotification$.unsubscribe();
    this.layout.supportNotificationList$.unsubscribe();
  }

  ngAfterViewInit(): void {
    if (this.ktAside) {
      for (const key in this.asideHTMLAttributes) {
        if (this.asideHTMLAttributes.hasOwnProperty(key)) {
          this.ktAside.nativeElement.attributes[key] = this.asideHTMLAttributes[key];
        }
      }
    }
    if (this.ktHeaderMobile) {
      for (const key in this.headerMobileAttributes) {
        if (this.headerMobileAttributes.hasOwnProperty(key)) {
          this.ktHeaderMobile.nativeElement.attributes[key] = this.headerMobileAttributes[key];
        }
      }
    }
    if (this.ktHeader) {
      for (const key in this.headerHTMLAttributes) {
        if (this.headerHTMLAttributes.hasOwnProperty(key)) {
          this.ktHeader.nativeElement.attributes[key] = this.headerHTMLAttributes[key];
        }
      }
    }
    KTLayoutContent.init('kt_content');
  }
}
