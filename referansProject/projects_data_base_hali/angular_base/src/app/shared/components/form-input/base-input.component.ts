import {
  Component,
  Input,
  forwardRef,
  OnInit
} from '@angular/core';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  FormControl
} from '@angular/forms';

@Component({
  selector: 'app-base-input',
  template: `
    <mat-form-field [appearance]="appearance" [class]="fieldClass">
      <mat-label *ngIf="label">{{ label }}</mat-label>
      <input
        matInput
        [type]="type"
        [placeholder]="placeholder"
        [formControl]="control"
        [readonly]="readonly"
        (blur)="onTouched()"
      />
      <mat-icon matPrefix *ngIf="prefixIcon">{{ prefixIcon }}</mat-icon>
      <mat-icon matSuffix *ngIf="suffixIcon">{{ suffixIcon }}</mat-icon>
      <mat-hint *ngIf="hint">{{ hint }}</mat-hint>
      <mat-error *ngIf="control.invalid && control.touched">
        {{ getErrorMessage() }}
      </mat-error>
    </mat-form-field>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => BaseInputComponent),
      multi: true
    }
  ]
})
export class BaseInputComponent implements ControlValueAccessor, OnInit {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() type: 'text' | 'password' | 'email' | 'number' = 'text';
  @Input() appearance: 'fill' | 'outline' = 'outline';
  @Input() fieldClass = '';
  @Input() prefixIcon = '';
  @Input() suffixIcon = '';
  @Input() hint = '';
  @Input() readonly = false;
  @Input() errorMessages: { [key: string]: string } = {};

  control = new FormControl('');

  onChange: (value: unknown) => void = () => {};
  onTouched: () => void = () => {};

  ngOnInit(): void {
    this.control.valueChanges.subscribe((value) => {
      this.onChange(value);
    });
  }

  writeValue(value: unknown): void {
    this.control.setValue(value as string, { emitEvent: false });
  }

  registerOnChange(fn: (value: unknown) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    if (isDisabled) {
      this.control.disable();
    } else {
      this.control.enable();
    }
  }

  getErrorMessage(): string {
    const errors = this.control.errors;
    if (!errors) return '';

    if (errors['required']) return this.errorMessages['required'] || 'Bu alan zorunludur.';
    if (errors['email']) return this.errorMessages['email'] || 'Geçerli bir e-posta adresi girin.';
    if (errors['minlength']) return this.errorMessages['minlength'] || `En az ${errors['minlength'].requiredLength} karakter giriniz.`;
    if (errors['maxlength']) return this.errorMessages['maxlength'] || `En fazla ${errors['maxlength'].requiredLength} karakter giriniz.`;

    return 'Geçersiz değer.';
  }
}
