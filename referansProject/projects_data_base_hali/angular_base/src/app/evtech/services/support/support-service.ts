// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\services\support\support-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { GetSupportNotificationItemResponseModel } from "../../models/support/get-support-notification-item-response-model";
import { SupportAnswerRequestModel } from "../../models/support/support-answer-request-model";
import { SupportAnswerResponseModel } from "../../models/support/support-answer-response-model";
import { SupportCloseRequestModel } from "../../models/support/support-close-request-model";
import { SupportCloseResponseModel } from "../../models/support/support-close-response-model";
import { SupportDatatableItemModel } from "../../models/support/support-datatable-item-model";
import { SupportDatatableRequestModel } from "../../models/support/support-datatable-request-model";
import { SupportItemModel } from "../../models/support/support-item-model";
import { SupportRequestModel } from "../../models/support/support-request-model";

@Injectable()
export class SupportService {
    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "SupportManagment/";
    }

    listDataTable(searchCriter: DatatableRequestWrapper<SupportDatatableRequestModel>): Observable<DatatableResponseWrapper<SupportDatatableItemModel[]>> {
        return this.http.post<DatatableResponseWrapper<SupportDatatableItemModel[]>>(this.srvUrl + "GetSupportForDatatablePanel", searchCriter);
    }

    list(searchCriter: DatatableRequestWrapper<SupportRequestModel>): Observable<SupportItemModel[]> {
        return this.http.post<SupportItemModel[]>(this.srvUrl + "GetSupportList", searchCriter);
    }

    getSupportDetail(searchCriter: SupportRequestModel): Observable<SupportItemModel> {
        return this.http.post<SupportItemModel>(this.srvUrl + "GetSupportDetail", searchCriter);
    }

    answerSupport(searchCriter: SupportAnswerRequestModel): Observable<SupportAnswerResponseModel> {
        return this.http.post<SupportAnswerResponseModel>(this.srvUrl + "AnswerSupport", searchCriter);
    }

    closeSupport(searchCriter: SupportCloseRequestModel): Observable<SupportCloseResponseModel> {
        return this.http.post<SupportCloseResponseModel>(this.srvUrl + "CloseSupport", searchCriter);
    }

    listForNotification(searchCriter: SupportRequestModel): Observable<GetSupportNotificationItemResponseModel> {
        return this.http.post<GetSupportNotificationItemResponseModel>(this.srvUrl + "GetSupportListForNotification", searchCriter);
    }
}
