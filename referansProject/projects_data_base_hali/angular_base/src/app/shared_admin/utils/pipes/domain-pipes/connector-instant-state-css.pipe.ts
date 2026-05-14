// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connector-instant-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceConnectorInstantState } from "src/app/evtech/enums/chargeDevice/charge-device-connector-instant-state-enum";

@Pipe({ name: 'chargeDeviceConnectorInstantStateCss' })
export class ChargeDeviceConnectorInstantStateCssPipe implements PipeTransform {
    transform(value: ChargeDeviceConnectorInstantState): string {
        if (value == ChargeDeviceConnectorInstantState.AVAILABLE) return 'success';
        else if (value == ChargeDeviceConnectorInstantState.PREPARING) return 'info';
        else if (value == ChargeDeviceConnectorInstantState.IN_PROCESS) return 'info';
        else if (value == ChargeDeviceConnectorInstantState.FAULTED) return 'danger';
        else if (value == ChargeDeviceConnectorInstantState.OCCUPIED) return 'danger';
        else if (value == ChargeDeviceConnectorInstantState.UNAVAILABLE) return 'danger';
        else return "danger";
    }
}
