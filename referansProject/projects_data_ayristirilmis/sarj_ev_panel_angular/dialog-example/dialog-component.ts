// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\chargeDevice\chargeDevice-change-state-dialog\chargeDevice-change-state-dialog.component.ts
// Pattern: Dialog Component — MAT_DIALOG_DATA ile data alır, MatDialogRef ile kapatır, iç içe EvetHayir dialog açar

import { GoogleMapsAPIWrapper, Marker } from '@agm/core';
import { Component, EventEmitter, Inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import {  MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDialogRef } from "@angular/material/dialog";
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { merge, tap,Subject, BehaviorSubject } from 'rxjs';
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
import { ChargeDeviceCommandManagmentService } from 'src/app/evtech/services/chargeDeviceRemoteManagment/chargeDevice-commandManagment-service';
import { ChargeDeviceCommandManagmentResponseModel } from 'src/app/evtech/models/chargeDeviceCommandManagment/chargeDevice-commandManagment-response-model';
import { ChargeDeviceCommandListModel } from 'src/app/evtech/models/chargeDeviceCommandManagment/chargeDevice-command-list-model';
import { ChargeDeviceCommandFilterModel } from 'src/app/evtech/models/chargeDeviceCommandManagment/chargeDevice-command-filter-model';
import { OcppMessageType, ocppMessageTypeMapping } from 'src/app/evtech/enums/chargeDevice/chargeDevice-type-enum';
import { ConnectorPriceRequestInfoModel } from 'src/app/evtech/models/connector/connector-price-request-info-model';
import { ConnectorService } from 'src/app/evtech/services/connector/connector-service';
import { ConnectorChangeStateRequestModel } from 'src/app/evtech/models/connector/connector-change-state-request-model';
import { ChargeDeviceChangeStateRequestModel } from 'src/app/evtech/models/chargeDevice/chargeDevice-change-state-request-model';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';

@Component({
  selector: 'app-chargeDevice-change-state-dialog',
  templateUrl: './chargeDevice-change-state-dialog.component.html',
  styleUrls: ['./chargeDevice-change-state-dialog.component.scss']
})
export class ChargeDeviceChangeStateDialogComponent implements OnInit {
//#region  soket durumunu değiştirmek için request model tanımlanıyor
connectorChangeStateRequestModel : ConnectorChangeStateRequestModel = new ConnectorChangeStateRequestModel();
//#endregion
//#region  cihaz durumunu değiştirmek için request model tanımlanıyor
chargeDeviceChangeStateRequestModel : ChargeDeviceChangeStateRequestModel = new ChargeDeviceChangeStateRequestModel();
//#endregion
constructor(private srvProgressSpinner: ProgressSpinnerService,
  private srvConnectorService: ConnectorService,
  private srvChargeDevice: ChargeDeviceService,
  private srvUtils: UtilsService,
  private router: Router,
  public dialogRef: MatDialogRef<ChargeDeviceChangeStateDialogComponent>,
  public yesNoDialog: MatDialog,
  @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.connectorChangeStateRequestModel.connectorIdList = [];
    this.connectorChangeStateRequestModel.connectorIdList = data.idList;
    this.connectorChangeStateRequestModel.state = data.state;

    this.chargeDeviceChangeStateRequestModel.id = data.id;
    this.chargeDeviceChangeStateRequestModel.state = data.state;
  }
  changeActiveStateDialog(id,state){
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '250px',
      data: { title: "", description: "Durumunu Güncellemek İstediğinize Emin misiniz ?" },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.srvProgressSpinner.show();
        this.changeActiveState(id,state);
      } else {
      }
    });
  }
  changeActiveState(id,state) {
   this.srvProgressSpinner.show();
   this.chargeDeviceChangeStateRequestModel.id = id;
   this.chargeDeviceChangeStateRequestModel.state = state;
   this.srvChargeDevice.changeState(this.chargeDeviceChangeStateRequestModel).subscribe((response: any) => {
       this.srvUtils.showActionNotification("Başarıyla Güncellendi",EnumMessageType.Success, 2000);
       this.srvProgressSpinner.hide();
     }, (error) => {
       this.srvProgressSpinner.hide();
       this.onErrorChangePrice(error);
   });
  }
  onErrorChangePrice(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }
ngOnInit(): void {

}

ngAfterViewInit(): void {

}


}
