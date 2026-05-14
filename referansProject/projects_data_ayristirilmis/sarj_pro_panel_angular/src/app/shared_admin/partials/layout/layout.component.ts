// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\shared_admin\partials\layout\layout.component.ts

import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewInit,
} from '@angular/core';
import KTLayoutContent from '../../../../assets/js/layout/base/content';
import { DatatableRequestWrapper } from '../../utils/wrapper-models/datatable-request-wrapper.model';
import { LayoutInitService } from './layout-init.service';
import { LayoutService } from './layout.service';
import { environment } from 'src/environments/environment';
import { I } from '@angular/cdk/keycodes';
import { BehaviorSubject, filter, map, tap } from 'rxjs';
import { AuthenticationModel } from 'src/app/sarjAllPro/models/authentication/authentication-model';
import { PanelAdminUserType } from 'src/app/sarjAllPro/enums/authentication/panel-admin-user-type-enum';


@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit, AfterViewInit {

  // Public variables
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
  // offcanvases
  userPartialDisplay = true;
  @ViewChild('ktAside', { static: true }) ktAside: ElementRef;
  @ViewChild('ktHeaderMobile', { static: true }) ktHeaderMobile: ElementRef;
  @ViewChild('ktHeader', { static: true }) ktHeader: ElementRef;

  //#region oturum modelini tutacak değişkenler tanımlanıyor
  admin: AuthenticationModel;
  //#endregion

  constructor(
    private initService: LayoutInitService,
    public layout: LayoutService,
  ) {
    this.initService.init();
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
    //#region admin oturum bilgileri setleniyor
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
    //#endregion
  }

  ngOnDestroy() { }

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
    // Init Content
    KTLayoutContent.init('kt_content');
  }
}
