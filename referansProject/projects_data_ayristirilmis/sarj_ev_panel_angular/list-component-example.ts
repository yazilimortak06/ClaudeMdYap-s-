// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\stations\station-list\station-list.component.ts

import { GoogleMapsAPIWrapper, Marker } from '@agm/core';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSelect } from '@angular/material/select';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { merge, tap, Subject, Subscription, first } from 'rxjs';
import { StationManagmentType, stationManagmentTypeMapping } from 'src/app/evtech/enums/stationManagment/stationManagment-type';
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { CompanyFilterModel } from 'src/app/evtech/models/company/company-filter-model';
import { CompanyListItemModel } from 'src/app/evtech/models/company/company-list-item-model';
import { CompanySelectListModel } from 'src/app/evtech/models/company/company-select-list-model';
import { ParameterGroupModel } from 'src/app/evtech/models/parameterGroup/parameterGroup-model';
import { StationRemoveRequestModel } from 'src/app/evtech/models/stationManagment/station-remove-request-model';
import { StationManagmentFilterModel } from 'src/app/evtech/models/stationManagment/stationManagment-filter-model';
import { StationManagmentIsActiveUpdateModel } from 'src/app/evtech/models/stationManagment/stationManagment-isActive-update-model';
import { StationManagmentListModel } from 'src/app/evtech/models/stationManagment/stationManagment-list-model';
import { AuthenticationService } from 'src/app/shared_admin/auth/authentication-service';
import { CompanyService } from 'src/app/evtech/services/company/company-service';
import { PanelAdminTypeService } from 'src/app/evtech/services/panelAdminType/panelAdminType-service';
import { StationManagmentService } from 'src/app/evtech/services/stationManagment/stationManagment-service';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';

@Component({
  selector: 'app-station-list',
  templateUrl: './station-list.component.html',
  styleUrls: ['./station-list.component.scss']
})
export class StationListComponent implements OnInit {
  //#region  oturum modelini tutacak değişkenler tanımlanıyor
  admin: AuthenticationModel;
  //#endregion
  //#region  firmalaları getiren servis için modeller tanımlanıyor
  companyList: CompanySelectListModel[] = [];
  companyFilter: CompanyFilterModel = new CompanyFilterModel();
  //#endregion
  //#region  istasyon kaldırmak için modeller tanımlanıyor
  stationRemoveRequestModel: StationRemoveRequestModel = new StationRemoveRequestModel();
  //#endregion
  /// Datatable değişkenleri ayarlanıyor
  showingColumnNamesStationManagmentTable: string[] = ['Id', 'Name', 'Description', 'TotalLoadedKw', 'TotalPaidPrice', 'IsActive', 'Edit'];
  stationManagmentList: MatTableDataSource<StationManagmentListModel> = new MatTableDataSource();
  totalRecordSizeStationManagmentTable: number;
  @ViewChild(MatPaginator) paginatorStationManagmentTable: MatPaginator
  @ViewChild(MatSort) sortStationManagmentTable: MatSort;
  searchCriterStationManagment: DatatableRequestWrapper<StationManagmentFilterModel> = new DatatableRequestWrapper();
  ///
  stationManagmentIsActiveUpdateModel: StationManagmentIsActiveUpdateModel = new StationManagmentIsActiveUpdateModel();
  public lat: number;
  public lng: number;
  zoom: number = 5.7;

  //#region  station type enum
  stationManagmentTypeMapping = stationManagmentTypeMapping;
  stationManagmentTypes = Object.values(StationManagmentType).filter(value => typeof value === 'string');
  stationManagmentType = StationManagmentType;
  //#endregion

  constructor(private srvProgressSpinner: ProgressSpinnerService,
    private srvStationManagmentService: StationManagmentService,
    private srvUtils: UtilsService,
    private router: Router,
    public dialog: MatDialog,
    public gMaps: GoogleMapsAPIWrapper,
    private srvCompanyService: CompanyService,
    private subheader: SubHeaderService,
    public yesNoDialog: MatDialog,
  ) {
  }
  public map: any;
  ngOnDestroy() {
  }
  ngOnInit(): void {
    //#region  admin oturum bilgileri setleniyor
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
    //#endregion
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'İstasyon Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'İstasyon Listesi', linkPath: null, linkText: '', isActive: false },
    ]);
    //#region  istasyon filter model oluşturuluyor
    this.searchCriterStationManagment.data = new StationManagmentFilterModel();
    //#endregion
    //#region  firma filter model setleniyor
    this.companyFilter.id = this.admin.companyId;
    //#endregion
    this.srvProgressSpinner.show();
    this.getCompanies();
    this.initForm(false);
  }
  initForm(hasFilter: boolean) {
    //#region  istasyon filter model setleniyor
    this.searchCriterStationManagment.data.companyId = this.companyFilter.id;
    //#endregion
    this.setCurrentPosition();
    this.clearParameters();
  }
  public mapReady(map) {
    this.gMaps = map;
  }
  markerClicked(long, lat) {
  }
  openWindow(id: number, longtitude: number, latitude: number, index: number) {
    this.stationManagmentList.data.map((item) => {
      item.isOpen = false;
    })
    this.stationManagmentList.data.filter(x => x.id == id).map((item) => {
      item.isOpen = true;
    })
    this.zoom = 10;
  }
  private setCurrentPosition() {
    this.lat = 39.333439;
    this.lng = 35.831485;
  }
  getCompanies() {
    this.srvCompanyService.getCompanyForSelectList(this.companyFilter).subscribe((response: CompanySelectListModel[]) => {
      this.companyList = response;
      this.initForm(true);
    }, (error) => {
    });
  }
  getStations(hasFilter: boolean) {
    console.log(this.admin);
    this.stationManagmentList.data = [];
    this.srvProgressSpinner.show();
    if (hasFilter && this.paginatorStationManagmentTable) {
      this.paginatorStationManagmentTable.pageIndex = 0;
    }
    if (this.sortStationManagmentTable && this.sortStationManagmentTable.direction != "" && this.sortStationManagmentTable.active != "") {
      this.searchCriterStationManagment.orderDirective = this.sortStationManagmentTable.direction;
      this.searchCriterStationManagment.orderProperty = this.sortStationManagmentTable.active;
    } else {
      this.searchCriterStationManagment.orderDirective = "desc";
      this.searchCriterStationManagment.orderProperty = "Id";
    }
    if (this.paginatorStationManagmentTable) {
      this.searchCriterStationManagment.recordPerPage = this.paginatorStationManagmentTable.pageSize;
      this.searchCriterStationManagment.pageNumber = this.paginatorStationManagmentTable.pageIndex;
    } else {
      this.searchCriterStationManagment.recordPerPage = 15;
    }
    this.srvStationManagmentService.getStationManagmentDataTablePanel(this.searchCriterStationManagment).subscribe((response: DatatableResponseWrapper<StationManagmentListModel[]>) => {
      this.stationManagmentList.data = response.data;
      this.totalRecordSizeStationManagmentTable = response.recordCount;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorStationManagmentList(error);
    });
  }
  ngAfterViewInit(): void {
    this.sortStationManagmentTable.sortChange.subscribe(
      () => {
        this.paginatorStationManagmentTable.pageIndex = 0
      }
    );
    merge(this.sortStationManagmentTable.sortChange, this.paginatorStationManagmentTable.page)
      .pipe(
        tap(() => {
          this.getStations(false);
        })
      )
      .subscribe(() => { }, () => null);
  }

  onErrorStationManagmentList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  clearParameters() {
    this.searchCriterStationManagment.data.description = "";
    this.searchCriterStationManagment.data.name = "";
    this.searchCriterStationManagment.data.isActive = null;
    this.getStations(true);
  }
  onEditBtnClicked(id) {
    this.router.navigate(["/stations/update", { id: id }]);
  }
  onNewBtnClicked() {
    this.router.navigate(["stations/add"])

  }
  goChargeDeviceListBtnClicked(stationManagmentId) {
    this.router.navigate(["/chargeDevices/list", { stationManagmentId: stationManagmentId }]);
  }
  goRemoteManagmentBtnClicked(stationManagmentId) {
    this.router.navigate(["/chargeDevices/remoteManagment", { stationManagmentId: stationManagmentId }]);
  }
  changeActiveStateDialog(id, isActive, ob: MatSlideToggleChange) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "İstasyonun durumunu değiştirmek istediğinize emin misiniz? " },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.changeActiveState(id, isActive, ob);
      } else {
        ob.source.checked = isActive;
      }
    });
  }
  changeActiveState(id, isActive, ob: MatSlideToggleChange) {
    this.srvProgressSpinner.show();
    this.stationManagmentIsActiveUpdateModel.id = id;
    this.srvStationManagmentService.changeIsActive(this.stationManagmentIsActiveUpdateModel).subscribe((response: any) => {
      this.stationManagmentList.data.filter(x => x.id == id).map(item => {
        item.isActive = !isActive;
      });
      this.srvUtils.showActionNotification("Başarıyla Güncellendi", EnumMessageType.Success, 2000);
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      ob.source.checked = isActive;
      this.onErrorStationManagmentList(error);
    });
  }

  removeStation(id) {
    this.srvProgressSpinner.show();
    this.stationRemoveRequestModel.id = id;
    this.srvStationManagmentService.removeStation(this.stationRemoveRequestModel).subscribe((response: any) => {
      this.stationManagmentList.data = this.stationManagmentList.data.filter(x => x.id != id);
      this.srvUtils.showActionNotification("Başarıyla Güncellendi", EnumMessageType.Success, 2000);
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorStationManagmentList(error);
    });
  }

  removeStationDialog(id) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "İstasyonu kaldırmak istediğinize emin misiniz? " },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.removeStation(id);
      } else {
      }
    });
  }
}
