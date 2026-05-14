// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\announcement\announcement-list\announcement-list.component.ts
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { merge, tap } from 'rxjs';
import { AnnouncementFilterModel } from 'src/app/evtech/models/announcement/announcement-filter-model';
import { AnnouncementListModel } from 'src/app/evtech/models/announcement/announcement-list-model';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { DatatableRequestWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model';
import { DatatableResponseWrapper } from 'src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model';
import { AnnouncementChangeStateUpdateModel } from 'src/app/evtech/models/announcement/announcement-changeState-update-model';
import { AnnouncementService } from 'src/app/evtech/services/announcement/announcement-service';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { AnnouncementRemoveRequestModel } from 'src/app/evtech/models/announcement/announcement-remove-request-model';
import { EvetHayirDialogComponent } from 'src/app/shared_admin/partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-announcement-list',
  templateUrl: './announcement-list.component.html',
  styleUrls: ['./announcement-list.component.scss']
})
export class AnnouncementListComponent implements OnInit {

    showingColumnNamesAnnouncementTable: string[] = ['Id', 'Picture', 'Title', 'Content', 'State', 'Edit'];
    announcementList: MatTableDataSource<AnnouncementListModel> = new MatTableDataSource();
    totalRecordSizeAnnouncementTable: number;
    @ViewChild(MatPaginator) paginatorAnnouncementTable: MatPaginator;
    @ViewChild(MatSort) sortAnnouncementTable: MatSort;
    searchCriterAnnouncement: DatatableRequestWrapper<AnnouncementFilterModel> = new DatatableRequestWrapper();
    announcementRemoveRequest: AnnouncementRemoveRequestModel = new AnnouncementRemoveRequestModel();
    announcementChangeStateUpdateMdel: AnnouncementChangeStateUpdateModel = new AnnouncementChangeStateUpdateModel();

    constructor(
        private srvProgressSpinner: ProgressSpinnerService,
        private srvAnnouncementService: AnnouncementService,
        private srvUtils: UtilsService,
        private router: Router,
        public dialog: MatDialog,
        private subheader: SubHeaderService,
        public yesNoDialog: MatDialog,
    ) { }

    ngOnInit(): void {
        this.subheader.setBreadcrumbs([
            { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
            { title: 'Duyuru Islemleri', linkPath: null, linkText: '', isActive: false },
            { title: 'Duyuru Listesi', linkPath: null, linkText: '', isActive: false },
        ]);
        this.searchCriterAnnouncement.data = new AnnouncementFilterModel();
        this.srvProgressSpinner.show();
        this.getAnnouncements(false);
    }

    getAnnouncements(hasFilter: boolean) {
        this.announcementList.data = [];
        this.srvProgressSpinner.show();
        if (hasFilter) { this.paginatorAnnouncementTable.pageIndex = 0; }
        if (this.sortAnnouncementTable && this.sortAnnouncementTable.direction != "" && this.sortAnnouncementTable.active != "") {
            this.searchCriterAnnouncement.orderDirective = this.sortAnnouncementTable.direction;
            this.searchCriterAnnouncement.orderProperty = this.sortAnnouncementTable.active;
        } else {
            this.searchCriterAnnouncement.orderDirective = "desc";
            this.searchCriterAnnouncement.orderProperty = "Id";
        }
        if (this.paginatorAnnouncementTable) {
            this.searchCriterAnnouncement.recordPerPage = this.paginatorAnnouncementTable.pageSize;
            this.searchCriterAnnouncement.pageNumber = this.paginatorAnnouncementTable.pageIndex;
        } else {
            this.searchCriterAnnouncement.recordPerPage = 15;
        }
        this.srvAnnouncementService.list(this.searchCriterAnnouncement).subscribe((response: DatatableResponseWrapper<AnnouncementListModel[]>) => {
            this.announcementList.data = response.data;
            this.totalRecordSizeAnnouncementTable = response.recordCount;
        }, (error) => {
            this.srvProgressSpinner.hide();
            this.onErrorAnnouncementList(error);
        });
    }

    ngAfterViewInit(): void {
        this.sortAnnouncementTable.sortChange.subscribe(() => {
            this.paginatorAnnouncementTable.pageIndex = 0;
        });
        merge(this.sortAnnouncementTable.sortChange, this.paginatorAnnouncementTable.page)
            .pipe(tap(() => { this.getAnnouncements(false); }))
            .subscribe(() => { }, () => null);
    }

    onErrorAnnouncementList(error: any) {
        let errorData = this.srvUtils.getServerErrorRequest(error);
        this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
    }

    clearParameters() {
        this.searchCriterAnnouncement.data.title = null;
        this.searchCriterAnnouncement.data.content = null;
        this.getAnnouncements(true);
    }

    onEditBtnClicked(id) { this.router.navigate(["/announcement/update", { id: id }]); }
    onNewBtnClicked() { this.router.navigate(["announcement/add"]); }

    changeActiveStateDialog(id, state, ob: MatSlideToggleChange) {
        const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
            width: '300px',
            data: { title: "", description: "Duyurunun durumunu degistirmek istediginize emin misiniz? " },
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) { this.changeActiveState(id, state, ob); }
            else { ob.source.checked = state; }
        });
    }

    changeActiveState(id, state, ob: MatSlideToggleChange) {
        this.srvProgressSpinner.show();
        this.announcementChangeStateUpdateMdel.id = id;
        this.srvAnnouncementService.changeIsActive(this.announcementChangeStateUpdateMdel).subscribe((response: any) => {
            this.announcementList.data.filter(x => x.id == id).map(item => { item.state = !state; });
            this.srvUtils.showActionNotification("Basariyla Guncellendi", EnumMessageType.Success, 2000);
            this.srvProgressSpinner.hide();
        }, (error) => {
            this.srvProgressSpinner.hide();
            ob.source.checked = state;
            let errorData = this.srvUtils.getServerErrorRequest(error);
            this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
        });
    }

    removeDialog(id: number) {
        const dialogRef = this.yesNoDialog.open(EvetHayirDialogComponent, {
            width: '300px',
            data: { title: "", description: "Duyuruyu Kaldirmak Istediginize Emin misiniz ?" },
        });
        dialogRef.afterClosed().subscribe(result => {
            if (result) { this.remove(id); }
        });
    }

    remove(id) {
        this.srvProgressSpinner.show();
        this.announcementRemoveRequest.id = id;
        this.srvAnnouncementService.removeAnnouncement(this.announcementRemoveRequest).subscribe((response: any) => {
            this.announcementList.data = this.announcementList.data.filter(x => x.id != id);
            this.srvUtils.showActionNotification("Basariyla Kaldirildi", EnumMessageType.Success, 2000);
            this.srvProgressSpinner.hide();
        }, (error) => {
            this.srvProgressSpinner.hide();
            let errorData = this.srvUtils.getServerErrorRequest(error);
            this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
        });
    }
}
