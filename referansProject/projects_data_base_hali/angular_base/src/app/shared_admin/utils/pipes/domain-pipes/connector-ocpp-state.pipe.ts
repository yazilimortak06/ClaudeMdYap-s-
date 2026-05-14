// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connector-ocpp-state.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceConnectorOcppState } from "src/app/evtech/enums/chargeDevice/charge-device-connector-ocpp-state-enum";

@Pipe({ name: 'chargeDeviceConnectorOcppState' })
export class ChargeDeviceConnectorOcppStatePipe implements PipeTransform {
    transform(value: ChargeDeviceConnectorOcppState): string {
        if (value == ChargeDeviceConnectorOcppState.AVAILABLE) return 'Uygun';
        else if (value == ChargeDeviceConnectorOcppState.CHARGING) return 'Sarj Ediyor';
        else if (value == ChargeDeviceConnectorOcppState.FAULTED) return 'Arizali';
        else if (value == ChargeDeviceConnectorOcppState.OCCUPIED) return 'Mesgul';
        else if (value == ChargeDeviceConnectorOcppState.PREPARING) return 'Hazirlaniyor';
        else if (value == ChargeDeviceConnectorOcppState.UNAVAILABLE) return 'Uygun Degil';
        else return 'Uygun Degil';
    }
}
