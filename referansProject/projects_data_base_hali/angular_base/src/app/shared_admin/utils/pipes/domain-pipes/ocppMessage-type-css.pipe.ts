// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\ocppMessage-type-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { OcppMessageType } from "src/app/evtech/enums/chargeDevice/chargeDevice-type-enum";

@Pipe({ name: 'ocppMessageTypeCss' })
export class OcppMessageTypeCssPipe implements PipeTransform {
    transform(value: OcppMessageType): string {
        if (value == OcppMessageType.RECEIVED) return 'info';
        else if (value == OcppMessageType.SENDER) return 'primary';
        else return "danger";
    }
}
