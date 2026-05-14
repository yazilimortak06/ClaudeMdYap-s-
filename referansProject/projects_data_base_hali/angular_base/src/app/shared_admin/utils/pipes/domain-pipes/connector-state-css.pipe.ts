// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connector-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { ConnectorState } from "src/app/evtech/enums/connector/connector-state-enum";

@Pipe({ name: 'connectorStateCss' })
export class ConnectorStateCssPipe implements PipeTransform {
    transform(value: ConnectorState): string {
        if (value == ConnectorState.ACTIVE) return 'success';
        else return "danger";
    }
}
