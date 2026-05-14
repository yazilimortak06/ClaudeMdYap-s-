// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\chargeDevice-type-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDevicePowerType } from "src/app/evtech/enums/chargeDevice/chargeDevice-type-enum";

@Pipe({ name: 'chargeDevicePowerTypeCss' })
export class ChargeDevicePowerTypeCssPipe implements PipeTransform {
    transform(value: number): string {
        if (value == ChargeDevicePowerType.AC) return 'info';
        else if (value == ChargeDevicePowerType.DC) return 'primary';
        else return "danger";
    }
}
