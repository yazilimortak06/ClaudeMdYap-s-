// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\layout\layout.service.ts
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import * as objectPath from 'object-path';
import { DefaultLayoutConfig } from './default-layout.config';
import { SupportNotificationItemModel } from 'src/app/evtech/models/support/support-notification-item-model';
import * as signalR from "@microsoft/signalr";
import { HttpClient } from '@angular/common/http';
import { SupportRequestModel } from 'src/app/evtech/models/support/support-request-model';
import { SupportState } from 'src/app/evtech/enums/support/support-state-enum';

const LAYOUT_CONFIG_LOCAL_STORAGE_KEY = `${environment.appVersion}-layoutConfig`;

@Injectable({ providedIn: 'root' })
export class LayoutService {
  private layoutConfigSubject: BehaviorSubject<any> = new BehaviorSubject<any>(undefined);

  searchCriterOtherSupport: SupportRequestModel = new SupportRequestModel();

  private listForNotification(searchCriter: SupportRequestModel): Observable<SupportNotificationItemModel[]> {
    return this.http.post<SupportNotificationItemModel[]>(this.srvUrl + "GetSupportListForNotification", searchCriter);
  }

  private check(newSupport: SupportNotificationItemModel, supports: SupportNotificationItemModel[]) {
    if (this.supportNotificationList$.getValue().filter(item => item.supportId == newSupport.supportId).length == 0 && newSupport != null) {
      let newSupportArray: SupportNotificationItemModel[] = [];
      newSupportArray.push(newSupport);
      supports.push(...newSupportArray);
      supports = supports.filter(x => x.state != SupportState.WAITING);
      return supports.sort((a, b) => (a.lastUpdateDate > b.lastUpdateDate ? -1 : 1));
    } else {
      this.supportNotificationList$.getValue().filter(item => item.supportId == newSupport.supportId && item.lastUpdateDate <= newSupport.lastUpdateDate).map(data => {
        data.state = newSupport.state;
        data.lastUpdateDate = newSupport.lastUpdateDate;
      });
      supports = supports.filter(x => x.state == SupportState.WAITING);
      return supports.sort((a, b) => (a.lastUpdateDate > b.lastUpdateDate ? -1 : 1));
    }
  }

  supportNotificationList$: BehaviorSubject<SupportNotificationItemModel[]> = new BehaviorSubject<SupportNotificationItemModel[]>([]);
  supportNotification$: BehaviorSubject<SupportNotificationItemModel> = new BehaviorSubject<SupportNotificationItemModel>(null);

  supportNotificationsSubscribeCallback(support: SupportNotificationItemModel) {
    this.supportNotificationList$.next(this.check(support, this.supportNotificationList$.getValue()));
  }

  private supportNotificationHubConnection: signalR.HubConnection;

  public startConnectionSupportNotification = () => {
    this.supportNotificationHubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.notificationApiUrl}supportForPanelNotification`)
      .build();
    this.supportNotificationHubConnection
      .start()
      .then(() => { console.log("hub connection started"); })
      .catch(err => { console.log(err); });
  };

  public addListenerSupport = () => {
    this.supportNotificationHubConnection.on("supportForPanelNotification", (data: SupportNotificationItemModel) => {
      this.supportNotification$.next(data);
    });
  };

  private classes = {
    header: [], header_container: [], header_mobile: [], header_menu: [],
    aside_menu: [], subheader: [], subheader_container: [], content: [],
    content_container: [], footer_container: [],
  };
  private attrs = { aside_menu: {} };
  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "SupportManagment/";
  }

  initConfig(): any {
    const configFromLocalStorage = localStorage.getItem(LAYOUT_CONFIG_LOCAL_STORAGE_KEY);
    if (configFromLocalStorage) {
      try {
        this.layoutConfigSubject.next(JSON.parse(configFromLocalStorage));
        return;
      } catch (error) {
        this.removeConfig();
      }
    }
    this.layoutConfigSubject.next(DefaultLayoutConfig);
  }

  private removeConfig() { localStorage.removeItem(LAYOUT_CONFIG_LOCAL_STORAGE_KEY); }

  refreshConfigToDefault() { this.setConfigWithPageRefresh(undefined); }

  getConfig(): any {
    const config = this.layoutConfigSubject.value;
    if (!config) { return DefaultLayoutConfig; }
    return config;
  }

  setConfig(config: any) {
    if (!config) { this.removeConfig(); }
    else { localStorage.setItem(LAYOUT_CONFIG_LOCAL_STORAGE_KEY, JSON.stringify(config)); }
    this.layoutConfigSubject.next(config);
  }

  setConfigWithoutLocalStorageChanges(config: any) { this.layoutConfigSubject.next(config); }
  setConfigWithPageRefresh(config: any) { this.setConfig(config); document.location.reload(); }
  getProp(path: string): any { return objectPath.get(this.layoutConfigSubject.value, path); }

  setCSSClass(path: string, classesInStr: string) {
    const cssClasses = this.classes[path];
    if (!cssClasses) { this.classes[path] = []; }
    classesInStr.split(' ').forEach((cssClass: string) => this.classes[path].push(cssClass));
  }

  getCSSClasses(path: string): string[] {
    const cssClasses = this.classes[path];
    if (!cssClasses) { return []; }
    return cssClasses;
  }

  getStringCSSClasses(path: string) { return this.getCSSClasses(path).join(' '); }

  getHTMLAttributes(path: string): any {
    const attributesObj = this.attrs[path];
    if (!attributesObj) { return {}; }
    return attributesObj;
  }

  setHTMLAttribute(path, attrKey: string, attrValue: any) {
    const attributesObj = this.attrs[path];
    if (!attributesObj) { this.attrs[path] = {}; }
    this.attrs[path][attrKey] = attrValue;
  }
}
