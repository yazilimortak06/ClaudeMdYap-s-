// Kaynak: E:\Projeler\Angular\PixdinnYonetimPanel\PixdinnManagementPanel\src\app\pixdinnRestaurantSystem\components\generalSystem\product\product-add\product-add.component.ts
import { GoogleMapsAPIWrapper, MapsAPILoader, MouseEvent } from '@agm/core';
import { Component, EventEmitter, Inject, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { ProgressSpinnerService } from "src/app/shared_admin/partials/dialogs/progress-spinner/progress-spinner.service";
import { SubHeaderService } from 'src/app/shared_admin/partials/subheader/_services/subheader.service';
import { EnumMessageType } from 'src/app/shared_admin/utils/enums/message-type.enum';
import { UtilsService } from "src/app/shared_admin/utils/services/utils.service";
import { environment } from "src/environments/environment";
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { T } from '@angular/cdk/keycodes';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { ProductCategoryInsertModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/product-category/product-category-insert-model';
import { AddProductRequestModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/product/add-product-request-model';
import { PrepareProductInsertFormRequestModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/product/prepare-product-insert-form-request-model';
import { PrepareProductInsertFormResponseModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/product/prepare-product-insert-form-response-model';
import { ProductContentInsertModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/productContent/product-content-insert-model';
import { ProductPriceFormInputModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/productPrice/product-price-form-input-model';
import { ProductPriceInsertModel } from 'src/app/pixdinnRestaurantSystem/models/generalSystem/productPrice/product-price-insert-model';
import { AllergenFormModel } from 'src/app/pixdinnRestaurantSystem/models/qrMenuSystem/allergen/allergen-form-model';
import { AllergenProductInsertModel } from 'src/app/pixdinnRestaurantSystem/models/qrMenuSystem/allergenProduct/allergen-product-insert-model';
import { TagProductInserModel } from 'src/app/pixdinnRestaurantSystem/models/qrMenuSystem/tagProduct/tag-product-insert-model';
import { ProductService } from 'src/app/pixdinnRestaurantSystem/services/generalSystem/product/product-service';

@Component({
  selector: 'app-product-add',
  templateUrl: './product-add.component.html',
  styleUrls: ['./product-add.component.scss']
})
export class AddProductComponent implements OnInit {
  isLinear = false;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  @ViewChild('stepper') stepper;
  addProductRequestModel: AddProductRequestModel = new AddProductRequestModel();
  //#region  urun ekleme formunu hazirlayan servis icin modeller tanimlaniyor
  prepareProductInsertFormRequestModel: PrepareProductInsertFormRequestModel = new PrepareProductInsertFormRequestModel();
  prepareProductInsertFormResponseModel: PrepareProductInsertFormResponseModel = new PrepareProductInsertFormResponseModel();
  //#endregion
  fileApiUrl: string = environment.fileBaseUrl;
  menuId: number;
  selectedCategoryId;
  isSecondPriceActive: boolean = false;
  productPrices: ProductPriceFormInputModel[] = [];
  selectedAllergens: AllergenFormModel[] = [];

  constructor(
    private _formBuilder: FormBuilder,
    private router: Router,
    private srvProgressSpinner: ProgressSpinnerService,
    private srvProduct: ProductService,
    private srvUtils: UtilsService,
    private subheader: SubHeaderService,
    public dialogRef: MatDialogRef<AddProductComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.addProductRequestModel.productCategory = [];
    this.addProductRequestModel.productContent = [];
    this.addProductRequestModel.productPrice = [];
    this.addProductRequestModel.allergenProduct = [];
    this.addProductRequestModel.tagProduct = [];
    this.addProductRequestModel.isActive = true;

    this.selectedCategoryId = data.categoryId;
    this.prepareProductInsertFormRequestModel.qrMenuId = data.menuId;
  }

  ngAfterViewInit(): void {
  }

  ngOnInit() {
    this.firstFormGroup = this._formBuilder.group({
      isActiveCtrl: ['',],
      productCodeCtrl: ['', Validators.required],
      categoryCtrl: ['', Validators.required],
    });
    this.secondFormGroup = this._formBuilder.group({
      cookingTimeCtrl: ['',],
      calorieCtrl: ['',],
      allergenCtrl: ['',],
      tagCtrl: ['',]
    });
    this.prepareInsertForm();
  }

  prepareInsertForm() {
    this.srvProgressSpinner.show();
    this.srvProduct.prepareProductInsertForm(this.prepareProductInsertFormRequestModel).subscribe((response: PrepareProductInsertFormResponseModel) => {
      this.prepareProductInsertFormResponseModel = response;
      this.prepareProductInsertFormResponseModel.contentLanguageList.map(contentLanguage => {
        let productContentInsertModel = new ProductContentInsertModel();
        productContentInsertModel.name = "";
        productContentInsertModel.description = "";
        productContentInsertModel.languageId = contentLanguage.languageId;
        productContentInsertModel.isDefault = contentLanguage.isDefault;
        if (contentLanguage.isDefault) {
          productContentInsertModel.isManuelTranslate = true;
          productContentInsertModel.languageName = contentLanguage.name + " (Varsayilan)";
        } else {
          productContentInsertModel.isManuelTranslate = false;
          productContentInsertModel.languageName = contentLanguage.name;
        }
        this.addProductRequestModel.productContent.push(productContentInsertModel);
      });
      this.addProductRequestModel.productContent.sort(({ isDefault: stateA = false }, { isDefault: stateB = false }) =>
        Number(stateB) - Number(stateA)
      );
      let productPrice = new ProductPriceFormInputModel();
      productPrice.price = 0;
      productPrice.isActive = true;
      productPrice.order = 1;
      let productPrice2 = new ProductPriceFormInputModel();
      productPrice2.price = 0;
      productPrice2.isActive = false;
      productPrice2.order = 2;
      this.productPrices.push(productPrice);
      this.productPrices.push(productPrice2);
    }, (error) => {
      this.srvProgressSpinner.hide();
      this.dialogRef.close();
      this.onErrorPrepareInsertForm(error);
    });
  }

  onErrorPrepareInsertForm(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  onAdd = new EventEmitter();

  save() {
    if (this.firstFormGroup.valid) {
      this.srvProgressSpinner.show();
      //#region secili kategori modele ekleniyor
      let productCategoryInsertModel = new ProductCategoryInsertModel();
      productCategoryInsertModel.categoryId = this.selectedCategoryId;
      this.addProductRequestModel.productCategory.push(productCategoryInsertModel);
      //#endregion
      //#region  secili alerjenler modele ekleniyor
      this.selectedAllergens.map(allergen => {
        let allergenProductModel = new AllergenProductInsertModel();
        allergenProductModel.allergenId = allergen.id;
        allergenProductModel.text = allergen.text;
        this.addProductRequestModel.allergenProduct.push(allergenProductModel);
      });
      //#endregion
      //#region  fiyatlar modele ekleniyor
      this.productPrices.map(p => {
        let productPrice = new ProductPriceInsertModel();
        productPrice.order = p.order;
        productPrice.price = p.price;
        productPrice.isActive = p.isActive;
        this.addProductRequestModel.productPrice.push(productPrice);
      });
      //#endregion
      this.srvProduct.add(this.addProductRequestModel).subscribe((response: any) => {
        this.srvUtils.showActionNotification("Basariyla Eklendi", EnumMessageType.Success, 8000);
        this.onAdd.emit();
        this.dialogRef.close();
      }, (error) => {
        this.srvProgressSpinner.hide();
        this.onErrorSave(error);
      });
    } else {
      this.srvUtils.showActionNotification("Gerekli Alanlari Doldurun", EnumMessageType.Error, 4000);
    }
  }

  onErrorSave(error: any) {
    let errorData = this.srvUtils.getServerErrorRequest(error);
    this.srvUtils.showActionNotification(errorData.errorMessage, EnumMessageType.Error, 8000);
  }

  formNext() {
    this.stepper.next();
  }

  formPrevious() {
    this.stepper.previous();
  }

  changeProductPrice(ob: MatSlideToggleChange) {
    this.productPrices.filter(x => x.order == 2).map(productPrice => {
      productPrice.isActive = this.isSecondPriceActive;
    })
  }

  //#region  alerji seciminde cagrilan fonksiyon
  onChangeAllergenSelect(event) {
    if (event.isUserInput) {
      if (event.source.selected && this.selectedAllergens.filter(x => x.id == event.source.value).length == 0) {
        let allergen = new AllergenFormModel();
        allergen.id = event.source.value;
        allergen.text = "";
        if (this.prepareProductInsertFormResponseModel.allergenList != null &&
          this.prepareProductInsertFormResponseModel.allergenList.filter(x => x.id == event.source.value).length > 0) {
          allergen.name = this.prepareProductInsertFormResponseModel.allergenList.filter(x => x.id == event.source.value)[0].name;
          allergen.iconUrl = this.prepareProductInsertFormResponseModel.allergenList.filter(x => x.id == event.source.value)[0].iconUrl;
        }
        this.selectedAllergens.push(allergen);
      } else {
        this.selectedAllergens = this.selectedAllergens.filter(x => x.id != event.source.value);
      }
    }
  }
  //#endregion

  //#region  etiket seciminde cagrilan fonksiyon
  onChangeTagSelect(event) {
    if (event.isUserInput) {
      if (event.source.selected && this.addProductRequestModel.tagProduct.filter(x => x.tagId == event.source.value).length == 0) {
        let tag = new TagProductInserModel();
        tag.tagId = event.source.value;
        this.addProductRequestModel.tagProduct.push(tag);
      } else {
        this.addProductRequestModel.tagProduct = this.addProductRequestModel.tagProduct.filter(x => x.tagId != event.source.value);
      }
    }
  }
  //#endregion
}
