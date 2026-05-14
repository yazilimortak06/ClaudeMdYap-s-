// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\pipes\first-letter.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'firstLetter' })
export class FirstLetterPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return value.split(' ').map((n) => n[0]).join('');
  }
}
