// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\station-type-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { StationManagmentType } from 'src/app/evtech/enums/stationManagment/stationManagment-type';

@Pipe({ name: 'stationTypeCss' })
export class StationTypeCssPipe implements PipeTransform {
    transform(value: StationManagmentType): string {
        if (value == StationManagmentType.AC) return 'info';
        else if (value == StationManagmentType.DC) return 'primary';
        else if (value == StationManagmentType.ALL) return 'default';
        else return "danger";
    }
}
