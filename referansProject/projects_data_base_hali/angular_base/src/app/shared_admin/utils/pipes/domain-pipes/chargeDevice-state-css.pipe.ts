// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\chargeDevice-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceState } from "src/app/evtech/enums/chargeDevice/chargeDevice-type-enum";

@Pipe({ name: 'chargeDeviceStateCss' })
export class ChargeDeviceStateCssPipe implements PipeTransform {
    transform(value: ChargeDeviceState): string {
        if (value == ChargeDeviceState.ACTIVE) return 'success';
        else if (value == ChargeDeviceState.IN_CARE) return 'info';
        else return "danger";
    }
}
