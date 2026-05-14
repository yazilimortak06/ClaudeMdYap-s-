// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\campaign\campaign-add\campaign-add.component.ts
// Pattern: Add/Form Component — PixdinnDropzone ile resim upload, multi-language content, stepper form

import { GoogleMapsAPIWrapper, MapsAPILoader, MouseEvent } from '@agm/core';
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { FileDropZoneData } from "src/app/core/external-components/models/file-dropzone-model";
import { PixdinnDropzoneComponent } from "src/app/core/external-components/pixdinn-dropzone/pixdinn-dropzone.component";
import { CampaignContentInputModel } from 'src/app/evtech/models/campaign/campaign-content-input-model';
import { CampaignFileUploadResponseModel } from 'src/app/evtech/models/campaign/campaign-file-model';
import { CampaignInsertModel } from 'src/app/evtech/models/campaign/campaign-insert-model';
import { ContentLanguageModel } from "src/app/evtech/models/contentLanguage/contentLanguage-model";
import { CampaignService } from 'src/app/evtech/services/campaign/campaign-service';
import { ContentLanguageService } from "src/app/evtech/services/contentLanguage/contentLanguage-service";
import { ProgressSpinnerService } from "src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service";
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from "src/app/shared_admin/utils/services/utils.service";
import { environment } from "src/environments/environment";

@Component({
    selector: 'app-campaign-add',
    templateUrl: './campaign-add.component.html',
    styleUrls: ['./campaign-add.component.scss']
})
export class AddCampaignComponent implements OnInit {
  isLinear = false;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  campaignInsertModel: CampaignInsertModel = new CampaignInsertModel();
  contentLanguageList: ContentLanguageModel[] = [];
  contentInputs: CampaignContentInputModel[] = [];
  oldPicture :FileDropZoneData[] =[];
  @ViewChild('addCampaignPictureDropzone') addCampaignPictureDropzone?: PixdinnDropzoneComponent;
  imageUrl:string;

  constructor(private _formBuilder: FormBuilder,
    private router: Router,
    private srvContentLanguage: ContentLanguageService,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvCampaign: CampaignService,
    private srvUtils: UtilsService,
    private subheader: SubHeaderService
  ) {
    this.campaignInsertModel.campaignContent = [];
    this.campaignInsertModel.campaignPicture = [];
    this.imageUrl = environment.imageUrl;
  }

  ngAfterViewInit(): void {
    this.addCampaignPictureListener();
  }

  ngOnInit() {
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``,linkText:'' , isActive : true },
      { title: 'Kampanya İşlemleri', linkPath: null,linkText:'',isActive : false },
      { title: 'Kampanya Listesi', linkPath: '/campaign/list',linkText:'',isActive : true },
      { title: 'Kampanya Ekle', linkPath: null,linkText:null,isActive : false },
    ]);
    this.getContentLanguageList();
  }
  addCampaignPictureListener() {
    this.addCampaignPictureDropzone.onFileSelected = (onSelected: FileDropZoneData) => {
      this.sendCampaignFileToServer(onSelected);
    }
  }

  sendCampaignFileToServer(onSelected: FileDropZoneData) {
    this.srvCampaign.addFile(onSelected).subscribe((response: CampaignFileUploadResponseModel) => {
      onSelected.downloadFunction = (item: FileDropZoneData) => {
        window.location.href =   item.absoluteFileUrl;
      };
      this.addCampaignPictureDropzone.onFileUploaded(this.imageUrl+response.fileKey+"&&group="+1, onSelected.index, response.fileKey);
      console.log(this.addCampaignPictureDropzone.fileData);
      console.log(this.addCampaignPictureDropzone.getInsertedFiles);
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorAddCampaignFile(error, onSelected);
    });
  }

  onErrorAddCampaignFile(error: any, selected: FileDropZoneData) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showServerError(errorData);
    selected.reUploadFunction = (item: FileDropZoneData) => {
      this.addCampaignPictureDropzone.onFileUploadInit(selected);
      this.sendCampaignFileToServer(selected);
    };
    this.addCampaignPictureDropzone.onFileUploadedError(selected.index, errorData.errorMessage);
  }

  getContentLanguageList() {
    this.srvProgressSpinner.show();
    this.srvContentLanguage.list().subscribe((response: ContentLanguageModel[]) => {
      this.contentLanguageList = response;
      this.contentLanguageList.map((item) => {
        this.contentInputs.push({
          campaignContentId:0,
          campaignId:0,
          contentLanguageId:item.id,
          description:item.description,
          content:item.content,
          campaignTitle:"",
          campaignContent:"",
          isDefault:item.isDefault
        });
      })
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  save() {
    this.srvProgressSpinner.show();
    this.campaignInsertModel.id = 0;
    this.contentInputs.map((item) => {
      this.campaignInsertModel.campaignContent.push({
        title: item.campaignTitle,
        content: item.campaignContent,
        campaignId: this.campaignInsertModel.id,
        id:0,
        contentLanguageId: item.contentLanguageId
      });
    });
    this.addCampaignPictureDropzone.fileData.map((item) => {
      this.campaignInsertModel.campaignPicture.push({
        id : 0,
        campaignId : 0,
        mediaGuiId : item.guid,
      });
    })
    this.campaignInsertModel.state = true;
    this.srvCampaign.add(this.campaignInsertModel).subscribe((response: any) => {
      this.srvUtils.showActionNotification("Başarıyla Eklendi",EnumMessageType.Success, 8000);
      setTimeout(() => {
        this.router.navigate(['campaign/list']);
      }, 2500);
    }, (error) => {
      this.campaignInsertModel.campaignContent = [];
      this.campaignInsertModel.campaignPicture = [];
      this.srvProgressSpinner.hide();
    });
  }



}
