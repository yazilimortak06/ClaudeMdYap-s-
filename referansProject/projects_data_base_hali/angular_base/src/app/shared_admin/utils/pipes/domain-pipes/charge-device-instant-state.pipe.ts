// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\charge-device-instant-state.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { ChargeDeviceInstantState } from 'src/app/evtech/enums/chargeDevice/charge-device-instant-state-enum';

@Pipe({ name: 'chargeDeviceInstantState' })
export class ChargeDeviceInstantStatePipe implements PipeTransform {
    transform(value: ChargeDeviceInstantState): string {
        if (value == ChargeDeviceInstantState.AVAILABLE) return "Uygun";
        else if (value == ChargeDeviceInstantState.UNAVAILABLE) return "Uygun Degil";
        else if (value == ChargeDeviceInstantState.FAULTED) return "Arizali";
        else return "Uygun Degil";
    }
}
