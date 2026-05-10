// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\chargeManagment\charge-detail-partial\charge-detail-partial.component.ts
// Pattern: Detail/Partial Dialog Component — MAT_DIALOG_DATA ile data alır, API'den detay çeker,
// widget card'lar (loadedKw, calculatedPrice, processingTime), enum-based CSS class, MatDialogRef

import { Component, EventEmitter, Inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatDialogRef } from "@angular/material/dialog";
import { Router } from '@angular/router';
import { merge, tap } from 'rxjs';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';
import { environment } from 'src/environments/environment';
import * as signalR from "@microsoft/signalr";
import { ChargeDetailRequestModel } from 'src/app/evtech/models/charge/charge-detail-request-model';
import { ChargeDetailModel } from 'src/app/evtech/models/charge/charge-detail-model';
import { ChargeManagmentService } from 'src/app/evtech/services/chargeManagment/chargeManagment-service';
import { ChargeDetailResponseModel } from 'src/app/evtech/models/charge/charge-detail-response-model';
import { ChargeState, chargeStateMapping } from 'src/app/evtech/enums/charge/charge-state-enum';

@Component({
    selector: 'app-charge-detail-partial',
    templateUrl: './charge-detail-partial.component.html',
    styleUrls: ['./charge-detail-partial.component.scss']
})
export class ChargeDetailPartialComponent implements OnInit {

    //#region  widget renk/css değişkenleri
    loadedKwBaseColor = 'primary';
    loadedKwTextInverseCSSClass;
    loadedKwIconColor = 'success';
    loadedKwWidgetHeight = '150px';
    loadedKwSvgCSSClass;
    loadedKwCssClass = "gutter-b";
    calculatedPriceBaseColor = 'primary';
    calculatedPriceTextInverseCSSClass;
    calculatedPriceIconColor = 'success';
    calculatedPriceWidgetHeight = '150px';
    calculatedPriceSvgCSSClass;
    calculatedPriceCssClass = "gutter-b";
    //#endregion

    //#region şarj detayı model tanımlanıyor
    chargeDetailRequestModel: ChargeDetailRequestModel = new ChargeDetailRequestModel();
    chargeDetail: ChargeDetailModel = new ChargeDetailModel();
    //#endregion

    chargeStateMapping = chargeStateMapping;
    chargeStates = Object.values(ChargeState).filter(value => typeof value === 'string');
    chargeState = ChargeState;

    constructor(private srvProgressSpinner: ProgressSpinnerService,
        private srvChargeManagmentService: ChargeManagmentService,
        private srvUtils: UtilsService,
        private router: Router,
        public dialogRef: MatDialogRef<ChargeDetailPartialComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        public yesNoDialog: MatDialog,
    ) {
        // Dialog'a gelen data'dan request model oluşturuluyor
        this.chargeDetailRequestModel.id = data.id;
        this.chargeDetailRequestModel.guiId = data.guiId;
        this.chargeDetailRequestModel.stationId = data.stationId;
    }

    ngOnInit(): void {
        // Widget CSS class'ları dinamik olarak oluşturuluyor
        this.loadedKwCssClass = `bg-${this.loadedKwBaseColor} ${this.loadedKwCssClass}`;
        this.loadedKwTextInverseCSSClass = `text-inverse-${this.loadedKwBaseColor}`;
        this.calculatedPriceCssClass = `bg-${this.calculatedPriceBaseColor} ${this.calculatedPriceCssClass}`;
        this.calculatedPriceTextInverseCSSClass = `text-inverse-${this.calculatedPriceBaseColor}`;
        this.getChargeDetail();
    }

    ngAfterViewInit(): void {}
    ngOnDestroy() {}

    getChargeDetail() {
        this.srvProgressSpinner.show();
        this.srvChargeManagmentService.getChargeDetail(this.chargeDetailRequestModel).subscribe((response: ChargeDetailResponseModel) => {
            this.chargeDetail = response.chargeDetail;
            this.srvProgressSpinner.hide();
        }, (error) => {
            this.srvProgressSpinner.hide();
            this.onErrorGetChargeDetail(error);
        });
    }

    onErrorGetChargeDetail(error: any) {
        let errorData = this.srvUtils.getServerErrorRequest(error);
        this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
    }
}
