// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\campaign\campaign-update\campaign-update.component.ts
// Pattern: Edit/Update Component — route params ile id alır, mevcut veriyi çeker, dropzone ile resim yönetimi, güncelleme işlemi

import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ProgressSpinnerService } from 'src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service';
import { UtilsService } from 'src/app/shared_admin/utils/services/utils.service';
import { FileDropZoneData } from 'src/app/core/external-components/models/file-dropzone-model';
import { PixdinnDropzoneComponent } from 'src/app/core/external-components/pixdinn-dropzone/pixdinn-dropzone.component';
import { environment } from "src/environments/environment";
import { CampaignModel } from 'src/app/evtech/models/campaign/campaign-model';
import { CampaignUpdateModel } from 'src/app/evtech/models/campaign/campaign-update-model';
import { CampaignContentInputModel } from 'src/app/evtech/models/campaign/campaign-content-input-model';
import { CampaignService } from 'src/app/evtech/services/campaign/campaign-service';
import { CampaignFileUploadResponseModel } from 'src/app/evtech/models/campaign/campaign-file-model';
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { Subscription } from 'rxjs';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';


@Component({
    selector: 'app-campaign-update',
    templateUrl: './campaign-update.component.html',
    styleUrls: ['./campaign-update.component.scss']
})
export class UpdateCampaignComponent implements OnInit {
     idSubscription: Subscription;

    isLinear = false;
    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    campaignUpdateModel: CampaignUpdateModel = new CampaignUpdateModel();
    contentInputs :CampaignContentInputModel[] = [];
    oldPicture :FileDropZoneData[] =[];
    @ViewChild('addCampaignPictureDropzone') addCampaignPictureDropzone?: PixdinnDropzoneComponent;
    imageUrl:string;

    constructor(private _formBuilder: FormBuilder,
      private router: Router,
      private route: ActivatedRoute,
      private srvCampaign: CampaignService,
      private srvProgressSpinner: ProgressSpinnerService,
      private srvUtils: UtilsService,
      private subheader: SubHeaderService
    ) {
      this.campaignUpdateModel.campaign = new CampaignModel();
      this.campaignUpdateModel.campaignContent = [];
      this.campaignUpdateModel.campaignContentFormInput = [];
      this.campaignUpdateModel.campaignPicture = [];
      this.campaignUpdateModel.campaignPicturesDeleted = [];
      this.campaignUpdateModel.campaignPictureFormInput = [];
      this.imageUrl = environment.imageUrl;

    }

    addCampaignPictureListener() {
      this.addCampaignPictureDropzone.onFileSelected = (onSelected: FileDropZoneData) => {
        this.sendCampaignFileToServer(onSelected);
      }
      var files =  this.addCampaignPictureDropzone.getInsertedFiles();
    }


    sendCampaignFileToServer(onSelected: FileDropZoneData) {
      this.srvCampaign.addFile(onSelected).subscribe((response: CampaignFileUploadResponseModel) => {
        onSelected.downloadFunction = (item: FileDropZoneData) => {
          window.location.href = item.absoluteFileUrl;
        };
        this.addCampaignPictureDropzone.onFileUploaded(this.imageUrl+response.fileKey+"&&group="+1, onSelected.index, response.fileKey);
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

    ngAfterViewInit(): void {
      this.addCampaignPictureListener();
    }

      ngOnInit() {
        this.subheader.setBreadcrumbs([
          { title: 'Ana Sayfa', linkPath: ``,linkText:'' , isActive : true },
          { title: 'Kampanya İşlemleri', linkPath: null,linkText:'',isActive : false },
          { title: 'Kampanya Listesi', linkPath: '/campaign/list',linkText:'',isActive : true },
          { title: 'Kampanya Güncelle', linkPath: null,linkText:null,isActive : false },
        ]);
        this.idSubscription = this.route.params.subscribe(
          params => {
            // Listeleme ekranından gelen id değerini alıyor.
            let id = params['id'];
            this.campaignUpdateModel.campaign.id = id;
            this.campaignUpdateModel.campaignContent = [];
            this.getCampaign(this.campaignUpdateModel.campaign.id);
          });
    }

    getCampaign(id){
      this.srvProgressSpinner.show()
      this.srvCampaign.getCampaignById(id).subscribe(async (response:CampaignUpdateModel ) => {
        this.campaignUpdateModel.campaignPictureFormInput=response.campaignPictureFormInput;
        this.contentInputs = response.campaignContentFormInput;
        this.srvProgressSpinner.hide();
        this.initPictures();
      }, (error) => {
        this.srvProgressSpinner.hide();
      },
      () => {
          }
      );
    }

    initPictures(){
      this.campaignUpdateModel.campaignPictureFormInput.map((item) => {
        this.oldPicture.push({
          url:this.imageUrl+item.code+"&&group="+1,
          onUploaded : true,
          fileId : item.id,
          guid : item.code,
          fileType:1,
          fileRelativeName:"",
          index:0,
          oldfile:true,
          absoluteFileUrl:this.imageUrl+item.code+"&&group="+1,
          uploadError:false,
          uploadErrorMessage:"",
          reUploadFunction :null,
          downloadFunction :null,
          file:null,
          setFile : null,
          setRelativeName:null
        })
      })
      console.log(this.oldPicture);
      this.addCampaignPictureDropzone.initFileData(this.oldPicture);
    }

    update() {
      this.srvProgressSpinner.show();
      this.contentInputs.map((item) => {
        this.campaignUpdateModel.campaignContent.push({
            id: item.campaignContentId ? item.campaignContentId : 0,
            title: item.campaignTitle,
            contentLanguageId: item.contentLanguageId,
            content: item.campaignContent,
            campaignId: this.campaignUpdateModel.campaign.id,
          });
      });
      this.addCampaignPictureDropzone.fileData.filter(x=>!x.oldfile).map((item) => {
        this.campaignUpdateModel.campaignPicture.push({
          id : 0,
          mediaGuiId: item.guid,
          campaignId:this.campaignUpdateModel.campaign.id,
        });
      });
      this.addCampaignPictureDropzone.fileDataDeleted.filter(x=>x.oldfile).map((item) => {
        this.campaignUpdateModel.campaignPicturesDeleted.push({
          id:item.fileId,
          campaignId:this.campaignUpdateModel.campaign.id,
          mediaGuiId:item.guid
        });
      });
        this.srvCampaign.update(this.campaignUpdateModel).subscribe((response: any) => {
          this.srvUtils.showActionNotification("Başarıyla Güncellendi",EnumMessageType.Success, 8000);
          setTimeout(() => {
            this.router.navigate(['campaign/list']);
          }, 2500);
        }, (error) => {
        this.contentInputs = [];
        this.campaignUpdateModel.campaignPictureFormInput=[];
          this.srvProgressSpinner.hide();
        });
      }
}
