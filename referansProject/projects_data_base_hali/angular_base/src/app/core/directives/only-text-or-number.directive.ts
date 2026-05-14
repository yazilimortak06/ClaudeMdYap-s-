// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\directives\only-text-or-number.directive.ts
/* eslint-disable @angular-eslint/directive-selector */
import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: 'input[textOrNumbersOnly]'
})
export class TextOrNumberDirective {

    private navigationKeys = [
        'Backspace', 'Delete', 'Tab', 'Escape', 'Enter',
        'Home', 'End', 'ArrowLeft', 'ArrowRight', 'Clear', 'Copy', 'Paste',
    ];

    constructor(private _el: ElementRef) { }

    @HostListener('keydown', ['$event'])
    onKeyDown(e: KeyboardEvent) {
        if (this.navigationKeys.indexOf(e.key) > -1) { return; }
        const allowedKeys = [
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','y','z','x','w',
            'ç','ğ','ı','ö','ş','ü',
            'A','B','C','D','E','F','G','H','İ','J','K','L','M','N','O','P','Q','R','S','T','U','V','Y','Z','X','W',
            'Ç','Ğ','I','Ö','Ş','Ü',
            '0','1','2','3','4','5','6','7','8','9'
        ];
        if (!allowedKeys.includes(e.key)) {
            e.preventDefault();
        }
    }
}
