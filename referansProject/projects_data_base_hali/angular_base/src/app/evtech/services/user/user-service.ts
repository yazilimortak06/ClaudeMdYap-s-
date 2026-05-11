// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\services\user\user-service.ts
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { UserFilterModel } from "../../models/user/user-filter-model";
import { UserIsActiveUpdateModel } from "../../models/user/user-isActive-update-model";
import { UserIsActiveUpdateRespnseModel } from "../../models/user/user-isActive-update-response-model";
import { UserListModel } from "../../models/user/user-list-model";

@Injectable()
export class UserService {

    private srvUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "User/";
    }

    list(searchCriter: DatatableRequestWrapper<UserFilterModel>): Observable<DatatableResponseWrapper<UserListModel[]>> {
        return this.http.post<DatatableResponseWrapper<UserListModel[]>>(this.srvUrl + "list", searchCriter);
    }

    getUser(searchCriter: any): Observable<UserListModel> {
        return this.http.post<UserListModel>(this.srvUrl + "GetUser", searchCriter);
    }

    changeIsActive(setOnAirModel: UserIsActiveUpdateModel): Observable<UserIsActiveUpdateRespnseModel> {
        return this.http.put<UserIsActiveUpdateRespnseModel>(this.srvUrl + "ChangeActiveState", setOnAirModel);
    }
}
