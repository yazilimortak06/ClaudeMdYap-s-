import { KeyValuePipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { SupportItemModel } from 'src/app/evtech/models/support/support-item-model';
import { SupportNotificationItemModel } from 'src/app/evtech/models/support/support-notification-item-model';
import { SupportRequestModel } from 'src/app/evtech/models/support/support-request-model';
import { SupportService } from 'src/app/evtech/services/support/support-service';
import { LayoutService } from 'src/app/shared_admin/partials/layout/layout.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';



@Component({
    selector: 'app-support-notification-partial',
    templateUrl: './support-notification-partial.component.html',
    styleUrls: ['./support-notification-partial.component.scss'],
})
export class SupportNotificationPartialComponent implements OnInit {
    @Input() supportNotificationList$: BehaviorSubject<SupportNotificationItemModel[]>;
    //#region  oturum modelini tutacak değişkenler tanımlanıyor
    admin: AuthenticationModel;
    //#endregion
    extrasQuickPanelOffcanvasDirectionCSSClass = 'offcanvas-right';
    activeTabId: string = 'kt_quick_panel_notifications';
    constructor(private layout: LayoutService,
        private srvSupportService: SupportService,
        private keyValuePipe: KeyValuePipe,
        private router: Router,
    ) { }
    ngOnInit(): void {
        this.extrasQuickPanelOffcanvasDirectionCSSClass = `offcanvas-${this.layout.getProp(
            'extras.quickPanel.offcanvas.direction'
        )}`;
        console.log("init oldu");
        //#region  admin oturum bilgileri setleniyor
        this.admin = JSON.parse(localStorage.getItem("auth_token"));
        //#endregion
    }
    setActiveTabId(tabId) {
        this.activeTabId = tabId;
    }
    getActiveCSSClasses(tabId) {
        if (tabId !== this.activeTabId) {
            return '';
        }
        return 'active show';
    }
    goSupportDetail(supportId) {
        this.router.navigate(["support/detail", { id: supportId }])
    }
}
