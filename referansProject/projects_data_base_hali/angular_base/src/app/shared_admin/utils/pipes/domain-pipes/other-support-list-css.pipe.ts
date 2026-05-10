// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\other-support-list-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'otherSupportListCss' })
export class OtherSupportListCssPipe implements PipeTransform {
    transform(value: boolean): string {
        if (value) return "active_chat";
        else return "";
    }
}
