// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\general-pipes\active-passive.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'activePassive' })
export class ActivePassivePipe implements PipeTransform {
  transform(value: boolean): string {
    if (value == true) return "Aktif";
    else if (value == false) return "Pasif";
    else return "";
  }
}
