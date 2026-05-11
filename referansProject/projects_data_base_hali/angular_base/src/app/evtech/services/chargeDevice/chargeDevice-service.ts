// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\services\chargeDevice\chargeDevice-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { ChargeDeviceChangeStateRequestModel } from "../../models/chargeDevice/chargeDevice-change-state-request-model";
import { ChargeDeviceFilterModel } from "../../models/chargeDevice/chargeDevice-filter-model";
import { ChargeDeviceInsertFormPrepareModel } from "../../models/chargeDevice/chargeDevice-insert-formPrepare-model";
import { ChargeDeviceInsertModel } from "../../models/chargeDevice/chargeDevice-insert-model";
import { ChargeDeviceListModel } from "../../models/chargeDevice/chargeDevice-list-model";
import { ChargeDeviceUpdateModel } from "../../models/chargeDevice/chargeDevice-update-model";
import { ChargeDeviceUpdatePriceRequestModel } from "../../models/chargeDevice/chargeDevice-update-price-request-model";

@Injectable()
export class ChargeDeviceService {

    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "ChargeDevice/";
    }

    list(searchCriter: DatatableRequestWrapper<ChargeDeviceFilterModel>): Observable<DatatableResponseWrapper<ChargeDeviceListModel[]>> {
        return this.http.post<DatatableResponseWrapper<ChargeDeviceListModel[]>>(this.srvUrl + "GetChargeDeviceDataTablePanel", searchCriter);
    }

    getChargeDeviceById(id: number): Observable<ChargeDeviceUpdateModel> {
        return this.http.get<ChargeDeviceUpdateModel>(this.srvUrl + "GetChargeDeviceById", { params: { id: id.toString() } });
    }

    update(entity: ChargeDeviceUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }

    changeState(request: ChargeDeviceChangeStateRequestModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeState", request);
    }

    add(entity: ChargeDeviceInsertModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }

    updatePrice(request: ChargeDeviceUpdatePriceRequestModel[]): Observable<any> {
        return this.http.post<any>(this.srvUrl + "UpdateChargeDevicePrice", request);
    }

    prepareInsertForm(): Observable<ChargeDeviceInsertFormPrepareModel> {
        return this.http.post<ChargeDeviceInsertFormPrepareModel>(this.srvUrl + "ChargeDevicePrepareInsertForm", null);
    }
}
