// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\support-type-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { SupportType } from 'src/app/evtech/enums/support/support-type-enum';

@Pipe({ name: 'supportTypeCss' })
export class SupportTypeCssPipe implements PipeTransform {
    transform(value: SupportType): string {
        if (value == SupportType.MULTIPLE_MESSAGE) return 'primary';
        else if (value == SupportType.SINGLE_MESSAGE) return 'info';
        else return "default";
    }
}
