// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\chargeDevice-reservation-state-css-pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeDeviceReservationState } from "src/app/evtech/enums/chargeDeviceReservation/chargeDevice-reservation-state-enum";

@Pipe({ name: 'chargeDeviceReservationStateCss' })
export class ChargeDeviceReservationStateCssPipe implements PipeTransform {
    transform(value: ChargeDeviceReservationState): string {
        if (value == ChargeDeviceReservationState.RECEIVED) return 'info';
        else if (value == ChargeDeviceReservationState.COMPLETED) return 'success';
        else if (value == ChargeDeviceReservationState.CANCELLED) return 'danger';
        else if (value == ChargeDeviceReservationState.TIME_EXPIRED) return 'default';
        else return "danger";
    }
}
