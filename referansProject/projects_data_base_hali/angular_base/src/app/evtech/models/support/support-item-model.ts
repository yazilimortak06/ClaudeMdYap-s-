// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\support\support-item-model.ts
import { SupportState } from "../../enums/support/support-state-enum";
import { SupportType } from "../../enums/support/support-type-enum";
import { SupportMessageItemModel } from "./support-message-item-model";

export class SupportItemModel {
    id: number;
    title: string;
    summaryTitle: string;
    startingDate: Date;
    closingDate: Date;
    lastUpdateDate: Date;
    state: SupportState;
    supportType: SupportType;
    point: number;
    userId: number;
    userName: string;
    userSurname: string;
    selected: boolean = false;
    supportMessage: SupportMessageItemModel[];
}
