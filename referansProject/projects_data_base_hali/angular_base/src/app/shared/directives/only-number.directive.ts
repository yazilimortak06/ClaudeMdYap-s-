import { Directive, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appOnlyNumber]'
})
export class OnlyNumberDirective {
  @Input() allowDecimal = false;
  @Input() allowNegative = false;

  @HostListener('keypress', ['$event'])
  onKeyPress(event: KeyboardEvent): boolean {
    const charCode = event.which || event.keyCode;
    const char = String.fromCharCode(charCode);

    if (this.allowDecimal && (char === '.' || char === ',')) {
      const input = event.target as HTMLInputElement;
      if (input.value.includes('.') || input.value.includes(',')) {
        return false;
      }
      return true;
    }

    if (this.allowNegative && char === '-') {
      const input = event.target as HTMLInputElement;
      if (input.selectionStart === 0 && !input.value.includes('-')) {
        return true;
      }
      return false;
    }

    return charCode >= 48 && charCode <= 57;
  }

  @HostListener('paste', ['$event'])
  onPaste(event: ClipboardEvent): void {
    const clipboardData = event.clipboardData;
    const pastedText = clipboardData?.getData('text') || '';
    const pattern = this.allowDecimal ? /^-?\d*[.,]?\d*$/ : /^-?\d*$/;
    if (!pattern.test(pastedText)) {
      event.preventDefault();
    }
  }
}
