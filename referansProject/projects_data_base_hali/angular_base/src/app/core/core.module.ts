// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\core.module.ts
import { LOCALE_ID, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FirstLetterPipe } from './pipes/first-letter.pipe';
import { SafePipe } from './pipes/safe.pipe';
import { CellTemplateDirective } from './directives/cell-template.directive';
import { TextOrNumberDirective } from './directives/only-text-or-number.directive';
import { TextDirective } from './directives/only-text.directive';
import { RemovewhitespacesPipe } from './pipes/remove-whitespaces.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { UppercaseturkishPipe } from './pipes/uppercase-turkish.pipe';
import { NumberDirective } from './directives/only-number.directive';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { MAT_DATE_LOCALE, MAT_DATE_FORMATS, DateAdapter } from '@angular/material/core';
import { MOMENT_DATE_FORMATS } from './adapters/MOMENT_DATE_FORMATS';

@NgModule({
  declarations: [
    FirstLetterPipe,
    SafePipe,
    NumberDirective,
    TextOrNumberDirective,
    TextDirective,
    CellTemplateDirective,
    RemovewhitespacesPipe,
    UppercaseturkishPipe,
    SafeHtmlPipe,
  ],
  imports: [CommonModule],
  exports: [
    FirstLetterPipe,
    SafePipe,
    NumberDirective,
    TextOrNumberDirective,
    TextDirective,
    RemovewhitespacesPipe,
    UppercaseturkishPipe,
    SafeHtmlPipe
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'tr-TR' },
    { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
    { provide: MAT_DATE_FORMATS, useValue: MOMENT_DATE_FORMATS },
    { provide: DateAdapter, useClass: MomentDateAdapter },
  ]
})
export class CoreModule { }
