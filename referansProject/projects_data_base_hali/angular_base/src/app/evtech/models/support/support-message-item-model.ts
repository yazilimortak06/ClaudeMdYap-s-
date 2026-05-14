// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\support\support-message-item-model.ts
import { SupportSenderType } from "../../enums/support/support-sender-type-enum";

export class SupportMessageItemModel {
    id: number;
    message: string;
    senderType: SupportSenderType;
    date: Date;
    supportId: number;
    adminName: string;
}
