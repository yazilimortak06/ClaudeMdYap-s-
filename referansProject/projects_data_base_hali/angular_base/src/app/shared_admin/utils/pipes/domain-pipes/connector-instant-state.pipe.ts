// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connector-instant-state.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceConnectorInstantState } from "src/app/evtech/enums/chargeDevice/charge-device-connector-instant-state-enum";

@Pipe({ name: 'chargeDeviceConnectorInstantState' })
export class ChargeDeviceConnectorInstantStatePipe implements PipeTransform {
    transform(value: ChargeDeviceConnectorInstantState): string {
        if (value == ChargeDeviceConnectorInstantState.AVAILABLE) return 'Uygun';
        else if (value == ChargeDeviceConnectorInstantState.PREPARING) return 'Hazirlaniyor';
        else if (value == ChargeDeviceConnectorInstantState.IN_PROCESS) return 'Sarj Ediyor';
        else if (value == ChargeDeviceConnectorInstantState.FAULTED) return 'Arizali';
        else if (value == ChargeDeviceConnectorInstantState.OCCUPIED) return 'Mesgul';
        else if (value == ChargeDeviceConnectorInstantState.UNAVAILABLE) return 'Uygun Degil';
        else return 'Uygun Degil';
    }
}
