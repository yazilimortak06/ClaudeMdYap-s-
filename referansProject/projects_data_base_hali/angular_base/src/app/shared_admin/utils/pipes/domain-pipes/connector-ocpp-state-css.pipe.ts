// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connector-ocpp-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceConnectorOcppState } from "src/app/evtech/enums/chargeDevice/charge-device-connector-ocpp-state-enum";

@Pipe({ name: 'chargeDeviceConnectorOcppStateCss' })
export class ChargeDeviceConnectorOcppStateCssPipe implements PipeTransform {
    transform(value: ChargeDeviceConnectorOcppState): string {
        if (value == ChargeDeviceConnectorOcppState.AVAILABLE) return 'success';
        else if (value == ChargeDeviceConnectorOcppState.CHARGING) return 'info';
        else if (value == ChargeDeviceConnectorOcppState.FAULTED) return 'danger';
        else if (value == ChargeDeviceConnectorOcppState.OCCUPIED) return 'danger';
        else if (value == ChargeDeviceConnectorOcppState.PREPARING) return 'warning';
        else if (value == ChargeDeviceConnectorOcppState.UNAVAILABLE) return 'danger';
        else return "danger";
    }
}
