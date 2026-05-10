// Kaynak: E:\Projeler\Angular\PixdinnYonetimPanel\PixdinnManagementPanel\src\app\pixdinnRestaurantSystem\services\generalSystem\product\product-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AddProductRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/add-product-request-model";
import { AddProductResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/add-product-response-model";
import { ChangeStateProductRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/change-state-product-request-model";
import { ChangeStateProductResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/change-state-product-response-model";
import { PrepareProductInsertFormRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/prepare-product-insert-form-request-model";
import { PrepareProductInsertFormResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/prepare-product-insert-form-response-model";
import { PrepareProductUpdateFormRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/prepare-product-update-form-request-model";
import { PrepareProductUpdateFormResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/prepare-product-update-form-response-model";
import { UpdateProductRequestModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/update-product-request-model";
import { UpdateProductResponseModel } from "src/app/pixdinnRestaurantSystem/models/generalSystem/product/update-product-response-model";
import { environment } from "src/environments/environment";


@Injectable()
export class ProductService {

  private srvUrl: string;

  constructor(private http: HttpClient) {
    this.srvUrl = environment.apiUrl + "ProductManagment/";
  }

  add(request: AddProductRequestModel): Observable<AddProductResponseModel> {
    return this.http.post<AddProductResponseModel>(this.srvUrl + "AddProduct", request);
  }

  update(request: UpdateProductRequestModel): Observable<UpdateProductResponseModel> {
    return this.http.post<UpdateProductRequestModel>(this.srvUrl + "UpdateProduct", request);
  }

  prepareProductInsertForm(request: PrepareProductInsertFormRequestModel): Observable<PrepareProductInsertFormResponseModel> {
    return this.http.post<PrepareProductInsertFormResponseModel>(this.srvUrl + "PrepareProductInsertForm", request);
  }

  changeStateProduct(request: ChangeStateProductRequestModel): Observable<ChangeStateProductResponseModel> {
    return this.http.post<ChangeStateProductResponseModel>(this.srvUrl + "ChangeStateProduct", request);
  }

  prepareProductUpdateForm(request: PrepareProductUpdateFormRequestModel): Observable<PrepareProductUpdateFormResponseModel> {
    return this.http.post<PrepareProductUpdateFormResponseModel>(this.srvUrl + "PrepareProductUpdateForm", request);
  }
}
