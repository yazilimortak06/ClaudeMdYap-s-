// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\directives\cell-template.directive.ts
import { Directive, Input, TemplateRef } from '@angular/core';

@Directive({
    selector: 'ng-template[cellTemplate]'
})
export class CellTemplateDirective {
    @Input('cellTemplate') name: string;

    constructor(public template: TemplateRef<any>) { }
}
