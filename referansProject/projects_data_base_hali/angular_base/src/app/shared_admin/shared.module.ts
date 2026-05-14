// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\shared.module.ts
import { registerLocaleData, CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule, LOCALE_ID } from '@angular/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { MAT_DATE_LOCALE, MAT_DATE_FORMATS, DateAdapter } from '@angular/material/core';
import { MOMENT_DATE_FORMATS } from '../core/adapters/MOMENT_DATE_FORMATS';
import { GeneralMaterialModule } from './general-material.module';
import localeTr from '@angular/common/locales/tr';
import { ActionNotificationComponent } from './partials/dialogs/action-natification/action-notification.component';
import { EvetHayirDialogComponent } from './partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { GenelSilDialogComponent } from './partials/dialogs/genel-sil-dialog/genel-sil-dialog.component';
import { YardimDialogComponent } from './partials/dialogs/yardim-dialog/yardim-dialog.component';
import { httpEventInterceptorProvider } from './utils/interseptors/http-event-interseptor';
import { ActivePassiveCssPipe } from './utils/pipes/general-pipes/active-passive-css.pipe';
import { DateTimeDirective } from '../core/directives/date-time.directive';
import { MonthPipe } from './utils/pipes/general-pipes/month-pipe';
import { ChargeApplicationTypeCssPipe } from './utils/pipes/domain-pipes/charge-application-type-css.pipe';
import { MatDateModule } from '../core/date-core/mat-date.module';
import { ChargeDeviceInstantStateCssPipe } from './utils/pipes/domain-pipes/charge-device-instant-state-css.pipe';
import { ChargeDeviceInstantStatePipe } from './utils/pipes/domain-pipes/charge-device-instant-state.pipe';
import { ChargeDeviceOcppStateCssPipe } from './utils/pipes/domain-pipes/charge-device-ocpp-state-css.pipe';
import { ChargeDeviceOcppStatePipe } from './utils/pipes/domain-pipes/charge-device-ocpp-state.pipe';
import { ChargeStateCssPipe } from './utils/pipes/domain-pipes/charge-state-css.pipe';
import { ChargeDeviceReservationStateCssPipe } from './utils/pipes/domain-pipes/chargeDevice-reservation-state-css-pipe';
import { ChargeDeviceStateCssPipe } from './utils/pipes/domain-pipes/chargeDevice-state-css.pipe';
import { ChargeDevicePowerTypeCssPipe } from './utils/pipes/domain-pipes/chargeDevice-type-css.pipe';
import { ConnectionStateCssPipe } from './utils/pipes/domain-pipes/connection-state-css.pipe';
import { ConnectionStateNotifyCssPipe } from './utils/pipes/domain-pipes/connection-state-notify-css.pipe';
import { ConnectionStatePipe } from './utils/pipes/domain-pipes/connection-state.pipe';
import { ChargeDeviceConnectorInstantStateCssPipe } from './utils/pipes/domain-pipes/connector-instant-state-css.pipe';
import { ChargeDeviceConnectorInstantStatePipe } from './utils/pipes/domain-pipes/connector-instant-state.pipe';
import { ChargeDeviceConnectorOcppStateCssPipe } from './utils/pipes/domain-pipes/connector-ocpp-state-css.pipe';
import { ChargeDeviceConnectorOcppStatePipe } from './utils/pipes/domain-pipes/connector-ocpp-state.pipe';
import { ConnectorStateCssPipe } from './utils/pipes/domain-pipes/connector-state-css.pipe';
import { ConnectorStatePipe } from './utils/pipes/domain-pipes/connector-state.pipe';
import { GoogleMapMarkerCssPipe } from './utils/pipes/domain-pipes/google-map-marker-css.pipe';
import { ImageSrcPipe } from './utils/pipes/domain-pipes/image-src.pipe';
import { OcppMessageTypeCssPipe } from './utils/pipes/domain-pipes/ocppMessage-type-css.pipe';
import { OtherSupportListCssPipe } from './utils/pipes/domain-pipes/other-support-list-css.pipe';
import { PanelAdminCompanySelectDisableStatePipe } from './utils/pipes/domain-pipes/panel-admin-company-select-css.pipe';
import { PanelAdminManagmentTypePipe } from './utils/pipes/domain-pipes/panel-admin-managment-type.pipe';
import { PaymentStatusCssPipe } from './utils/pipes/domain-pipes/payment-status-css.pipe';
import { StationTypeCssPipe } from './utils/pipes/domain-pipes/station-type-css.pipe';
import { SupportStateCssPipe } from './utils/pipes/domain-pipes/support-state-css.pipe';
import { SupportTypeCssPipe } from './utils/pipes/domain-pipes/support-type-css.pipe';
import { UserPaymentBankTypePipe } from './utils/pipes/domain-pipes/userPayment-bank-type.pipe';
import { UserPaymentMethodCssPipe } from './utils/pipes/domain-pipes/userPayment-method-css.pipe';
import { UserPaymentMethodPipe } from './utils/pipes/domain-pipes/userPayment-method.pipe';
import { VersionPuplicationCssPipe } from './utils/pipes/domain-pipes/version-puplication-css.pipe';
import { WalletProcessStateCssPipe } from './utils/pipes/domain-pipes/wallet-process-state-css.pipe';
import { WalletPullMoneyStateCssPipe } from './utils/pipes/domain-pipes/wallet-pull-money-state-css.pipe';
import { TrueFalseCssPipe } from './utils/pipes/general-pipes/true-false-css.pipe';

registerLocaleData(localeTr, 'tr');

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    GeneralMaterialModule,
    MatDateModule
  ],
  declarations: [
    GenelSilDialogComponent,
    ActionNotificationComponent,
    EvetHayirDialogComponent,
    YardimDialogComponent,
    ImageSrcPipe,
    ActivePassiveCssPipe,
    TrueFalseCssPipe,
    StationTypeCssPipe,
    ConnectionStatePipe,
    ConnectionStateCssPipe,
    ChargeDevicePowerTypeCssPipe,
    ChargeDeviceReservationStateCssPipe,
    OcppMessageTypeCssPipe,
    VersionPuplicationCssPipe,
    PaymentStatusCssPipe,
    ConnectionStateNotifyCssPipe,
    DateTimeDirective,
    WalletPullMoneyStateCssPipe,
    ChargeDeviceStateCssPipe,
    ConnectorStateCssPipe,
    ConnectorStatePipe,
    GoogleMapMarkerCssPipe,
    PanelAdminCompanySelectDisableStatePipe,
    PanelAdminManagmentTypePipe,
    SupportStateCssPipe,
    OtherSupportListCssPipe,
    ChargeDeviceOcppStatePipe,
    ChargeDeviceOcppStateCssPipe,
    ChargeDeviceInstantStatePipe,
    ChargeDeviceInstantStateCssPipe,
    ChargeDeviceConnectorOcppStatePipe,
    ChargeDeviceConnectorOcppStateCssPipe,
    ChargeDeviceConnectorInstantStatePipe,
    ChargeDeviceConnectorInstantStateCssPipe,
    UserPaymentMethodPipe,
    UserPaymentMethodCssPipe,
    ChargeStateCssPipe,
    ChargeApplicationTypeCssPipe,
    SupportTypeCssPipe,
    UserPaymentBankTypePipe,
    WalletProcessStateCssPipe,
    MonthPipe
  ],
  exports: [
    CommonModule,
    HttpClientModule,
    GeneralMaterialModule,
    ImageSrcPipe,
    ActivePassiveCssPipe,
    TrueFalseCssPipe,
    ConnectionStatePipe,
    ConnectionStateCssPipe,
    StationTypeCssPipe,
    ChargeDevicePowerTypeCssPipe,
    ChargeDeviceReservationStateCssPipe,
    OcppMessageTypeCssPipe,
    VersionPuplicationCssPipe,
    PaymentStatusCssPipe,
    ConnectionStateNotifyCssPipe,
    DateTimeDirective,
    WalletPullMoneyStateCssPipe,
    ChargeDeviceStateCssPipe,
    ConnectorStateCssPipe,
    ConnectorStatePipe,
    GoogleMapMarkerCssPipe,
    PanelAdminCompanySelectDisableStatePipe,
    PanelAdminManagmentTypePipe,
    SupportStateCssPipe,
    OtherSupportListCssPipe,
    ChargeDeviceOcppStatePipe,
    ChargeDeviceOcppStateCssPipe,
    ChargeDeviceInstantStatePipe,
    ChargeDeviceInstantStateCssPipe,
    ChargeDeviceConnectorOcppStatePipe,
    ChargeDeviceConnectorOcppStateCssPipe,
    ChargeDeviceConnectorInstantStatePipe,
    ChargeDeviceConnectorInstantStateCssPipe,
    UserPaymentMethodPipe,
    UserPaymentMethodCssPipe,
    ChargeStateCssPipe,
    ChargeApplicationTypeCssPipe,
    SupportTypeCssPipe,
    UserPaymentBankTypePipe,
    WalletProcessStateCssPipe,
    MonthPipe
  ],
  providers: [
    httpEventInterceptorProvider
  ]
})
export class SharedModule { }
