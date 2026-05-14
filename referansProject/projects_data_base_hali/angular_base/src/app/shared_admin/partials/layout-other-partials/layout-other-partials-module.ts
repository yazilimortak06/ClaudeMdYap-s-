import { CommonModule, KeyValuePipe } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { InlineSVGModule } from "ng-inline-svg";
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from "ngx-perfect-scrollbar";
import { SupportService } from "src/app/evtech/services/support/support-service";
import { ScrollTopPartialComponent } from "./scroll-top-partial/scroll-top-partial.component";
import { SupportNotificationPartialComponent } from "./support-notification-partial/support-notification-partial.component";
import { UserPartialComponent } from "./user-partial/user-partial.component";

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    suppressScrollX: true,
  };
  @NgModule({
    declarations: [
        SupportNotificationPartialComponent,
        UserPartialComponent,
        ScrollTopPartialComponent
    ],
    imports: [CommonModule, InlineSVGModule, PerfectScrollbarModule, RouterModule],
    providers: [
      {
        provide: PERFECT_SCROLLBAR_CONFIG,
        useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG,
      },
      SupportService,
      KeyValuePipe
    ],
    exports: [
        SupportNotificationPartialComponent,
        UserPartialComponent,
        ScrollTopPartialComponent
    ],
  })
  export class LayoutOtherPartialsModule { }
