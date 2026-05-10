// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\general-pipes\month-pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'monthToMonthName' })
export class MonthPipe implements PipeTransform {
  transform(value: number): string {
    const months = ['', 'Ocak', 'Subat', 'Mart', 'Nisan', 'Mayis', 'Haziran',
      'Temmuz', 'Agustos', 'Eylul', 'Ekim', 'Kasim', 'Aralik'];
    return months[value] || "";
  }
}
