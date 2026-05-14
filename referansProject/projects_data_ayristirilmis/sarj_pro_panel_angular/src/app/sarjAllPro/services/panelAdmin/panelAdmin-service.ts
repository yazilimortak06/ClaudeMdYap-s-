// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\sarjAllPro\services\panelAdmin\panelAdmin-service.ts

import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { PanelAdminFilterModel } from "../../models/panelAdmin/panelAdmin-filter-model";
import { PanelAdminIsActiveUpdateModel } from "../../models/panelAdmin/panelAdmin-isActive-update-model";
import { PanelAdminListModel } from "../../models/panelAdmin/panelAdmin-list-model";
import { PanelAdminModel } from "../../models/panelAdmin/panelAdmin-model";

@Injectable()
export class PanelAdminService {

    private srvUrl: string;
    private fileUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "PanelAdmin/";
    }

    list(searchCriter: DatatableRequestWrapper<PanelAdminFilterModel>): Observable<DatatableResponseWrapper<PanelAdminListModel[]>> {
        return this.http.post<DatatableResponseWrapper<PanelAdminListModel[]>>(this.srvUrl + "list", searchCriter);
    }

    getPanelAdminTypeById(id: number): Observable<PanelAdminModel> {
        return this.http.get<PanelAdminModel>(this.srvUrl + "GetPanelAdminById", {
            params: {
                id: id.toString(),
            }
        });
    }

    changeIsActive(setOnAirModel: PanelAdminIsActiveUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeActiveState", setOnAirModel);
    }

    update(entity: PanelAdminModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }

    add(entity: PanelAdminModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }
}
