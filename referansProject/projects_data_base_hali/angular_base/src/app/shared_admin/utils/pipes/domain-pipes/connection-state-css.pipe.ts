// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connection-state-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'connectionStateCss' })
export class ConnectionStateCssPipe implements PipeTransform {
  transform(value: boolean): string {
    if (value == true) return "success";
    else if (value == false) return "danger";
    else return "default";
  }
}
