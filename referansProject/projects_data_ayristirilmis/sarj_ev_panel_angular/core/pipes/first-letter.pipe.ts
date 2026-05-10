// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\pipes\first-letter.pipe.ts
// Pattern: FirstLetter Pipe — her kelimenin ilk harfini alarak kısaltma oluşturur (örn: avatar initials için)

import { Pipe, PipeTransform } from '@angular/core';

/**
 * Returns only first letter of string
 */
@Pipe({
  name: 'firstLetter',
})
export class FirstLetterPipe implements PipeTransform {
  /**
   * Transform
   *
   * @param value: any
   * @param args: any
   */
  transform(value: any, args?: any): any {
    return value
      .split(' ')
      .map((n) => n[0])
      .join('');
  }
}
