// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\pipes\remove-whitespaces.pipe.ts
// Pattern: Remove Whitespaces Pipe — string'den tüm boşlukları kaldırır, null/undefined güvenli

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'removewhitespaces'
})

export class RemovewhitespacesPipe implements PipeTransform {
    transform(value: string, args?: any): string {
        if (value === null || value === undefined) {
            console.log(value);
            return '';
        }
        return value.replace(/ /g, '');
    }
}
