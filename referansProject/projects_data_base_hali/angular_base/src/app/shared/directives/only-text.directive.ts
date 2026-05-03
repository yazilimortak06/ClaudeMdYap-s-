import { Directive, HostListener } from '@angular/core';

@Directive({
  selector: '[appOnlyText]'
})
export class OnlyTextDirective {
  @HostListener('keypress', ['$event'])
  onKeyPress(event: KeyboardEvent): boolean {
    const charCode = event.which || event.keyCode;
    // Allow: letters (a-z, A-Z), Turkish chars, space, backspace
    const turkishChars = [
      231, 199, 351, 350, 287, 286, 252, 220, 246, 214, 305, 304
    ];
    if (
      (charCode >= 65 && charCode <= 90) ||
      (charCode >= 97 && charCode <= 122) ||
      charCode === 32 ||
      turkishChars.includes(charCode)
    ) {
      return true;
    }
    return false;
  }

  @HostListener('paste', ['$event'])
  onPaste(event: ClipboardEvent): void {
    const clipboardData = event.clipboardData;
    const pastedText = clipboardData?.getData('text') || '';
    if (/\d/.test(pastedText)) {
      event.preventDefault();
    }
  }
}
