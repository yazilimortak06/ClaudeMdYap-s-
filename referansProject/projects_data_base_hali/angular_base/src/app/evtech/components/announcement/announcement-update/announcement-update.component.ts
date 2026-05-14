// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\announcement\announcement-update\announcement-update.component.ts
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { FileDropZoneData } from 'src/app/core/external-components/models/file-dropzone-model';
import { PixdinnDropzoneComponent } from 'src/app/core/external-components/pixdinn-dropzone/pixdinn-dropzone.component';
import { environment } from "src/environments/environment";
import { AnnouncementModel } from 'src/app/evtech/models/announcement/announcement-model';
import { AnnouncementUpdateModel } from 'src/app/evtech/models/announcement/announcement-update-model';
import { AnnouncementContentInputModel } from 'src/app/evtech/models/announcement/announcement-content-input-model';
import { AnnouncementService } from 'src/app/evtech/services/announcement/announcement-service';
import { AnnouncementFileUploadResponseModel } from 'src/app/evtech/models/announcement/announcement-file-model';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { Subscription } from 'rxjs';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import moment from 'moment';

@Component({
    selector: 'app-announcement-update',
    templateUrl: './announcement-update.component.html',
    styleUrls: ['./announcement-update.component.scss'],
})
export class UpdateAnnouncementComponent implements OnInit {
    idSubscription: Subscription;
    isLinear = false;
    announcementUpdateModel: AnnouncementUpdateModel = new AnnouncementUpdateModel();
    contentInputs: AnnouncementContentInputModel[] = [];
    oldPicture: FileDropZoneData[] = [];
    todayDate: Date = new Date();
    imageUrl: string;

    @ViewChild('addAnnouncementPictureDropzone') addAnnouncementPictureDropzone?: PixdinnDropzoneComponent;

    constructor(
        private _formBuilder: FormBuilder,
        private router: Router,
        private route: ActivatedRoute,
        private srvAnnouncement: AnnouncementService,
        private srvProgressSpinner: ProgressSpinnerService,
        private srvUtils: UtilsService,
        private subheader: SubHeaderService
    ) {
        this.announcementUpdateModel.announcement = new AnnouncementModel();
        this.announcementUpdateModel.announcementContent = [];
        this.announcementUpdateModel.announcementContentFormInput = [];
        this.announcementUpdateModel.announcementPicture = [];
        this.announcementUpdateModel.announcementPicturesDeleted = [];
        this.announcementUpdateModel.announcementPictureFormInput = [];
        this.imageUrl = environment.imageUrl;
    }

    addAnnouncementPictureListener() {
        this.addAnnouncementPictureDropzone.onFileSelected = (onSelected: FileDropZoneData) => {
            this.sendAnnouncementFileToServer(onSelected);
        };
    }

    sendAnnouncementFileToServer(onSelected: FileDropZoneData) {
        this.srvAnnouncement.addFile(onSelected).subscribe((response: AnnouncementFileUploadResponseModel) => {
            onSelected.downloadFunction = (item: FileDropZoneData) => { window.location.href = item.absoluteFileUrl; };
            this.addAnnouncementPictureDropzone.onFileUploaded(this.imageUrl + response.fileKey + "&&group=" + 1, onSelected.index, response.fileKey);
        }, (error) => {
            this.srvProgressSpinner.hide();
            let errorData = this.srvUtils.getServerErrorRequest(error);
            this.srvUtils.showServerError(errorData);
            onSelected.reUploadFunction = (item: FileDropZoneData) => {
                this.addAnnouncementPictureDropzone.onFileUploadInit(onSelected);
                this.sendAnnouncementFileToServer(onSelected);
            };
            this.addAnnouncementPictureDropzone.onFileUploadedError(onSelected.index, errorData.errorMessage);
        });
    }

    ngAfterViewInit(): void {
        this.addAnnouncementPictureListener();
    }

    ngOnInit() {
        this.subheader.setBreadcrumbs([
            { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
            { title: 'Duyuru Islemleri', linkPath: null, linkText: '', isActive: false },
            { title: 'Duyuru Listesi', linkPath: '/announcement/list', linkText: '', isActive: true },
            { title: 'Duyuru Guncelle', linkPath: null, linkText: '', isActive: false },
        ]);
        this.idSubscription = this.route.params.subscribe(params => {
            let id = params['id'];
            this.announcementUpdateModel.announcement.id = id;
            this.announcementUpdateModel.announcementContent = [];
            this.getAnnouncement(this.announcementUpdateModel.announcement.id);
        });
    }

    getAnnouncement(id) {
        this.srvProgressSpinner.show();
        this.srvAnnouncement.getAnnouncementById(id).subscribe(async (response: AnnouncementUpdateModel) => {
            this.announcementUpdateModel.announcement.validityDate = response.announcement.validityDate;
            this.announcementUpdateModel.announcementPictureFormInput = response.announcementPictureFormInput;
            this.contentInputs = response.announcementContentFormInput;
            this.srvProgressSpinner.hide();
            this.initPictures();
        }, (error) => {
            this.srvProgressSpinner.hide();
        });
    }

    initPictures() {
        this.announcementUpdateModel.announcementPictureFormInput.map((item) => {
            this.oldPicture.push({
                url: this.imageUrl + item.code + "&&group=" + 1,
                onUploaded: true,
                fileId: item.id,
                guid: item.code,
                fileType: 1,
                fileRelativeName: "",
                index: 0,
                oldfile: true,
                absoluteFileUrl: this.imageUrl + item.code + "&&group=" + 1,
                uploadError: false,
                uploadErrorMessage: "",
                reUploadFunction: null,
                downloadFunction: null,
                file: null,
                setFile: null,
                setRelativeName: null
            });
        });
        this.addAnnouncementPictureDropzone.initFileData(this.oldPicture);
    }

    update() {
        this.srvProgressSpinner.show();
        this.contentInputs.map((item) => {
            this.announcementUpdateModel.announcementContent.push({
                id: item.announcementContentId ? item.announcementContentId : 0,
                title: item.announcementTitle,
                content: item.announcementContent,
                contentLanguageId: item.contentLanguageId,
                announcementId: this.announcementUpdateModel.announcement.id,
            });
        });
        this.addAnnouncementPictureDropzone.fileData.filter(x => !x.oldfile).map((item) => {
            this.announcementUpdateModel.announcementPicture.push({ id: 0, mediaGuiId: item.guid, announcementId: this.announcementUpdateModel.announcement.id });
        });
        this.addAnnouncementPictureDropzone.fileDataDeleted.filter(x => x.oldfile).map((item) => {
            this.announcementUpdateModel.announcementPicturesDeleted.push({ id: item.fileId, announcementId: this.announcementUpdateModel.announcement.id, mediaGuiId: item.guid });
        });
        if (this.announcementUpdateModel.announcement.validityDate != null) {
            this.announcementUpdateModel.announcement.validityDate = moment(this.announcementUpdateModel.announcement.validityDate).set({ hour: 23, minute: 59 }).toDate();
        }
        this.srvAnnouncement.update(this.announcementUpdateModel).subscribe((response: any) => {
            this.srvUtils.showActionNotification("Basariyla Guncellendi", EnumMessageType.Success, 8000);
            setTimeout(() => { this.router.navigate(['announcement/list']); }, 2500);
        }, (error) => {
            this.contentInputs = [];
            this.announcementUpdateModel.announcementPictureFormInput = [];
            this.srvProgressSpinner.hide();
        });
    }

    resetValidityDate() {
        this.announcementUpdateModel.announcement.validityDate = new Date();
    }
}
