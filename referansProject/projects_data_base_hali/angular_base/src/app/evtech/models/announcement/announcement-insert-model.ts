// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\announcement\announcement-insert-model.ts
import { AnnouncementContentModel } from "./announcement-content-model";
import { AnnouncementPictureModel } from "./announcement-picture-model";

export class AnnouncementInsertModel {
    id: number;
    state: boolean;
    validityDate: Date;
    announcementContent: AnnouncementContentModel[];
    announcementPicture: AnnouncementPictureModel[];
}
