// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\models\authentication\authentication-model.ts
import { PanelAdminUserType } from "../../enums/authentication/panel-admin-user-type-enum";
import { AccessTokenModel } from "./access-token-model";

export class AuthenticationModel {
    token: AccessTokenModel;
    name: string;
    surname: string;
    phone: string;
    mail: string;
    companyName: string;
    connectionId: string;
    companyId: number;
    panelAdminUserType: PanelAdminUserType;
}
