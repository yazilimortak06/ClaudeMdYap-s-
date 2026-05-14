// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\panel-admin-company-select-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { AdminManagmentType } from 'src/app/evtech/enums/authentication/panel-admin-user-type-enum';

@Pipe({ name: 'panelAdminCompanySelectDisableState' })
export class PanelAdminCompanySelectDisableStatePipe implements PipeTransform {
    transform(value: AdminManagmentType): string {
        if (value == AdminManagmentType.MAIN_ADMIN) return 'true';
        else if (value == AdminManagmentType.COMPANY_ADMIN) return 'false';
        else return "true";
    }
}
