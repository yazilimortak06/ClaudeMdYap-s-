// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\services\contentLanguage\contentLanguage-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { ContentLanguageModel } from "../../models/contentLanguage/contentLanguage-model";

@Injectable()
export class ContentLanguageService {

    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "ContentLanguage/";
    }

    list(): Observable<ContentLanguageModel[]> {
        return this.http.post<ContentLanguageModel[]>(this.srvUrl + "list", "");
    }

    listForPanel(searchCriter: DatatableRequestWrapper<any>): Observable<DatatableResponseWrapper<ContentLanguageModel[]>> {
        return this.http.post<DatatableResponseWrapper<ContentLanguageModel[]>>(this.srvUrl + "ListForPanel", searchCriter);
    }

    setDefault(setDefaultModel: any): Observable<any> {
        return this.http.put<any>(this.srvUrl + "SetDefault", setDefaultModel);
    }
}
