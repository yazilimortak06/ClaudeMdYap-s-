// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\directives\label-control.directive.ts
import { Directive, forwardRef, HostBinding, Input, Optional } from '@angular/core';
import { ControlContainer, NG_VALUE_ACCESSOR } from '@angular/forms';

@Directive({
  selector: 'label[formControlName]',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => LabelControlDirective),
    multi: true
  }]
})
export class LabelControlDirective {
  @Input() formControlName: string;

  constructor(@Optional() private parent: ControlContainer) { }

  @HostBinding('textContent')
  get controlValue() {
    return this.parent ? this.parent.control.get(this.formControlName).value : '';
  }

  writeValue() { }
  registerOnChange() { }
  registerOnTouched() { }
}
