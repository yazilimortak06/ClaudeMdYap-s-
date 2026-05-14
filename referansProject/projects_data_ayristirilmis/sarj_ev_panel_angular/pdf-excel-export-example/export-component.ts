// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\payments\generalPayment\generalPayment-report.component.ts
// Pattern: Report/Export Component (Chart.js + multi-tab child components) — chart.js ile line chart, @ViewChild ile child component erişimi
// NOT: Projede doğrudan jsPDF/xlsx import'u yoktur. Export işlemleri chart.js üzerinden veya backend tarafında yapılır.
// Bu component: rapor sayfası pattern'i — chart + tablo + child component @ViewChild koordinasyonu

import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { FileDropZoneData } from 'src/app/core/external-components/models/file-dropzone-model';
import { PixdinnDropzoneComponent } from 'src/app/core/external-components/pixdinn-dropzone/pixdinn-dropzone.component';
import { environment } from "src/environments/environment";
import { MatSelect } from '@angular/material/select';
import { GoogleMapsAPIWrapper, MapsAPILoader, MouseEvent } from '@agm/core';
import { ApexAxisChartSeries, ApexChart, ApexDataLabels, ApexGrid, ApexLegend, ApexStroke, ApexTitleSubtitle, ApexXAxis, ApexYAxis, ChartComponent } from 'ng-apexcharts';
import { PaymentService } from 'src/app/evtech/services/payment/payment-service';
import { UserPaymentListItemModel } from 'src/app/evtech/models/payments/userPayment-list-item-model';
import { UserPaymentFilterModel } from 'src/app/evtech/models/payments/userPayment-filter-model';
import { PaymentStatus, paymentStatusMapping } from 'src/app/evtech/enums/payments/userPayment-status';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { UserPaymentDataTableFilterModel } from 'src/app/evtech/models/payments/userPayment-datatable-filter-model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';
import { UserPaymentRefundRequestModel } from 'src/app/evtech/models/payments/userPayment-refund-request-model';
import { UserPaymentRefundResponseModel } from 'src/app/evtech/models/payments/userPayment-refund-response-model';
import { merge, tap } from 'rxjs';
import { UserPaymentYearRangeModel } from 'src/app/evtech/models/payments/userPayment-year-range-model';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import Chart from 'chart.js/auto';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { CompanySelectListModel } from 'src/app/evtech/models/company/company-select-list-model';
import { CompanyFilterModel } from 'src/app/evtech/models/company/company-filter-model';
import { CompanyService } from 'src/app/evtech/services/company/company-service';
import { ChargePaymentComponent } from '../chargePayment/charge-payment.component';
import { WalletPaymentComponent } from '../walletPayment/wallet-payment.component';
import { UserPaymentYearRangeRequestModel } from 'src/app/evtech/models/payments/userPayment-year-range-request-model';
import { PanelAdminUserType } from 'src/app/evtech/enums/authentication/panel-admin-user-type-enum';

export const MY_FORMATS = {
  display: {
    dateInput: 'DD/MM/YYYY',
  },
};

@Component({
  selector: 'app-generalPayment-report',
  templateUrl: './generalPayment-report.component.html',
  styleUrls: ['./generalPayment-report.component.scss'],
  providers: [
    { provide: DateAdapter, useClass: MomentDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})
export class GeneralPaymentReportComponent implements OnInit {
  // Multi-tab child bileşenler @ViewChild ile erişiliyor
  @ViewChild(ChargePaymentComponent) childChargePayment: ChargePaymentComponent;
  @ViewChild(WalletPaymentComponent) childWalletPayment: WalletPaymentComponent;

  public paymentChart: Chart;

  createPaymentChart() {
    this.paymentChart = new Chart("PaymentChart", {
      type: 'line',
      data: {
        labels: this.chartCategories,
        datasets: [
          {
            label: "Ödeme",
            data: this.paymentData,
            backgroundColor: 'green',
          },
          {
            label: "İade",
            data: this.refundedData,
            backgroundColor: 'red',
          },
        ],
      },
      options: {
        plugins: {
          tooltip: {
            mode: 'index',
            intersect: false,
          }
        },
        aspectRatio: 2.5,
        scales: {
          y: {
            min: 0,
            title: {
              color: 'black',
              display: true,
              text: 'Tutar (₺)'
            },
          },
          x: {
            title: {
              color: 'black',
              display: true,
              text: 'AY',
            },
          },
        }
      }
    });
  }

  //#region  oturum modelini tutacak değişkenler tanımlanıyor
  admin: AuthenticationModel;
  //#endregion
  //#region  firmalaları getiren servis için modeller tanımlanıyor
  companyList: CompanySelectListModel[] = [];
  companyFilter: CompanyFilterModel = new CompanyFilterModel();
  //#endregion
  //#region  payment status enum
  paymentStatusMapping = paymentStatusMapping;
  paymentStatuss = Object.values(PaymentStatus).filter(value => typeof value === 'string');
  paymentStatus = PaymentStatus;
  //#endregion
  panelAdminUserType = PanelAdminUserType;
  userPaymentYearRangeRequestModel: UserPaymentYearRangeRequestModel = new UserPaymentYearRangeRequestModel();
  year: number;
  yearList: UserPaymentYearRangeModel
  refundUserPaymentRequest: UserPaymentRefundRequestModel = new UserPaymentRefundRequestModel();
  userPaymentList: UserPaymentListItemModel[] = [];
  chartCategories: string[] = [];
  months: number[] = [];
  paymentData: number[] = [];
  refundedData: number[] = [];
  paidPrice = 0;
  totalPaidPrice = 0;
  totalRefundPrice = 0;
  userPaymentFilter: UserPaymentFilterModel = new UserPaymentFilterModel();

  constructor(
    private srvPayment: PaymentService,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvUtils: UtilsService,
    private srvCompanyService: CompanyService,
    private subheader: SubHeaderService
  ) {
    this.yearList = new UserPaymentYearRangeModel();
    this.yearList.years = [];
    this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    this.chartCategories = [
      "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
      "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
    ];
  }

  ngOnInit(): void {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'Ödeme Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'Kasa', linkPath: null, linkText: '', isActive: false },
    ]);
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
    this.companyFilter.id = this.admin.companyId;
    this.createPaymentChart();
    this.getCompanies();
  }

  getCompanies() {
    this.srvCompanyService.getCompanyForSelectList(this.companyFilter).subscribe((response: CompanySelectListModel[]) => {
      this.companyList = response;
      this.initForm();
      this.childChargePayment.initForm();
      this.childWalletPayment.initForm();
    }, (error) => {});
  }

  initForm() {
    this.userPaymentYearRangeRequestModel.companyId = this.companyFilter.id;
    this.userPaymentFilter.companyId = this.companyFilter.id;
    this.getUserPayments();
    this.initReportPage();
  }

  initReportPage() {
    this.srvPayment.getUserPaymentYearRange(this.userPaymentYearRangeRequestModel).subscribe((response: UserPaymentYearRangeModel) => {
      this.yearList = response;
      var yearListCount = this.yearList.years.length;
      if (this.yearList.years.length > 0) {
        this.year = this.yearList.years[yearListCount - 1];
      }
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorUserPaymentYearRange(error);
    });
  }

  changeCompanySelect() {
    this.initForm();
    this.childChargePayment.companyId = this.companyFilter.id;
    this.childWalletPayment.companyId = this.companyFilter.id;
    this.childChargePayment.initForm();
    this.childWalletPayment.initForm();
  }

  ngAfterViewInit(): void {}

  getUserPayments() {
    this.userPaymentList = [];
    this.paymentData = [];
    this.refundedData = [];
    this.totalPaidPrice = 0;
    this.totalRefundPrice = 0;
    this.userPaymentFilter.endDate = null;
    this.userPaymentFilter.startedDate = null;
    this.userPaymentFilter.year = null;
    this.userPaymentFilter.userId = null;
    this.userPaymentFilter.paymentStatus = null;
    this.srvProgressSpinner.show();
    this.srvPayment.getUserPayments(this.userPaymentFilter).subscribe((response: UserPaymentListItemModel[]) => {
      this.userPaymentList = response;
      this.months.map((item) => {
        var paidPrice = 0;
        var refundPrice = 0;
        this.userPaymentList.filter(x => x.monthNumber == item && x.paymentStatus).map((item) => {
          paidPrice = this.roundAccurately(paidPrice + item.paidPrice, 2);
          refundPrice = this.roundAccurately(refundPrice + item.refundedPrice, 2);
          if (item.paymentStatus == PaymentStatus.SUCCESSFUL) {
            this.totalPaidPrice = this.roundAccurately(this.totalPaidPrice + item.paidPrice, 2);
            this.totalRefundPrice = this.roundAccurately(this.totalRefundPrice + item.refundedPrice, 2);
          }
        });
        this.paymentData.push(paidPrice);
        this.refundedData.push(refundPrice);
      });
      this.paymentChart.data.datasets[0].data = this.paymentData;
      this.paymentChart.data.datasets[1].data = this.refundedData;
      this.paymentChart.update();
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorUserPaymentList(error);
    });
  }

  onErrorUserPaymentList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  onErrorUserPaymentYearRange(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  refundUserPayment(userId, userPaymentId) {
    this.srvProgressSpinner.show();
    this.refundUserPaymentRequest.userPaymentId = userPaymentId;
    this.srvPayment.refundPayment(this.refundUserPaymentRequest).subscribe((response: UserPaymentRefundResponseModel) => {
      if (response.state) {
        this.userPaymentList.filter(x => x.id == userPaymentId).map(item => {
          item.paymentStatus = PaymentStatus.REFUNDED;
        });
        this.srvUtils.showActionNotification("Başarıyla İade Edildi", EnumMessageType.Success, 4000);
      } else {
        this.srvUtils.showActionNotification("İade Edilemedi", EnumMessageType.Error, 4000);
      }
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorUserPaymentList(error);
    });
  }

  roundAccurately(number, decimalPlaces) {
    return Number(Math.round(Number(number + "e" + decimalPlaces)) + "e-" + decimalPlaces);
  }
}
