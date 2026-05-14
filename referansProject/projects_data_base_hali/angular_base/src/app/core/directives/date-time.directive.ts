// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\directives\date-time.directive.ts
import { Directive, HostListener, forwardRef, Input } from '@angular/core';
import * as moment from 'moment';
import 'moment-timezone';
import { NG_VALUE_ACCESSOR, NgControl } from '@angular/forms';

export const DATE_VALUE_ACCESSOR: any = {
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DateTimeDirective),
    multi: true
};

@Directive({ selector: '[dateTime]' })
export class DateTimeDirective {
    @Input() sendType: string;
    @HostListener('dateChange', ['$event']) onDateChange = (gelen: any) => { this.convertToTurkishTimezone(gelen.target.value); };

    constructor(private control: NgControl) { }

    convertToTurkishTimezone(date: moment.Moment): any {
        if (date == null) { return; }
        if (this.sendType == 'get') {
            var momentDateGet = moment(date).utcOffset(0, false).format();
            this.control.control.setValue(momentDateGet);
        } else {
            var momentDate = moment(date);
            var targetDate = momentDate.clone().tz("Europe/Istanbul").utc().add(momentDate.utcOffset(), 'm').toDate();
            this.control.control.setValue(targetDate);
        }
    };

    registerOnDateChange(fn: () => void): void { this.onDateChange = fn; }
}
