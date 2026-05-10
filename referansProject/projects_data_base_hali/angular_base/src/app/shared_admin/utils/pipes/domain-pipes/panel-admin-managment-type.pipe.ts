// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\panel-admin-managment-type.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { AdminManagmentType } from 'src/app/evtech/enums/authentication/panel-admin-user-type-enum';

@Pipe({ name: 'panelAdminManagmentType' })
export class PanelAdminManagmentTypePipe implements PipeTransform {
    transform(value: AdminManagmentType): string {
        if (value == AdminManagmentType.MAIN_ADMIN) return 'Genel Admin';
        else if (value == AdminManagmentType.COMPANY_ADMIN) return 'Firma Admin';
        else if (value == AdminManagmentType.TEST_ADMIN_USER) return 'Test Admin';
        else return "";
    }
}
