// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\announcement\announcement-add\announcement-add.component.ts
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import moment from 'moment';
import { FileDropZoneData } from "src/app/core/external-components/models/file-dropzone-model";
import { PixdinnDropzoneComponent } from "src/app/core/external-components/pixdinn-dropzone/pixdinn-dropzone.component";
import { AnnouncementContentInputModel } from 'src/app/evtech/models/announcement/announcement-content-input-model';
import { AnnouncementFileUploadResponseModel } from 'src/app/evtech/models/announcement/announcement-file-model';
import { AnnouncementInsertModel } from 'src/app/evtech/models/announcement/announcement-insert-model';
import { AnnouncementService } from 'src/app/evtech/services/announcement/announcement-service';
import { ProgressSpinnerService } from "src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service";
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from "src/app/shared_admin/utils/services/utils.service";
import { environment } from "src/environments/environment";

@Component({
    selector: 'app-announcement-add',
    templateUrl: './announcement-add.component.html',
    styleUrls: ['./announcement-add.component.scss']
})
export class AddAnnouncementComponent implements OnInit {
    isLinear = false;
    firstFormGroup: FormGroup;
    announcementInsertModel: AnnouncementInsertModel = new AnnouncementInsertModel();
    contentInputs: AnnouncementContentInputModel[] = [];
    oldPicture: FileDropZoneData[] = [];
    todayDate: Date = new Date();
    imageUrl: string;

    @ViewChild('addAnnouncementPictureDropzone') addAnnouncementPictureDropzone?: PixdinnDropzoneComponent;

    constructor(
        private _formBuilder: FormBuilder,
        private router: Router,
        private srvProgressSpinner: ProgressSpinnerService,
        private srvAnnouncement: AnnouncementService,
        private srvUtils: UtilsService,
        private subheader: SubHeaderService
    ) {
        this.announcementInsertModel.announcementContent = [];
        this.announcementInsertModel.announcementPicture = [];
        this.imageUrl = environment.imageUrl;
    }

    ngAfterViewInit(): void {
        this.addAnnouncementPictureListener();
    }

    ngOnInit() {
        this.subheader.setBreadcrumbs([
            { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
            { title: 'Duyuru Islemleri', linkPath: null, linkText: '', isActive: false },
            { title: 'Duyuru Listesi', linkPath: '/announcement/list', linkText: '', isActive: true },
            { title: 'Duyuru Ekle', linkPath: null, linkText: null, isActive: false },
        ]);
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

    save() {
        if (this.announcementInsertModel.validityDate != null) {
            this.announcementInsertModel.validityDate = moment(this.announcementInsertModel.validityDate).set({ hour: 23, minute: 59 }).toDate();
        }
        this.srvProgressSpinner.show();
        this.announcementInsertModel.id = 0;
        this.contentInputs.map((item) => {
            this.announcementInsertModel.announcementContent.push({
                title: item.announcementTitle,
                content: item.announcementContent,
                contentLanguageId: item.contentLanguageId,
                announcementId: this.announcementInsertModel.id,
                id: 0
            });
        });
        this.addAnnouncementPictureDropzone.fileData.map((item) => {
            this.announcementInsertModel.announcementPicture.push({ id: 0, announcementId: 0, mediaGuiId: item.guid });
        });
        this.announcementInsertModel.state = true;
        this.srvAnnouncement.add(this.announcementInsertModel).subscribe((response: any) => {
            this.srvUtils.showActionNotification("Basariyla Eklendi", EnumMessageType.Success, 8000);
            setTimeout(() => { this.router.navigate(['announcement/list']); }, 2500);
        }, (error) => {
            this.announcementInsertModel.announcementContent = [];
            this.announcementInsertModel.announcementPicture = [];
            this.srvProgressSpinner.hide();
        });
    }

    resetValidityDate() {
        this.announcementInsertModel.validityDate = null;
    }
}
