// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\services\announcement\announcement-service.ts
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { FileDropZoneData } from "src/app/core/external-components/models/file-dropzone-model";
import { DatatableRequestWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-request-wrapper.model";
import { DatatableResponseWrapper } from "src/app/shared_admin/utils/wrapper-models/datatable-response-wrapper.model";
import { environment } from "src/environments/environment";
import { PictureType } from "../../enums/mediaFile/picture-type-enum";
import { AnnouncementChangeStateUpdateModel } from "../../models/announcement/announcement-changeState-update-model";
import { AnnouncementFileUploadResponseModel } from "../../models/announcement/announcement-file-model";
import { AnnouncementFilterModel } from "../../models/announcement/announcement-filter-model";
import { AnnouncementInsertModel } from "../../models/announcement/announcement-insert-model";
import { AnnouncementListModel } from "../../models/announcement/announcement-list-model";
import { AnnouncementRemoveRequestModel } from "../../models/announcement/announcement-remove-request-model";
import { AnnouncementRemoveResponseModel } from "../../models/announcement/announcement-remove-response-model";
import { AnnouncementUpdateModel } from "../../models/announcement/announcement-update-model";

@Injectable()
export class AnnouncementService {

    private srvUrl: string;
    private fileUrl: string;

    constructor(private http: HttpClient) {
        this.srvUrl = environment.apiUrl + "Announcement/";
        this.fileUrl = environment.fileApiUrl + "File/";
    }

    list(searchCriter: DatatableRequestWrapper<AnnouncementFilterModel>): Observable<DatatableResponseWrapper<AnnouncementListModel[]>> {
        return this.http.post<DatatableResponseWrapper<AnnouncementListModel[]>>(this.srvUrl + "list", searchCriter);
    }

    getAnnouncementById(id: number): Observable<AnnouncementUpdateModel> {
        return this.http.get<AnnouncementUpdateModel>(this.srvUrl + "GetAnnouncementById", { params: { id: id.toString() } });
    }

    update(entity: AnnouncementUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "Update", entity);
    }

    removeAnnouncement(entity: AnnouncementRemoveRequestModel): Observable<AnnouncementRemoveResponseModel> {
        return this.http.post<AnnouncementRemoveResponseModel>(this.srvUrl + "RemoveAnnouncement", entity);
    }

    changeIsActive(changeStateModel: AnnouncementChangeStateUpdateModel): Observable<any> {
        return this.http.put<any>(this.srvUrl + "ChangeActiveState", changeStateModel);
    }

    add(entity: AnnouncementInsertModel): Observable<any> {
        return this.http.post<any>(this.srvUrl + "Add", entity);
    }

    addFile(fileDropZone: FileDropZoneData): Observable<AnnouncementFileUploadResponseModel> {
        var formData = new FormData();
        var mediaFileGroup: any[] = [
            { group: PictureType["preview"], compressRate: 0 },
            { group: PictureType["detailed"], compressRate: 70 }
        ];
        formData.append('fileGroup', JSON.stringify(mediaFileGroup));
        formData.append("file", fileDropZone.file, fileDropZone.file.name);
        let headers = new HttpHeaders();
        headers.append('Content-Type', 'multipart/form-data');
        headers.append('Accept', 'application/json');
        return this.http.post<any>(this.srvUrl + "AddAnnouncementPicture", formData, { headers: headers });
    }
}
