// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connector-state.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'connectorState' })
export class ConnectorStatePipe implements PipeTransform {
    transform(value: number): string {
        if (value == 1) return "Aktif";
        else if (value == 2) return "Pasif";
        else return null;
    }
}
