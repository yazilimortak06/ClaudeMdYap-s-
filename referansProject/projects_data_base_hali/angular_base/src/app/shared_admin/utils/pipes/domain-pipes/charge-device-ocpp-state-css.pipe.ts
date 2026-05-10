// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\charge-device-ocpp-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceOcppState } from "src/app/evtech/enums/chargeDevice/charge-device-ocpp-state-enum";

@Pipe({ name: 'chargeDeviceOcppStateCss' })
export class ChargeDeviceOcppStateCssPipe implements PipeTransform {
    transform(value: ChargeDeviceOcppState): string {
        if (value == ChargeDeviceOcppState.AVAILABLE) return 'success';
        else if (value == ChargeDeviceOcppState.UNAVAILABLE) return 'danger';
        else return "danger";
    }
}
