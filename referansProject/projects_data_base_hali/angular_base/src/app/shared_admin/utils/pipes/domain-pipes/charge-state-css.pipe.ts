// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\charge-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeState } from "src/app/evtech/enums/charge/charge-state-enum";

@Pipe({ name: 'chargeStateCss' })
export class ChargeStateCssPipe implements PipeTransform {
    transform(value: ChargeState): string {
        if (value == ChargeState.PROCESS_STARTING) return 'warning';
        else if (value == ChargeState.PROCESS_START) return 'info';
        else if (value == ChargeState.PROCESS_ENDING) return 'info';
        else if (value == ChargeState.PAYMENT_WAIT) return 'warning';
        else if (value == ChargeState.PAYMENT_FAIL) return 'danger';
        else if (value == ChargeState.FAILED) return 'danger';
        else if (value == ChargeState.COMPLETED) return 'success';
        else return "danger";
    }
}
