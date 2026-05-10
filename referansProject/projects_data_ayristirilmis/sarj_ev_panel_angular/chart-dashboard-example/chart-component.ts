// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\home\dashboardStats\dashboard-monthly-charge-process\dashboard-monthly-charge-process.component.ts
// Pattern: Chart/Dashboard Component — chart.js/auto kullanımı, canvas ile bar chart, API'den veri çekip chart update

import { Component, Input, OnInit } from '@angular/core';
import Chart from 'chart.js/auto';
import moment from 'moment';
import { title } from 'process';
import { ChargePrcessDashboardFilterModel } from 'src/app/evtech/models/dashboard/charge-process-dashboard-filter-model';
import { MonthlyChargeProcessDashboardItemModel } from 'src/app/evtech/models/dashboard/monthly-charge-process-dashboard-item-model';
import { MonthlyChargeProcessDashboardModel } from 'src/app/evtech/models/dashboard/monthly-charge-process-dashboard-model';
import { DashboardService } from 'src/app/evtech/services/dashboard/dashboard-service';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { LayoutService } from 'src/app/shared_admin/partials/layout/layout.service';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';

@Component({
  selector: 'dashboard-monthly-charge-process',
  templateUrl: './dashboard-monthly-charge-process.component.html',
  styleUrls: ['./dashboard-monthly-charge-process.component.scss']
})
export class DashboardMonthlyChargeProcessComponent implements OnInit {

  public monthlyChargeProcessChart: Chart;
  createMonthlyChargeProcessChart() {
    this.monthlyChargeProcessChart = new Chart("MonthlyChargeProcessChart", {
      type: 'bar',
      data: {
        labels: this.categories,
        datasets: [
          {
            label: "AC",
            data: this.dataAc,
            backgroundColor: 'blue',
          },
          {
            label: "DC",
            data: this.dataDc,
            backgroundColor: 'limegreen',
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
            ticks: {},
            title: {
              color: 'black',
              display: true,
              text: 'Kw'
            },
          },
          x: {
            title: {
              color: 'black',
              display: true,
              text: 'TARİH',
            },
          },
        }
      }
    });
  }
  //#region  son 6 aylık şarj işlem bilgilerini gösteren chart için değişkenler tanımlanıyor
  lastMonthLoadedKw: number = 0;
  dataAc = [];
  dataDc = [];
  categories = [];
  cssClass: string = "gutter-b";
  //#endregion
  //#region şarj işlem istek modeli tanımlanıyor
  chargePrcessDashboardFilter: ChargePrcessDashboardFilterModel = new ChargePrcessDashboardFilterModel();
  //#endregion
  //#region aylık şarj işlemlerini tutan model tanımlanıyor
  monthlyChargeProcessWithAc: MonthlyChargeProcessDashboardItemModel[] = [];
  monthlyChargeProcessWithDc: MonthlyChargeProcessDashboardItemModel[] = [];
  //#endregion
  @Input() companyId: number;

  constructor(private subheader: SubHeaderService,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvDashboard: DashboardService,
    private srvUtils: UtilsService,
    private layout: LayoutService
  ) {
    this.categories = this.previousMonths(6);
  }

  initChartData() {
    this.categories = this.previousMonths(6);
  }

  ngOnInit() {
    this.createMonthlyChargeProcessChart();
  }

  //#region  aylara göre şarj işlem verileri getiriliyor
  getMonthlyChargeProcess() {
    this.chargePrcessDashboardFilter.companyId = this.companyId;
    this.chargePrcessDashboardFilter.lastMonth = 6;
    this.srvProgressSpinner.show();
    this.srvDashboard.getMonthlyChargeProcess(this.chargePrcessDashboardFilter).subscribe((response: MonthlyChargeProcessDashboardModel) => {
      this.monthlyChargeProcessWithAc = response.chargeWithAc;
      this.monthlyChargeProcessWithDc = response.chargeWithDc;
      //#region  Ac cihazlar için şarj işlem verileri setleniyor
      this.dataAc = [];
      this.categories.map(date => {
        if (this.monthlyChargeProcessWithAc.filter(x => x.dateText == date).length > 0) {
          this.monthlyChargeProcessWithAc.filter(x => x.dateText == date).map(item => {
            this.dataAc.push(item.totalLoadedKw);
          });
        } else {
          this.dataAc.push(0);
        }
      });
      if (this.dataAc.length > 0) {
        this.lastMonthLoadedKw = this.srvUtils.roundAccurately(this.dataAc[this.dataAc.length - 1], 2);
      }
      //#endregion
      //#region  Dc cihazlar için şarj işlem verileri setleniyor
      this.dataDc = [];
      this.categories.map(date => {
        if (this.monthlyChargeProcessWithDc.filter(x => x.dateText == date).length > 0) {
          this.monthlyChargeProcessWithDc.filter(x => x.dateText == date).map(item => {
            this.dataDc.push(item.totalLoadedKw);
          });
        } else {
          this.dataDc.push(0);
        }
      });
      if (this.dataDc.length > 0) {
        this.lastMonthLoadedKw = this.lastMonthLoadedKw + this.srvUtils.roundAccurately(this.dataDc[this.dataDc.length - 1], 2);
      }
      this.lastMonthLoadedKw = this.srvUtils.roundAccurately(this.lastMonthLoadedKw, 2);
      //#endregion
      //#region şarj işlemi chart verileri setleniyor
      this.monthlyChargeProcessChart.data.datasets[0].data = this.dataAc;
      this.monthlyChargeProcessChart.data.datasets[1].data = this.dataDc;
      this.monthlyChargeProcessChart.update();
      //#endregion
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorMonthlyChargeProcess(error);
    });
  }

  onErrorMonthlyChargeProcess(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }
  //#endregion
  //#region  belirtilen rakama göre önce ki ayları getirir
  previousMonths(last) {
    let months = [];
    for (var n = last - 1; n >= 0; n--)
      months.push(moment().subtract(n, 'months').format('M-YYYY'));
    return (months)
  };
  //#endregion

}
