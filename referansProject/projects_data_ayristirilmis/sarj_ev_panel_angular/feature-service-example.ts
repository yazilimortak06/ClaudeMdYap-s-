// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\services\stationManagment\stationManagment-service.ts

import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { FileDropZoneData } from "src/app/core/external-components/models/file-dropzone-model";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { PictureType } from "../../enums/mediaFile/picture-type-enum";
import { MediaFileGroupModel } from "../../models/mediaFile/mediaFile-group-model";
import { StationInsertFormPrepare } from "../../models/stationManagment/station-insert-formPrepare-model";
import { StationManagmentFileUploadResponseModel } from "../../models/stationManagment/station-managment-file-model";
import { StationManagmentFilterModel } from "../../models/stationManagment/stationManagment-filter-model";
import { StationManagmentInsertModel } from "../../models/stationManagment/stationManagment-insert-model";
import { StationManagmentIsActiveUpdateModel } from "../../models/stationManagment/stationManagment-isActive-update-model";
import { StationManagmentListModel } from "../../models/stationManagment/stationManagment-list-model";
import { StationManagmentSelectListModel } from "../../models/stationManagment/stationManagment-select-list-model";
import { StationManagmentUpdateFormModel } from "../../models/stationManagment/stationManagment-update-form-model";
import { StationManagmentUpdateRequestModel } from "../../models/stationManagment/stationManagment-update-request-model";
import { StationPaymentListModel } from "../../models/stationPayment/stationPayment-list-model";
import { StationRemoveRequestModel } from "../../models/stationManagment/station-remove-request-model";

@Injectable()
export class StationManagmentService {

    private srvUrl: string;
    private fileUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "StationManagment/";
    }

    getStationManagmentDataTablePanel(searchCriter: DatatableRequestWrapper<StationManagmentFilterModel>): Observable<DatatableResponseWrapper<StationManagmentListModel[]>> {
        return this.http.post<DatatableResponseWrapper<StationManagmentListModel[]>>(this.srvUrl + "GetStationManagmentDataTablePanel", searchCriter);
    }
    getStationsForSelectList(searchCriter: StationManagmentFilterModel): Observable<StationManagmentSelectListModel[]> {
        return this.http.post<StationManagmentSelectListModel[]>(this.srvUrl + "GetStationsForSelectList", searchCriter);
    }
    getStationManagmentById(id: number): Observable<StationManagmentUpdateFormModel> {
        return this.http.get<StationManagmentUpdateFormModel>(this.srvUrl + "GetStationManagmentById",     {
            params: {
              id: id.toString(),
            }
        });
    }

    update(entity: StationManagmentUpdateRequestModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }
    changeIsActive(setOnAirModel: StationManagmentIsActiveUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeActiveState", setOnAirModel);

    }
    prepareInsertForm(): Observable<StationInsertFormPrepare> {
        return this.http.post<StationInsertFormPrepare>(this.srvUrl + "StationPrepareInsertForm", null);
    }

    add(entity: StationManagmentInsertModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }

    removeStation(entity: StationRemoveRequestModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "RemoveStation", entity);
    }

    addFile(fileDropZone: FileDropZoneData): Observable<StationManagmentFileUploadResponseModel> {

        var formData = new FormData();
        var mediaFileGroup : MediaFileGroupModel[] = [];
        mediaFileGroup.push({
            group : PictureType["preview"],
            compressRate : 0
        });
        mediaFileGroup.push({
            group : PictureType["detailed"],
            compressRate : 70
        });
        formData.append('fileGroup', JSON.stringify(mediaFileGroup));
         formData.append("file", fileDropZone.file,fileDropZone.file.name);

        let headers = new HttpHeaders();
        /** In Angular 5, including the header Content-Type can invalidate your request */
        headers.append('Content-Type', 'multipart/form-data');
        headers.append('Accept', 'application/json');
        return this.http.post<any>(this.srvUrl + "AddStationPicture", formData , {headers: headers});
    }

}
