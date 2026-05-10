// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\announcement\announcement-update-model.ts
import { AnnouncementContentInputModel } from "./announcement-content-input-model";
import { AnnouncementContentModel } from "./announcement-content-model";
import { AnnouncementModel } from "./announcement-model";
import { AnnouncementPictureInputsModel } from "./announcement-picture-inputs-model";
import { AnnouncementPictureModel } from "./announcement-picture-model";

export class AnnouncementUpdateModel {
    announcement: AnnouncementModel;
    announcementContent: AnnouncementContentModel[];
    announcementPicture: AnnouncementPictureModel[];
    announcementContentFormInput: AnnouncementContentInputModel[];
    announcementPictureFormInput: AnnouncementPictureInputsModel[];
    announcementPicturesDeleted: AnnouncementPictureModel[];
}
