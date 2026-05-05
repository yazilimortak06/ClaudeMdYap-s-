import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
  name: 'dateFormat'
})
export class DateFormatPipe implements PipeTransform {
  transform(
    value: string | Date | null | undefined,
    format = 'DD.MM.YYYY HH:mm',
    locale = 'tr'
  ): string {
    if (!value) return '-';
    return moment(value).locale(locale).format(format);
  }
}
