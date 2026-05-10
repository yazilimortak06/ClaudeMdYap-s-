// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\support\support-request-model.ts
import { SupportState } from "../../enums/support/support-state-enum";

export class SupportRequestModel {
    id: number;
    state: SupportState;
    userName: string;
    userSurname: string;
    exceptId: number;
    startDate: string;
    endDate: string;
}
