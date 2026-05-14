// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\aside\aside.component.ts

import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { PanelAdminUserType } from 'src/app/evtech/enums/authentication/panel-admin-user-type-enum';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { LayoutService } from '../layout/layout.service';


@Component({
  selector: 'app-aside',
  templateUrl: './aside.component.html',
  styleUrls: ['./aside.component.scss'],
})
export class AsideComponent implements OnInit {
  disableAsideSelfDisplay: boolean;
  headerLogo: string;
  brandSkin: string;
  ulCSSClasses: string;
  location: Location;
  asideMenuHTMLAttributes: any = {};
  asideMenuCSSClasses: string;
  asideMenuDropdown;
  brandClasses: string;
  asideMenuScroll = 1;
  asideSelfMinimizeToggle = false;

  private authLocalStorageToken = "auth_token";
  authenticationData : AuthenticationModel = new AuthenticationModel();
  panelAdminUserType = PanelAdminUserType;
  constructor(private layout: LayoutService,private authService: AuthenticationService, private loc: Location) {
    this.authenticationData = JSON.parse(localStorage.getItem(this.authLocalStorageToken));
  }

  ngOnInit(): void {
    // load view settings
    this.disableAsideSelfDisplay =
      this.layout.getProp('aside.self.display') === false;
    this.brandSkin = this.layout.getProp('brand.self.theme');
    this.headerLogo = this.getLogo();
    this.ulCSSClasses = this.layout.getProp('aside_menu_nav');
    this.asideMenuCSSClasses = this.layout.getStringCSSClasses('aside_menu');
    this.asideMenuHTMLAttributes = this.layout.getHTMLAttributes('aside_menu');
    this.asideMenuDropdown = this.layout.getProp('aside.menu.dropdown') ? '1' : '0';
    this.brandClasses = this.layout.getProp('brand');
    this.asideSelfMinimizeToggle = this.layout.getProp(
      'aside.self.minimize.toggle'
    );
    this.asideMenuScroll = this.layout.getProp('aside.menu.scroll') ? 1 : 0;
    // this.asideMenuCSSClasses = `${this.asideMenuCSSClasses} ${this.asideMenuScroll === 1 ? 'scroll my-4 ps ps--active-y' : ''}`;
    // Routing
    this.location = this.loc;
  }

  private getLogo() {
    if (this.brandSkin === 'light') {
      return './assets/project-assets/evtechlogo.svg';
    } else {
      return './assets/project-assets/evtechlogowhite.svg';
    }
  }
}
