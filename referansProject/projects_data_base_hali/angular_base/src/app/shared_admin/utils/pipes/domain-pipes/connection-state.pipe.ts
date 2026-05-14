// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connection-state.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'connectionState' })
export class ConnectionStatePipe implements PipeTransform {
  transform(value: boolean): string {
    if (value == true) return "Bagli";
    else if (value == false) return "Baglanti Yok";
    else return "";
  }
}
