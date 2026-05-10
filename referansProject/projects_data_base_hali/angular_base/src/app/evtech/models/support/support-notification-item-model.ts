// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\support\support-notification-item-model.ts
import { SupportNotificationType } from "../../enums/support/support-notification-type-enum";
import { SupportState } from "../../enums/support/support-state-enum";

export class SupportNotificationItemModel {
    supportId: number;
    lastUpdateDate: Date;
    userName: string;
    userSurname: string;
    supportNotificationType: SupportNotificationType;
    summaryTitle: string;
    state: SupportState;
}
