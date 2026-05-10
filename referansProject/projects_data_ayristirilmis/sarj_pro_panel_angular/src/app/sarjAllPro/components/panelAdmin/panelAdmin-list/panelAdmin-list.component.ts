// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\sarjAllPro\components\panelAdmin\panelAdmin-list\panelAdmin-list.component.ts

import { GoogleMapsAPIWrapper, Marker } from '@agm/core';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSelect } from '@angular/material/select';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { merge, tap, Subject } from 'rxjs';
import { PanelAdminFilterModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-filter-model';
import { PanelAdminIsActiveUpdateModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-isActive-update-model';
import { PanelAdminListModel } from 'src/app/sarjAllPro/models/panelAdmin/panelAdmin-list-model';
import { PanelAdminService } from 'src/app/sarjAllPro/services/panelAdmin/panelAdmin-service';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';

@Component({
  selector: 'app-panelAdmin-list',
  templateUrl: './panelAdmin-list.component.html',
  styleUrls: ['./panelAdmin-list.component.scss']
})
export class PanelAdminListComponent implements OnInit {
  /// Datatable değişkenleri ayarlanıyor
  showingColumnNamesPanelAdminTable: string[] = ['Id', 'Name', 'Surname', 'TypeName', 'AdminManagmentType', 'CompanyName', 'IsActive', 'Edit'];
  panelAdminList: MatTableDataSource<PanelAdminListModel> = new MatTableDataSource();
  totalRecordSizePanelAdminTable: number;
  @ViewChild(MatPaginator) paginatorPanelAdminTable: MatPaginator;
  @ViewChild(MatSort) sortPanelAdminTable: MatSort;
  searchCriterPanelAdmin: DatatableRequestWrapper<PanelAdminFilterModel> = new DatatableRequestWrapper();
  ///
  panelAdminIsActiveUpdateModel: PanelAdminIsActiveUpdateModel = new PanelAdminIsActiveUpdateModel();

  constructor(
    private srvProgressSpinner: ProgressSpinnerService,
    private srvPanelAdminService: PanelAdminService,
    private srvUtils: UtilsService,
    private router: Router,
    public dialog: MatDialog,
    private subheader: SubHeaderService,
    public yesNoDialog: MatDialog,
  ) { }

  ngOnInit(): void {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'Admin Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'Adminler', linkPath: null, linkText: '', isActive: false },
    ]);
    this.searchCriterPanelAdmin.data = new PanelAdminFilterModel();
    this.srvProgressSpinner.show();
    this.getPanelAdmins(false);
  }

  getPanelAdmins(hasFilter: boolean) {
    this.panelAdminList.data = [];
    this.srvProgressSpinner.show();
    if (hasFilter) {
      this.paginatorPanelAdminTable.pageIndex = 0;
    }
    if (this.sortPanelAdminTable && this.sortPanelAdminTable.direction != "" && this.sortPanelAdminTable.active != "") {
      this.searchCriterPanelAdmin.orderDirective = this.sortPanelAdminTable.direction;
      this.searchCriterPanelAdmin.orderProperty = this.sortPanelAdminTable.active;
    } else {
      this.searchCriterPanelAdmin.orderDirective = "desc";
      this.searchCriterPanelAdmin.orderProperty = "Id";
    }
    if (this.paginatorPanelAdminTable) {
      this.searchCriterPanelAdmin.recordPerPage = this.paginatorPanelAdminTable.pageSize;
      this.searchCriterPanelAdmin.pageNumber = this.paginatorPanelAdminTable.pageIndex;
    } else {
      this.searchCriterPanelAdmin.recordPerPage = 15;
    }
    this.srvPanelAdminService.list(this.searchCriterPanelAdmin).subscribe((response: DatatableResponseWrapper<PanelAdminListModel[]>) => {
      this.panelAdminList.data = response.data;
      this.totalRecordSizePanelAdminTable = response.recordCount;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorPanelAdminList(error);
    });
  }

  ngAfterViewInit(): void {
    this.sortPanelAdminTable.sortChange.subscribe(
      () => {
        this.paginatorPanelAdminTable.pageIndex = 0;
      }
    );
    merge(this.sortPanelAdminTable.sortChange, this.paginatorPanelAdminTable.page)
      .pipe(
        tap(() => {
          this.getPanelAdmins(false);
        })
      )
      .subscribe(() => { }, () => null);
  }

  onErrorPanelAdminList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  clearParameters() {
    this.searchCriterPanelAdmin.data.name = "";
    this.searchCriterPanelAdmin.data.surname = "";
    this.getPanelAdmins(true);
  }

  onEditBtnClicked(id) {
    this.router.navigate(["/panelAdmin/update", { id: id }]);
  }

  onNewBtnClicked() {
    this.router.navigate(["panelAdmin/add"]);
  }

  changeActiveStateDialog(id, isActive, ob: MatSlideToggleChange) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "Adminin durumunu değiştirmek istediğinize emin misiniz? " },
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
    this.panelAdminIsActiveUpdateModel.id = id;
    this.srvPanelAdminService.changeIsActive(this.panelAdminIsActiveUpdateModel).subscribe((response: any) => {
      this.panelAdminList.data.filter(x => x.id == id).map(item => {
        item.isActive = !isActive;
      });
      this.srvUtils.showActionNotification("Başarıyla Güncellendi", EnumMessageType.Success, 2000);
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      ob.source.checked = isActive;
      this.onErrorChangeActiveState(error);
    });
  }

  onErrorChangeActiveState(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }
}
