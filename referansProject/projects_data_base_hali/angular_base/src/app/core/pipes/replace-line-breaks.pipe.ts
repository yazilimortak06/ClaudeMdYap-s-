// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\pipes\replace-line-breaks.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'replaceLineBreaks' })
export class ReplaceLineBreaks implements PipeTransform {
    transform(value: string): string {
        return value.replace(/\n/g, '<br/>');
    }
}
