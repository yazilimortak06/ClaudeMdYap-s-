// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\directives\only-text.directive.ts
/* eslint-disable @angular-eslint/directive-selector */
import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: 'input[textOnly]'
})
export class TextDirective {

  private navigationKeys = [
    'Backspace', 'Delete', 'Tab', 'Escape', 'Enter',
    'Home', 'End', 'ArrowLeft', 'ArrowRight', 'Clear', 'Copy', 'Paste',
  ];

  constructor(private _el: ElementRef) { }

  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    if (
      this.navigationKeys.indexOf(e.key) > -1 ||
      (e.key === 'a' && e.ctrlKey === true) ||
      (e.key === 'c' && e.ctrlKey === true) ||
      (e.key === 'v' && e.ctrlKey === true) ||
      (e.key === 'x' && e.ctrlKey === true) ||
      (e.key === 'a' && e.metaKey === true) ||
      (e.key === 'c' && e.metaKey === true) ||
      (e.key === 'v' && e.metaKey === true) ||
      (e.key === ',') ||
      (e.key === 'x' && e.metaKey === true)
    ) {
      return;
    }
    if (e.key === ' ' || !isNaN(Number(e.key))) {
      e.preventDefault();
    }
  }
}
