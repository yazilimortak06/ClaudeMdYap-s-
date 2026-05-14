// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\campaign\campaign-list\campaign-list.component.ts
// Pattern: List/Table Component — MatTableDataSource, MatPaginator, MatSort, DatatableRequestWrapper,
// filter + clear, satır bazlı aksiyon (düzenle/kaldır), MatSlideToggle ile state değiştirme,
// EvetHayirDialog ile onay mekanizması

import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSelect } from '@angular/material/select';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { merge, tap } from 'rxjs';
import { CampaignFilterModel } from 'src/app/evtech/models/campaign/campaign-filter-model';
import { CampaignListModel } from 'src/app/evtech/models/campaign/campaign-list-model';
import { CampaignModel } from 'src/app/evtech/models/campaign/campaign-model';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';
import { CampaignChangeStateUpdateModel } from 'src/app/evtech/models/campaign/campaign-changeState-update-model';
import { CampaignService } from 'src/app/evtech/services/campaign/campaign-service';
import { environment } from 'src/environments/environment';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { CampaignRemoveRequestModel } from 'src/app/evtech/models/campaign/campaign-remove-request-model';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-campaign-list',
  templateUrl: './campaign-list.component.html',
  styleUrls: ['./campaign-list.component.scss']
})
export class CampaignListComponent implements OnInit {

  /// Datatable değişkenleri ayarlanıyor
  showingColumnNamesCampaignTable: string[] = ['Id', 'Picture', 'Title', 'Content', 'State', 'Edit'];
  campaignList: MatTableDataSource<CampaignListModel> = new MatTableDataSource();
  totalRecordSizeCampaignTable: number;
  @ViewChild(MatPaginator) paginatorCampaignTable: MatPaginator;
  @ViewChild(MatSort) sortCampaignTable: MatSort;
  searchCriterCampaign: DatatableRequestWrapper<CampaignFilterModel> = new DatatableRequestWrapper();
  ///
  campaignRemoveRequest: CampaignRemoveRequestModel = new CampaignRemoveRequestModel();
  imageUrl: string;
  campaignChangeStateUpdateMdel: CampaignChangeStateUpdateModel = new CampaignChangeStateUpdateModel();

  constructor(private srvProgressSpinner: ProgressSpinnerService,
    private srvCampaignService: CampaignService,
    private srvUtils: UtilsService,
    private router: Router,
    public dialog: MatDialog,
    private subheader: SubHeaderService,
    public yesNoDialog: MatDialog,
  ) {
    this.imageUrl = environment.imageUrl;
  }

  ngOnInit(): void {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'Kampanya İşlemleri', linkPath: null, linkText: '', isActive: false },
      { title: 'Kampanya Listesi', linkPath: null, linkText: '', isActive: false },
    ]);
    this.searchCriterCampaign.data = new CampaignFilterModel();
    this.srvProgressSpinner.show();
    this.getCampaigns(false);
  }

  getCampaigns(hasFilter:boolean) {
    this.campaignList.data = [];
    this.srvProgressSpinner.show();
    if(hasFilter){
      this.paginatorCampaignTable.pageIndex = 0;
    }
    if (this.sortCampaignTable && this.sortCampaignTable.direction != "" && this.sortCampaignTable.active != "") {
      this.searchCriterCampaign.orderDirective = this.sortCampaignTable.direction;
      this.searchCriterCampaign.orderProperty = this.sortCampaignTable.active;
    }else{
      this.searchCriterCampaign.orderDirective = "desc"
      this.searchCriterCampaign.orderProperty = "Id";
    }
    if (this.paginatorCampaignTable) {
      this.searchCriterCampaign.recordPerPage = this.paginatorCampaignTable.pageSize;
      this.searchCriterCampaign.pageNumber = this.paginatorCampaignTable.pageIndex;
    } else {
      this.searchCriterCampaign.recordPerPage = 15;
    }
    this.srvCampaignService.list(this.searchCriterCampaign).subscribe((response: DatatableResponseWrapper<CampaignListModel[]>) => {
      this.campaignList.data = response.data;
      this.totalRecordSizeCampaignTable = response.recordCount;
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorCampaignList(error);
    });
  }

  ngAfterViewInit(): void {
    this.sortCampaignTable.sortChange.subscribe(
      () => {
        this.paginatorCampaignTable.pageIndex = 0
      }
    );
    merge(this.sortCampaignTable.sortChange, this.paginatorCampaignTable.page)
      .pipe(
        tap(() => {
          this.getCampaigns(false);
        })
      )
      .subscribe(() => { }, () => null);
  }

  onErrorCampaignList(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  clearParameters() {
    this.searchCriterCampaign.data.title = null;
    this.searchCriterCampaign.data.content = null;
    this.getCampaigns(true);
  }

  onEditBtnClicked(id) {
    this.router.navigate(["/campaign/update", { id: id }]);
  }

  onNewBtnClicked() {
    this.router.navigate(["campaign/add"])
  }

  changeActiveStateDialog(id, isActive, ob: MatSlideToggleChange) {
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "Kampanyanın durumunu değiştirmek istediğinize emin misiniz? " },
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
    this.campaignChangeStateUpdateMdel.id = id;
    this.srvCampaignService.changeIsActive(this.campaignChangeStateUpdateMdel).subscribe((response: any) => {
      this.campaignList.data.filter(x => x.id == id).map(item => {
        item.state = !isActive
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

  removeDialog(id : number){
    const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
      width: '300px',
      data: { title: "", description: "Kampanyayı Kaldırmak İstediğinize Emin misiniz ?" },
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.remove(id);
      }
    });
  }

  remove(id) {
    this.srvProgressSpinner.show();
    this.campaignRemoveRequest.id = id;
    this.srvCampaignService.removeCampaign(this.campaignRemoveRequest).subscribe((response: any) => {
      this.campaignList.data = this.campaignList.data.filter(x => x.id != id);
      this.srvUtils.showActionNotification("Başarıyla Kaldırıldı", EnumMessageType.Success, 2000);
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorRemoveCampaign(error);
    });
  }

  onErrorRemoveCampaign(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }
}
