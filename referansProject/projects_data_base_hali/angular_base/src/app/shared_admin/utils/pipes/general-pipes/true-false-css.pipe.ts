// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\general-pipes\true-false-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'trueFalseCss' })
export class TrueFalseCssPipe implements PipeTransform {
    transform(value: boolean): string {
        if (value == true) return 'success';
        else if (value == false) return 'danger';
        else return "";
    }
}
