// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\charge-device-ocpp-state.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { ChargeDeviceOcppState } from 'src/app/evtech/enums/chargeDevice/charge-device-ocpp-state-enum';

@Pipe({ name: 'chargeDeviceOcppState' })
export class ChargeDeviceOcppStatePipe implements PipeTransform {
    transform(value: ChargeDeviceOcppState): string {
        if (value == ChargeDeviceOcppState.AVAILABLE) return "Uygun";
        else if (value == ChargeDeviceOcppState.UNAVAILABLE) return "Uygun Degil";
        else if (value == ChargeDeviceOcppState.FAULTED) return "Arizali";
        else return "Uygun Degil";
    }
}
