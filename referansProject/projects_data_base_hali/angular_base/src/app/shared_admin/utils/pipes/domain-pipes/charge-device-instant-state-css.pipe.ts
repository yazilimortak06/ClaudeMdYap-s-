// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\charge-device-instant-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceInstantState } from "src/app/evtech/enums/chargeDevice/charge-device-instant-state-enum";

@Pipe({ name: 'chargeDeviceInstantStateCss' })
export class ChargeDeviceInstantStateCssPipe implements PipeTransform {
    transform(value: ChargeDeviceInstantState): string {
        if (value == ChargeDeviceInstantState.AVAILABLE) return 'success';
        else if (value == ChargeDeviceInstantState.UNAVAILABLE) return 'danger';
        else return "danger";
    }
}
