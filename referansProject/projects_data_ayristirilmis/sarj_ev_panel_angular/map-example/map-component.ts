// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\components\stations\station-add\station-add.component.ts
// Pattern: Google Maps Component — @agm/core kullanımı, agm-map + agm-marker, markerDragEnd ile koordinat alma,
// Geocoder API, ülke/il/ilçe cascading select, multi-step stepper ile harita + form entegrasyonu

import { GoogleMapsAPIWrapper, MapsAPILoader, MouseEvent } from '@agm/core';
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { FileDropZoneData } from "src/app/core/external-components/models/file-dropzone-model";
import { PixdinnDropzoneComponent } from "src/app/core/external-components/pixdinn-dropzone/pixdinn-dropzone.component";
import { StationManagmentType, stationManagmentTypeMapping } from 'src/app/evtech/enums/stationManagment/stationManagment-type';
import { CompanyFilterModel } from 'src/app/evtech/models/company/company-filter-model';
import { CompanyListItemModel } from 'src/app/evtech/models/company/company-list-item-model';
import { ContentLanguageModel } from "src/app/evtech/models/contentLanguage/contentLanguage-model";
import { CityFilterModel } from 'src/app/evtech/models/countryCityAndTown/city-filter-model';
import { CityModel } from 'src/app/evtech/models/countryCityAndTown/city-model';
import { CountryModel } from 'src/app/evtech/models/countryCityAndTown/country-model';
import { TownFilterModel } from 'src/app/evtech/models/countryCityAndTown/town-filter-model';
import { TownModel } from 'src/app/evtech/models/countryCityAndTown/town-model';
import { PaymentMethodModel } from 'src/app/evtech/models/paymentMethod/paymentMethod-model';
import { StationInsertFormPrepare } from 'src/app/evtech/models/stationManagment/station-insert-formPrepare-model';
import { StationManagmentFileUploadResponseModel } from 'src/app/evtech/models/stationManagment/station-managment-file-model';
import { StationManagmentAddressModel } from 'src/app/evtech/models/stationManagmentAddress/stationManagment-address-model';
import { StationManagmentContentInputModel } from "src/app/evtech/models/stationManagmentContent/stationManagment-content-input-model";
import { StationManagmentDetailModel } from 'src/app/evtech/models/stationManagmentDetail/stationManagment-detail-model';
import { StationManagmentInsertModel } from "src/app/evtech/models/stationManagment/stationManagment-insert-model";
import { CompanyService } from 'src/app/evtech/services/company/company-service';
import { ContentLanguageService } from "src/app/evtech/services/contentLanguage/contentLanguage-service";
import { CountryCityAndTownService } from 'src/app/evtech/services/countryCityAndTown/countryCityAndTown-service';
import { StationManagmentService } from "src/app/evtech/services/stationManagment/stationManagment-service";
import { ProgressSpinnerService } from "src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service";
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { UtilsService } from "src/app/shared_admin/utils/services/utils.service";
import { environment } from "src/environments/environment";
import { AuthenticationModel } from 'src/app/evtech/models/authentication/authentication-model';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { StationReservationSuitableDateModel } from 'src/app/evtech/models/stationReservationConfig/station-reservation-suitable-date-model';

@Component({
  selector: 'app-station-add',
  templateUrl: './station-add.component.html',
  styleUrls: ['./station-add.component.scss']
})
export class AddStationComponent implements OnInit {
  //#region  oturum modelini tutacak değişkenler tanımlanıyor
  admin: AuthenticationModel;
  //#endregion
  isLinear = false;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  stationManagmentInsertModel: StationManagmentInsertModel = new StationManagmentInsertModel();
  contentLanguageList: ContentLanguageModel[] = [];
  contentInputs: StationManagmentContentInputModel[] = [];
  selectedPaymentMethods: number[] = [];
  selectedTypesOfUsing: number[] = [];
  stationInsertFormPrepareModel: StationInsertFormPrepare = new StationInsertFormPrepare();

  // Google Maps değişkenleri
  public lat: number;
  public lng: number;
  zoom: number = 10;

  //#region  firmaları tutan model tanımlanıyor
  companyList: CompanyListItemModel[] = [];
  companyFilter: CompanyFilterModel = new CompanyFilterModel();
  //#endregion
  //#region  station type enum
  stationManagmentTypeMapping = stationManagmentTypeMapping;
  stationManagmentTypes = Object.values(StationManagmentType).filter(value => typeof value === 'string');
  stationManagmentType = StationManagmentType;
  //#endregion
  //#region  ülke,il,ilçe selecti — cascading dropdown
  countryList: CountryModel[] = [];
  cityList: CityModel[] = [];
  townList: TownModel[] = [];
  cityFilter: CityFilterModel = new CityFilterModel();
  townFilter: TownFilterModel = new TownFilterModel();
  //#endregion

  @ViewChild('addStationPictureDropzone') addStationPictureDropzone?: PixdinnDropzoneComponent;
  imageUrl: string;

  constructor(private _formBuilder: FormBuilder,
    private router: Router,
    private srvContentLanguage: ContentLanguageService,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvStationManagment: StationManagmentService,
    private srvCountryCityAndTown: CountryCityAndTownService,
    private srvCompanyService: CompanyService,
    private srvUtils: UtilsService,
    public gMaps: GoogleMapsAPIWrapper,   // AGM Core inject
    private subheader: SubHeaderService
  ) {
    this.imageUrl = environment.imageUrl;
    this.stationManagmentInsertModel.stationManagmentContent = [];
    this.stationManagmentInsertModel.stationManagmentPicture = [];
    this.stationManagmentInsertModel.stationPaymentMethod = [];
    this.stationManagmentInsertModel.stationTypesOfUsing = [];
  }

  ngAfterViewInit(): void {
    this.addStationPictureListener();
  }

  getCompanies() {
    this.srvCompanyService.list(this.companyFilter).subscribe((response: CompanyListItemModel[]) => {
      this.companyList = response;
    }, (error) => {});
  }

  addStationPictureListener() {
    this.addStationPictureDropzone.onFileSelected = (onSelected: FileDropZoneData) => {
      this.sendStationFileToServer(onSelected);
    }
  }

  sendStationFileToServer(onSelected: FileDropZoneData) {
    this.srvStationManagment.addFile(onSelected).subscribe((response: StationManagmentFileUploadResponseModel) => {
      onSelected.downloadFunction = (item: FileDropZoneData) => {
        window.location.href = item.absoluteFileUrl;
      };
      this.addStationPictureDropzone.onFileUploaded(this.imageUrl + response.fileKey + "&&group=" + 1, onSelected.index, response.fileKey);
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.onErrorAddStationFile(error, onSelected);
    });
  }

  onErrorAddStationFile(error: any, selected: FileDropZoneData) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showServerError(errorData);
    selected.reUploadFunction = (item: FileDropZoneData) => {
      this.addStationPictureDropzone.onFileUploadInit(selected);
      this.sendStationFileToServer(selected);
    };
    this.addStationPictureDropzone.onFileUploadedError(selected.index, errorData.errorMessage);
  }

  ngOnInit() {
    this.admin = JSON.parse(localStorage.getItem("auth_token"));
    this.subheader.setBreadcrumbs([
      { title: 'Ana Sayfa', linkPath: ``, linkText: '', isActive: true },
      { title: 'İstasyon Yönetimi', linkPath: null, linkText: '', isActive: false },
      { title: 'İstasyon Listesi', linkPath: '/stations/list', linkText: '', isActive: true },
      { title: 'İstasyon Ekle', linkPath: null, linkText: '', isActive: false },
    ]);
    this.stationManagmentInsertModel.stationManagmentAddress = new StationManagmentAddressModel();
    this.stationManagmentInsertModel.stationManagmentDetail = new StationManagmentDetailModel();
    this.stationManagmentInsertModel.stationReservationSuitableDate = new StationReservationSuitableDateModel();
    this.stationManagmentInsertModel.priceOfParkArea = 0;
    this.stationManagmentInsertModel.stationManagmentAddress.distanceFromRoute = 15;
    this.stationManagmentInsertModel.stationManagmentDetail.reservationMinuteFlexibility = 10;
    this.stationManagmentInsertModel.stationManagmentDetail.availableReservationAfterChargeMinute = 180;
    this.stationManagmentInsertModel.stationManagmentDetail.chargeProcessTempTimeOutMinute = 10;
    this.stationManagmentInsertModel.stationManagmentDetail.lockingForReservationMinute = 10;
    this.stationManagmentInsertModel.stationManagmentDetail.reservationAdditionalMinuteForCharge = 5;
    this.stationManagmentInsertModel.stationManagmentDetail.reservationTempTimeOutMinute = 10;
    this.stationManagmentInsertModel.companyId = this.admin.companyId;
    this.companyFilter.id = this.admin.companyId;
    this.stationInsertFormPrepare();
    this.getContentLanguageList();
    this.setCurrentPosition();
    this.getCountry();
    this.getCompanies();
  }

  // Varsayılan harita pozisyonu (Türkiye merkezi)
  private setCurrentPosition() {
    this.lat = 39.05080959919167;
    this.lng = 34.306163387500014;
  }

  // Haritada marker sürüklendiğinde koordinatları günceller
  markerDragEnd($event: MouseEvent) {
    this.lat = $event.coords.lat;
    this.lng = $event.coords.lng;
    this.stationManagmentInsertModel.stationManagmentAddress.latitude = $event.coords.lat + "";
    this.stationManagmentInsertModel.stationManagmentAddress.longtitude = $event.coords.lng + "";
  }

  stationInsertFormPrepare() {
    this.srvProgressSpinner.show();
    this.srvStationManagment.prepareInsertForm().subscribe((response: StationInsertFormPrepare) => {
      this.stationInsertFormPrepareModel = response;
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  getContentLanguageList() {
    this.srvProgressSpinner.show();
    this.srvContentLanguage.list().subscribe((response: ContentLanguageModel[]) => {
      this.contentLanguageList = response;
      this.contentLanguageList.map((item) => {
        this.contentInputs.push({
          stationManagmentContentId: 0,
          stationManagmentId: 0,
          contentLanguageId: item.id,
          description: item.description,
          content: item.content,
          stationDescription: "",
          stationName: "",
          isDefault: item.isDefault
        });
      })
      this.srvProgressSpinner.hide();
    }, (error) => {
      this.srvProgressSpinner.hide();
    });
  }

  save() {
    this.srvProgressSpinner.show();
    this.stationManagmentInsertModel.id = 0;
    this.stationManagmentInsertModel.stationManagmentAddress.latitude = this.lat + "";
    this.stationManagmentInsertModel.stationManagmentAddress.longtitude = this.lng + "";
    this.selectedPaymentMethods.map((item) => {
      this.stationManagmentInsertModel.stationPaymentMethod.push(
        { stationManagmentId: 0, paymentMethodId: item, paymentMethod: null, id: 0 }
      );
    });
    this.selectedTypesOfUsing.map((item) => {
      this.stationManagmentInsertModel.stationTypesOfUsing.push(
        { stationManagmentId: 0, typesOfUsingId: item, typesOfUsing: null, id: 0 }
      );
    });
    this.contentInputs.map((item) => {
      this.stationManagmentInsertModel.stationManagmentContent.push({
        name: item.stationName,
        description: item.stationDescription,
        contentLanguageId: item.contentLanguageId,
        stationManagmentId: this.stationManagmentInsertModel.id,
        id: 0
      });
    })
    this.addStationPictureDropzone.fileData.map((item) => {
      this.stationManagmentInsertModel.stationManagmentPicture.push({
        id: 0,
        stationManagmentId: 0,
        mediaGuiId: item.guid,
      });
    })
    this.stationManagmentInsertModel.isActive = true;
    this.srvStationManagment.add(this.stationManagmentInsertModel).subscribe((response: any) => {
      this.srvUtils.showActionNotification("Başarıyla Eklendi", EnumMessageType.Success, 8000);
      setTimeout(() => {
        this.router.navigate(['stations/list']);
      }, 2500);
    }, (error) => {
      this.stationManagmentInsertModel.stationManagmentContent = [];
      this.stationManagmentInsertModel.stationManagmentPicture = [];
      this.srvProgressSpinner.hide();
    });
  }

  // Cascading ülke->il->ilçe dropdown'ları
  getCountry() {
    this.srvCountryCityAndTown.countryList().subscribe((response: CountryModel[]) => {
      this.countryList = response;
    }, (error) => {});
  }

  changeCountrySelect(event) {
    this.stationManagmentInsertModel.stationManagmentAddress.countryId = event.value.id;
    this.stationManagmentInsertModel.stationManagmentAddress.countryName = event.value.countryName;
    this.cityFilter.countryId = event.value.id;
    this.srvCountryCityAndTown.cityList(this.cityFilter).subscribe((response: CityModel[]) => {
      this.cityList = [];
      this.cityList = response;
    }, (error) => {});
  }

  changeCitySelect(event) {
    this.stationManagmentInsertModel.stationManagmentAddress.cityId = event.value.id;
    this.stationManagmentInsertModel.stationManagmentAddress.cityName = event.value.cityName;
    this.townFilter.cityId = event.value.id;
    this.srvCountryCityAndTown.townList(this.townFilter).subscribe((response: TownModel[]) => {
      this.townList = [];
      this.townList = response;
    }, (error) => {});
  }

  changeTownSelect(event) {
    this.stationManagmentInsertModel.stationManagmentAddress.townId = event.value.id;
    this.stationManagmentInsertModel.stationManagmentAddress.townName = event.value.townName;
  }
}
