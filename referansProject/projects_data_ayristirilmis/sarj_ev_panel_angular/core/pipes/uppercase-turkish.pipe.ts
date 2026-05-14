// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\pipes\uppercase-turkish.pipe.ts
// Pattern: Uppercase Turkish Pipe — Türkçe locale'e göre büyük harf dönüşümü (toLocaleUpperCase), null guard

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'uppercaseTurkish'
})

export class UppercaseturkishPipe implements PipeTransform {
    transform(value: string) {
        if (value === null || value === undefined || value == '' ) {
            return value;
        }
        return value.toLocaleUpperCase();
    }
}
