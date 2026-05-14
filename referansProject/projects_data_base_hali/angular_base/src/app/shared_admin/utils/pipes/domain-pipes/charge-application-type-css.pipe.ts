// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\charge-application-type-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ChargeApplicationType } from "src/app/evtech/enums/charge/charge-application-type-enum";

@Pipe({ name: 'chargeApplicationTypeCss' })
export class ChargeApplicationTypeCssPipe implements PipeTransform {
    transform(value: ChargeApplicationType): string {
        if (value == ChargeApplicationType.EVTECH) return 'success';
        else if (value == ChargeApplicationType.SARJALL) return 'info';
        else return "danger";
    }
}
