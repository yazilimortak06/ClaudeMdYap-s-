// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\authentication\login-response-model.ts
import { PanelAdminUserType } from "../../enums/authentication/panel-admin-user-type-enum";
import { AccessTokenModel } from "./access-token-model";

export class LoginResponseModel {
    name: string;
    surname: string;
    phone: string;
    mail: string;
    companyName: string;
    connectionId: string;
    companyId: number;
    panelAdminUserType: PanelAdminUserType;
    accessToken: AccessTokenModel;
}
