/**
 * Custom UI Component Pattern
 *
 * Bu dosya, projede kullanılan özel UI bileşenlerinin nasıl oluşturulduğunu gösterir.
 * Bileşenler, Angular Material üzerine inşa edilmiş ve domain-specific wrapper'lardır.
 *
 * Pattern: ControlValueAccessor implementasyonu ile form entegrasyonu
 */

import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  forwardRef
} from '@angular/core';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  FormControl
} from '@angular/forms';

// ─── ÖRNEK: Generic Input Bileşeni ───────────────────────────────────────────
// Kullanım: <custom-input label="Ad Soyad" formControlName="fullName" />

@Component({
  selector: 'custom-input',
  template: `
    <mat-form-field appearance="outline" class="w-full">
      <mat-label>{{ label }}</mat-label>
      <input
        matInput
        [type]="type"
        [placeholder]="placeholder"
        [formControl]="control"
        (blur)="onTouched()"
      />
      <mat-error *ngIf="control.hasError('required')">Bu alan zorunludur</mat-error>
      <mat-error *ngIf="control.hasError('email')">Geçerli email giriniz</mat-error>
    </mat-form-field>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomInputComponent),
      multi: true
    }
  ]
})
export class CustomInputComponent implements ControlValueAccessor, OnInit {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: string = 'text';

  control = new FormControl('');

  onChange: (value: any) => void = () => {};
  onTouched: () => void = () => {};

  ngOnInit(): void {
    this.control.valueChanges.subscribe(value => {
      this.onChange(value);
    });
  }

  writeValue(value: any): void {
    this.control.setValue(value, { emitEvent: false });
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    isDisabled ? this.control.disable() : this.control.enable();
  }
}

// ─── ÖRNEK: Generic Data Table Bileşeni ──────────────────────────────────────
// Kullanım: <custom-data-table [columns]="cols" [dataSource]="data" />

export interface TableColumn {
  key: string;
  label: string;
  type?: 'text' | 'date' | 'currency' | 'status' | 'action';
  sortable?: boolean;
}

@Component({
  selector: 'custom-data-table',
  template: `
    <div class="table-container">
      <mat-table [dataSource]="dataSource" matSort>

        <ng-container *ngFor="let col of columns" [matColumnDef]="col.key">
          <mat-header-cell *matHeaderCellDef [mat-sort-header]="col.sortable ? col.key : ''">
            {{ col.label }}
          </mat-header-cell>
          <mat-cell *matCellDef="let row">
            <ng-container [ngSwitch]="col.type">
              <span *ngSwitchCase="'date'">{{ row[col.key] | date:'dd.MM.yyyy HH:mm' }}</span>
              <span *ngSwitchCase="'currency'">{{ row[col.key] | currency:'TRY':'symbol-narrow' }}</span>
              <span *ngSwitchCase="'status'" [class]="'status-' + row[col.key]">{{ row[col.key] }}</span>
              <span *ngSwitchDefault>{{ row[col.key] }}</span>
            </ng-container>
          </mat-cell>
        </ng-container>

        <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
        <mat-row *matRowDef="let row; columns: displayedColumns;"></mat-row>

      </mat-table>

      <mat-paginator [pageSizeOptions]="[10, 25, 50]" showFirstLastButtons></mat-paginator>
    </div>
  `
})
export class CustomDataTableComponent implements OnInit {
  @Input() columns: TableColumn[] = [];
  @Input() dataSource: any[] = [];
  @Output() rowClick = new EventEmitter<any>();

  displayedColumns: string[] = [];

  ngOnInit(): void {
    this.displayedColumns = this.columns.map(c => c.key);
  }
}

// ─── ÖRNEK: Generic Select Bileşeni ──────────────────────────────────────────

export interface SelectOption {
  value: any;
  label: string;
  disabled?: boolean;
}

@Component({
  selector: 'custom-select',
  template: `
    <mat-form-field appearance="outline" class="w-full">
      <mat-label>{{ label }}</mat-label>
      <mat-select
        [formControl]="control"
        [multiple]="multiple"
        (selectionChange)="onSelectionChange($event)"
      >
        <mat-option *ngIf="allowClear" [value]="null">-- Seçiniz --</mat-option>
        <mat-option
          *ngFor="let option of options"
          [value]="option.value"
          [disabled]="option.disabled || false"
        >
          {{ option.label }}
        </mat-option>
      </mat-select>
    </mat-form-field>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomSelectComponent),
      multi: true
    }
  ]
})
export class CustomSelectComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() options: SelectOption[] = [];
  @Input() multiple: boolean = false;
  @Input() allowClear: boolean = false;
  @Output() selectionChange = new EventEmitter<any>();

  control = new FormControl(null);

  onChange: (value: any) => void = () => {};
  onTouched: () => void = () => {};

  onSelectionChange(event: any): void {
    this.onChange(event.value);
    this.selectionChange.emit(event.value);
  }

  writeValue(value: any): void {
    this.control.setValue(value, { emitEvent: false });
  }

  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }

  setDisabledState(isDisabled: boolean): void {
    isDisabled ? this.control.disable() : this.control.enable();
  }
}
