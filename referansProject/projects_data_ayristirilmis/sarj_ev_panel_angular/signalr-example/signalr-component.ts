// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\chargeDevice\chargeDevice-remote-managment\chargeDevice-remote-managment.component.ts
// Pattern: SignalR Component — @microsoft/signalr ile iki ayrı hub bağlantısı, BehaviorSubject ile real-time notification, ngOnDestroy'da bağlantı kapatma

import { GoogleMapsAPIWrapper, Marker } from '@agm/core';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { merge, tap, Subject, BehaviorSubject, Subscription } from 'rxjs';
import { ChargeDeviceFilterModel } from 'src/app/evtech/models/chargeDevice/chargeDevice-filter-model';
import { ChargeDeviceListModel } from 'src/app/evtech/models/chargeDevice/chargeDevice-list-model';
import { ChargeDeviceService } from 'src/app/evtech/services/chargeDevice/chargeDevice-service';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';
import { ChargeDeviceStatusRequestModel } from 'src/app/evtech/models/chargeDevice/chargeDevice-status-request-model';
import { ChargeDeviceRemoteManagmentService } from 'src/app/evtech/services/chargeDeviceRemoteManagment/chargeDevice-remoteManagment-service';
import { ChargeDeviceStatusResponseModel } from 'src/app/evtech/models/chargeDevice/chargeDevice-status-response-model';
import { ChargeDeviceStatusNotificationService } from 'src/app/evtech/services/chargeDeviceRemoteManagment/chargeDevice-status-notification-service';
import { ChargeDeviceNotificationStatusModel } from 'src/app/evtech/models/chargeDevice/chargeDevice-status-notification-model';
import { ChargeDeviceCommandManagmentRequestModel } from 'src/app/evtech/models/chargeDeviceCommandManagment/chargeDevice-commandManagment-request-model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ChargeDeviceCommandComponent } from '../chargeDevice-command/chargeDevice-command.component';
import { ChargeDeviceCommandListComponent } from '../chargeDevice-command-list/chargeDevice-command-list.component';
import { ChargeDeviceRemoteStartTransactionRequestModel } from 'src/app/evtech/models/chargeDeviceRemote/chargeDeviceRemote-startTransaction-request-model';
import { ChargeDeviceRemoteService } from 'src/app/evtech/services/chargeDevice/chargeDevice-remote-service';
import { ChargeDeviceRemoteStartTransactionResponseModel } from 'src/app/evtech/models/chargeDeviceRemote/chargeDeviceRemote-startTransaction-response-model';
import { ChargeDeviceRemoteStopTransactionRequestModel } from 'src/app/evtech/models/chargeDeviceRemote/chargeDeviceRemote-stopTransaction-request-model';
import { ChargeDeviceRemoteStopTransactionResponseModel } from 'src/app/evtech/models/chargeDeviceRemote/chargeDeviceRemote-stopTransaction-response-model';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { environment } from 'src/environments/environment';
import * as signalR from "@microsoft/signalr";
import { ChargeDeviceOcppState, chargeDeviceOcppStateMapping } from 'src/app/evtech/enums/chargeDevice/charge-device-ocpp-state-enum';
import { ConnectorRemoteManagmentPartialComponent } from '../connector-remote-managment-partial/connector-remote-managment-partial.component';
import { ChargeDeviceAvailabilityType, chargeDeviceAvailabilityTypeMapping } from 'src/app/evtech/enums/chargeDevice/charge-device-availability-type-enum';
import { ChargeDeviceRemoteChangeAvailabilityRequestModel } from 'src/app/evtech/models/chargeDeviceRemote/chargeDeviceRemote-change-availability-request-model';
import { ChargeDeviceRemoteChangeAvailabilityResponseModel } from 'src/app/evtech/models/chargeDeviceRemote/chargeDeviceRemote-change-availability-response-model';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { ChargeDeviceRemoteResetModalComponent } from '../chargeDevice-remote-reset-modal/chargeDevice-remote-reset-modal.component';
import { ChargeDeviceStateNotificatonModel } from 'src/app/evtech/models/chargeDevice/charge-device-state-notification-model';
import { ChargeDeviceInstantState, chargeDeviceInstantStateMapping } from 'src/app/evtech/enums/chargeDevice/charge-device-instant-state-enum';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { CompanyFilterModel } from 'src/app/evtech/models/company/company-filter-model';
import { CompanyListItemModel } from 'src/app/evtech/models/company/company-list-item-model';
import { CompanyService } from 'src/app/evtech/services/company/company-service';
import { CompanySelectListModel } from 'src/app/evtech/models/company/company-select-list-model';
import { StationManagmentService } from 'src/app/evtech/services/stationManagment/stationManagment-service';
import { StationManagmentListModel } from 'src/app/evtech/models/stationManagment/stationManagment-list-model';
import { StationManagmentFilterModel } from 'src/app/evtech/models/stationManagment/stationManagment-filter-model';
import { ChargeDeviceChangeConfigurationComponent } from '../chargeDevice-change-configuration/chargeDevice-change-configuration.component';
import { ChargeDeviceTriggerMessageComponent } from '../chargeDevice-trigger-message/chargeDevice-trigger-message.component';

@Component({
  selector: 'app-chargeDevice-remote-managment',
  templateUrl: './chargeDevice-remote-managment.component.html',
  styleUrls: ['./chargeDevice-remote-managment.component.scss']
})
export class ChargeDeviceRemoteManagmentComponent implements OnInit {
  //#region  oturum modelini tutacak değişkenler tanımlanıyor
  admin: AuthenticationModel;
  //#endregion
  idSubscription: Subscription;
  stationId: number;
  //#region  firmalaları getiren servis için modeller tanımlanıyor
  companyList: CompanySelectListModel[] = [];
  companyFilter: CompanyFilterModel = new CompanyFilterModel();
  //#endregion
  stationList: StationManagmentListModel[] = [];
  stationFilter: StationManagmentFilterModel = new StationManagmentFilterModel();
  /// charge device status için değişkenleri ayarlanıyor
  chargeDeviceStatusRequest: ChargeDeviceStatusRequestModel[] = [];
  chargeDeviceStatusResponse: ChargeDeviceStatusResponseModel[] = [];
  ///
  /// Datatable değişkenleri ayarlanıyor
  showingColumnNamesChargeDeviceTable: string[] = ['Id', 'Name', 'StationManagmentName', 'LastInstantStateUpdatedDate', 'InstantState', 'ConnectionState', 'HearthBeatDate', 'Edit'];
  chargeDeviceList: MatTableDataSource<ChargeDeviceListModel> = new MatTableDataSource();
  totalRecordSizeChargeDeviceTable: number;
  @ViewChild(MatPaginator) paginatorChargeDeviceTable: MatPaginator;
  @ViewChild(MatSort) sortChargeDeviceTable: MatSort;
  searchCriterChargeDevice: DatatableRequestWrapper<ChargeDeviceFilterModel> = new DatatableRequestWrapper();
  ///
  chargeDeviceAvailabilityTypeMapping = chargeDeviceAvailabilityTypeMapping;
  chargeDeviceAvailabilityTypes = Object.values(ChargeDeviceAvailabilityType).filter(value => typeof value === 'string');
  chargeDeviceAvailabilityType = ChargeDeviceAvailabilityType;
  chargeDeviceOcppStateMapping = chargeDeviceOcppStateMapping;
  chargeDeviceOcppStates = Object.values(ChargeDeviceOcppState).filter(value => typeof value === 'string');
  chargeDeviceOcppState = ChargeDeviceOcppState;
  chargeDeviceInstantStateMapping = chargeDeviceInstantStateMapping;
  chargeDeviceInstantStates = Object.values(ChargeDeviceInstantState).filter(value => typeof value === 'string');
  chargeDeviceInstantState = ChargeDeviceInstantState;
  //#region  change availability komutu için request model tanımlanıyor
  chargeDeviceRemoteChangeAvailabilityRequestModel: ChargeDeviceRemoteChangeAvailabilityRequestModel = new ChargeDeviceRemoteChangeAvailabilityRequestModel();
  //#endregion
  stationManagmentId: number = 0;
  chargeDeviceRemoteStartTransactionRequestModel: ChargeDeviceRemoteStartTransactionRequestModel = new ChargeDeviceRemoteStartTransactionRequestModel();
  chargeDeviceRemoteStopTransactionRequestModel: ChargeDeviceRemoteStopTransactionRequestModel = new ChargeDeviceRemoteStopTransactionRequestModel();

  constructor(private srvProgressSpinner: ProgressSpinnerService,
    private srvChargeDeviceService: ChargeDeviceService,
    private srvChargeDeviceRemoteManagmentService: ChargeDeviceRemoteManagmentService,
    private srvChargeDeviceRemoteService: ChargeDeviceRemoteService,
    private srvStationManagmentService : StationManagmentService,
    private srvUtils: UtilsService,
    private router: Router,
    private route: ActivatedRoute,
    public dialog: MatDialog,
    public gMaps: GoogleMapsAPIWrapper,
    public chargeDeviceNotificationService: ChargeDeviceStatusNotificationService,
    private srvCompanyService: CompanyService,
    private subheader: SubHeaderService,
    public yesNoDialog: MatDialog,
  ) {
  }

  ngOnInit(): void {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'İstasyon Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'Uzaktan Yönetim', linkPath: null, linkText: '', isActive: false },
    ]);
    this.idSubscription = this.route.params.subscribe(
      params => {
        this.searchCriterChargeDevice.data = new ChargeDeviceFilterModel();
        this.stationId = Number(params['stationManagmentId']);
        //#region  admin oturum bilgileri setleniyor
        this.admin = JSON.parse(localStorage.getItem("auth_token"));
        //#endregion
        this.companyFilter.id = this.admin.companyId;
        this.srvProgressSpinner.show();
        this.chargeDeviceNotificationService.notification$.subscribe((data) => {
          this.chargeDeviceList.data.filter(x => x.guiId == data.guiId).map((item) => {
            item.connectStateInfo = data.connectState;
          });
        });
        this.getCompanies();
      })
  }

  initForm() {
    this.searchCriterChargeDevice.data.companyId = this.companyFilter.id;
    this.startConnection();
    this.addListener();
    this.startConnectionDeviceState();
    this.addListenerDeviceState();
    this.clearParameters();
    this.searchCriterChargeDevice.data.stationManagmentId = this.stationId;
    this.getStations();
    this.getChargeDevices(true);
  }

  companyChange() {
    this.searchCriterChargeDevice.data.companyId = this.companyFilter.id;
    this.startConnection();
    this.addListener();
    this.startConnectionDeviceState();
    this.addListenerDeviceState();
    this.clearParameters();
    this.getStations();
    this.getChargeDevices(true);
  }

  getStations() {
    this.stationList = [];
    this.srvProgressSpinner.show();
    this.srvStationManagmentService.getStationsForSelectList(this.stationFilter).subscribe((response: StationManagmentListModel[]) => {
      this.stationList = response;
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  openConnectorListDialog(deviceId, deviceName, deviceGuiId): void {
   const dialogRef  = this.dialog.open(ConnectorRemoteManagmentPartialComponent, {
      width: '1200px',
      height: '90vh',
      data: { chargeDeviceId: deviceId, deviceName: deviceName, deviceGuiId: deviceGuiId },
    });
    dialogRef.afterClosed().subscribe(result => {});
  }

  //#region  cihaz durumunun anlık gösterilmesi için signalr bağlantısı yapılıyor
  public notificationDeviceState$ = new BehaviorSubject<ChargeDeviceStateNotificatonModel>(null);
  public notificationsDeviceState$ = new BehaviorSubject<ChargeDeviceStateNotificatonModel[]>([]);
  public notificationDeviceStateViewTime$ = new BehaviorSubject<number>(3000);

  public startConnectionDeviceState = () => {
    console.log("hub connection started")
    this.notificationDeviceState$ = new BehaviorSubject<ChargeDeviceStateNotificatonModel>(null);
    this.notificationsDeviceState$ = new BehaviorSubject<ChargeDeviceStateNotificatonModel[]>([]);
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.notificationApiUrl}deviceStateNotification`)
      .build()
    this.hubConnection
      .start()
      .then(() => { console.log("hub connection started") })
      .catch(err => { console.log(err) })
  }

  public addListenerDeviceState = () => {
    this.notificationDeviceState$ = new BehaviorSubject<ChargeDeviceStateNotificatonModel>(null);
    this.notificationsDeviceState$ = new BehaviorSubject<ChargeDeviceStateNotificatonModel[]>([]);
    this.hubConnection.on("deviceStateNotification", data => {
      this.chargeDeviceList.data.filter(x => x.guiId == data.guiId).map(item => {
        item.lastInstantStateUpdatedDate = data.lastUpdateDate;
        item.instantState = this.chargeDeviceInstantStates[data.state - 1];
      });
      this.notificationDeviceState$.next(data);
      this.notificationsDeviceState$.next([data, ...this.notificationsDeviceState$.value])
      this.notificationDeviceStateViewTime$.next(3);
      this.clearNotificationDeviceState(data);
    })
  }

  public clearNotificationDeviceState = (data: ChargeDeviceStateNotificatonModel) => {
    setTimeout(() => {
      const notifyArray: any[] = this.notificationsDeviceState$.getValue();
      notifyArray.forEach((item, index) => {
        if (item === data) { notifyArray.splice(index, 1); }
      });
      this.notificationsDeviceState$.next(notifyArray);
    }, this.notificationsDeviceState$.getValue().length * 2000 > 8000 ? 8000 : this.notificationsDeviceState$.getValue().length * 2000);
  }
  //#endregion

  //#region  cihaz bağlılığının gösterilmesi için signalr bağlantısı yapılıyor
  public notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
  public notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
  public notificationViewTime$ = new BehaviorSubject<number>(3000);

  private hubConnection: signalR.HubConnection;

  public getConnectionId(): string {
    return this.hubConnection.connectionId;
  }

  public startConnection = () => {
    this.notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
    this.notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.notificationApiUrl}chargeDeviceConnectionStateNotification`)
      .build()
    this.hubConnection
      .start()
      .then(() => { console.log("hub connection started") })
      .catch(err => { console.log(err) })
  }

  public addListener = () => {
    this.notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
    this.notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
    this.hubConnection.on("chargeDeviceConnectionStateNotification", data => {
      this.chargeDeviceList.data.filter(x => x.guiId == data.guiId).map(item => {
        item.connectStateInfo = data.connectState;
        item.lastInstantStateUpdatedDate = data.lastUpdateDate;
        if (data.state != null) {
          item.instantState = ChargeDeviceInstantState[this.chargeDeviceInstantStates[data.state - 1]];
        }
      });
      this.notification$.next(data);
      this.notifications$.next([data, ...this.notifications$.value])
      this.notificationViewTime$.next(3);
      this.clearNotification(data);
    })
  }

  public clearNotification = (data: ChargeDeviceNotificationStatusModel) => {
    setTimeout(() => {
      const notifyArray: any[] = this.notifications$.getValue();
      notifyArray.forEach((item, index) => {
        if (item === data) { notifyArray.splice(index, 1); }
      });
      this.notifications$.next(notifyArray);
    }, this.notifications$.getValue().length * 2000 > 8000 ? 8000 : this.notifications$.getValue().length * 2000);
  }
  //#endregion

  openResetDeviceDialog(chargeDeviceGuiId, deviceName): void {
    const dialogRef = this.dialog.open(ChargeDeviceRemoteResetModalComponent, {
      width: '500px',
      data: { chargeDeviceGuiId: chargeDeviceGuiId, deviceName: deviceName },
    });
    dialogRef.afterClosed().subscribe(result => {});
  }

  openCommandDialog(guiId, deviceMarkId): void {
    const dialogRef = this.dialog.open(ChargeDeviceCommandComponent, {
      width: '600px',
      data: { guiId: guiId, deviceMarkId: deviceMarkId },
    });
    dialogRef.afterClosed().subscribe(result => {});
  }

  openChangeConfigurationDialog(guiId): void {
    const dialogRef = this.dialog.open(ChargeDeviceChangeConfigurationComponent, {
      width: '600px',
      data: { guiId: guiId },
    });
    dialogRef.afterClosed().subscribe(result => {});
  }

  openTriggerMessageDialog(guiId): void {
    const dialogRef = this.dialog.open(ChargeDeviceTriggerMessageComponent, {
      width: '600px',
      data: { guiId: guiId, connectorId: null },
    });
    dialogRef.afterClosed().subscribe(result => {});
  }

  openDeviceCommandListDialog(chargeDeviceGuiId): void {
    const dialogRef = this.dialog.open(ChargeDeviceCommandListComponent, {
      width: '1200px',
      height: '90vh',
      data: {
        chargeDeviceGuiId: chargeDeviceGuiId,
        companyId: this.searchCriterChargeDevice.data.companyId
      },
    });
    dialogRef.afterClosed().subscribe(result => {});
  }

  ngOnDestroy() {
    this.hubConnection.stop();
    this.hubConnection = null;
    this.notification$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel>(null);
    this.notifications$ = new BehaviorSubject<ChargeDeviceNotificationStatusModel[]>([]);
    this.notifications$.unsubscribe();
    this.notification$.unsubscribe();
  }

  changeAvailabilityDialog(deviceGuiId: string, availabilityType: ChargeDeviceAvailabilityType) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "Cihazı " + chargeDeviceAvailabilityTypeMapping[this.chargeDeviceAvailabilityType[availabilityType]] + " Yapmak İstediğinize Emin misiniz ?" },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.changeAvailability(deviceGuiId, availabilityType);
      }
    });
  }

  changeAvailability(deviceGuiId: string, availabilityType: ChargeDeviceAvailabilityType) {
    this.chargeDeviceRemoteChangeAvailabilityRequestModel.availabilityType = availabilityType;
    this.chargeDeviceRemoteChangeAvailabilityRequestModel.chargeDeviceGuiId = deviceGuiId;
    this.srvChargeDeviceRemoteService.changeAvailability(this.chargeDeviceRemoteChangeAvailabilityRequestModel).subscribe((response: ChargeDeviceRemoteChangeAvailabilityResponseModel) => {
      this.srvUtils.showActionNotification("Durum Değiştirme Komutu Yollandı", EnumMessageType.Success, 8000);
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorChangeAvailability(error);
    });
  }

  onErrorChangeAvailability(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  getChargeDevices(hasFilter: boolean) {
    return new Promise<void>((resolve, reject) => {
      this.srvProgressSpinner.show();
      this.chargeDeviceList.data = [];
      if (hasFilter) {
        this.paginatorChargeDeviceTable.pageIndex = 0;
      }
      if (this.sortChargeDeviceTable && this.sortChargeDeviceTable.direction != "" && this.sortChargeDeviceTable.active != "") {
        this.searchCriterChargeDevice.orderDirective = this.sortChargeDeviceTable.direction;
        this.searchCriterChargeDevice.orderProperty = this.sortChargeDeviceTable.active;
      } else {
        this.searchCriterChargeDevice.orderDirective = "desc";
        this.searchCriterChargeDevice.orderProperty = "Id";
      }
      if (this.paginatorChargeDeviceTable) {
        this.searchCriterChargeDevice.recordPerPage = this.paginatorChargeDeviceTable.pageSize;
        this.searchCriterChargeDevice.pageNumber = this.paginatorChargeDeviceTable.pageIndex;
      } else {
        this.searchCriterChargeDevice.recordPerPage = 15;
      }
      this.srvChargeDeviceService.list(this.searchCriterChargeDevice).subscribe((response: DatatableResponseWrapper<ChargeDeviceListModel[]>) => {
        this.chargeDeviceList.data = response.data;
        this.totalRecordSizeChargeDeviceTable = response.recordCount;
        resolve();
      }, (error) => {
        this.onErrorChargeDeviceList(error);
        reject();
      });
    });
  }

  ngAfterViewInit(): void {
    this.sortChargeDeviceTable.sortChange.subscribe(
      () => {
        this.paginatorChargeDeviceTable.pageIndex = 0
      }
    );
    merge(this.sortChargeDeviceTable.sortChange, this.paginatorChargeDeviceTable.page)
      .pipe(
        tap(() => {
          this.getChargeDevices(false);
        })
      )
      .subscribe(() => { }, () => null);
  }

  initData(hasFilter: boolean) {
    this.srvProgressSpinner.show();
    this.searchCriterChargeDevice.data.stationManagmentId = null;
    this.searchCriterChargeDevice.data.chargeDevicePowerTypeId = null;
    this.getChargeDevices(hasFilter).then(res => {
      this.srvProgressSpinner.hide();
    }).catch(err => {
      this.srvProgressSpinner.hide();
      this.onErrorChargeDeviceList(err);
    });
  }

  getCompanies() {
    this.srvCompanyService.getCompanyForSelectList(this.companyFilter).subscribe((response: CompanySelectListModel[]) => {
      this.companyList = response;
      this.initForm();
    }, (error) => {});
  }

  getChargeDeviceStatus() {
    return new Promise<void>((resolve, reject) => {
      this.chargeDeviceList.data.forEach(element => {
        this.chargeDeviceStatusRequest.push({ guiId: element.guiId });
      });
      this.srvChargeDeviceRemoteManagmentService.getChargeDeviceStatus(this.chargeDeviceStatusRequest)
        .subscribe((response: ChargeDeviceStatusResponseModel[]) => {
          this.chargeDeviceStatusResponse = response;
          this.chargeDeviceStatusResponse.forEach(element => {
            this.chargeDeviceList.data.filter(x => x.guiId == element.chargeDeviceGuiId).forEach(chargeDevice => {
              chargeDevice.connectStateInfo = element.connectState;
            });
          });
          resolve();
        }, (error) => {
          this.onErrorChargeDeviceStatusList(error);
          reject();
        });
    });
  }

  onErrorChargeDeviceList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  onErrorChargeDeviceStatusList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  clearParameters() {
    this.searchCriterChargeDevice.data.stationManagmentId = null;
    this.searchCriterChargeDevice.data.chargeDevicePowerTypeId = null;
    this.searchCriterChargeDevice.data.name = null;
    this.searchCriterChargeDevice.data.identifierContain = null;
    this.searchCriterChargeDevice.data.deviceState = null;
    this.searchCriterChargeDevice.data.instantState = null;
    this.searchCriterChargeDevice.data.connectionState = null;
  }

  clearParametersAndGetChargeDevices() {
    this.clearParameters();
    this.getChargeDevices(true);
  }
}
