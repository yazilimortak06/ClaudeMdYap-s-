// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\version-puplication-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'versionPuplicationCss' })
export class VersionPuplicationCssPipe implements PipeTransform {
    transform(value: boolean): string {
        if (value == true) return "success";
        else if (value == false) return "danger";
        else return "danger";
    }
}
