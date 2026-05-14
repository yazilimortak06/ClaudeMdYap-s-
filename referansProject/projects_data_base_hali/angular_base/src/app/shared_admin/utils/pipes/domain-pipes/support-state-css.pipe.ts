// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\support-state-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { SupportState } from 'src/app/evtech/enums/support/support-state-enum';

@Pipe({ name: 'supportStateCss' })
export class SupportStateCssPipe implements PipeTransform {
    transform(value: SupportState): string {
        if (value == SupportState.WAITING) return 'danger';
        else if (value == SupportState.ANSWERED) return 'primary';
        else if (value == SupportState.CLOSED) return 'success';
        else return "default";
    }
}
