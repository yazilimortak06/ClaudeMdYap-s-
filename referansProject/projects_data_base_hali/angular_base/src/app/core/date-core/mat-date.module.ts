// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\date-core\mat-date.module.ts
import { NgModule, LOCALE_ID } from '@angular/core';
import { MatMomentDateModule, MomentDateAdapter } from '@angular/material-moment-adapter';
import { MAT_DATE_LOCALE, MAT_DATE_FORMATS, DateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MOMENT_DATE_FORMATS } from '../adapters/MOMENT_DATE_FORMATS';

@NgModule({
    imports: [
        MatDatepickerModule,
        MatMomentDateModule
    ],
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
        { provide: LOCALE_ID, useValue: "tr-TR" },
        { provide: MAT_DATE_FORMATS, useValue: MOMENT_DATE_FORMATS },
        { provide: DateAdapter, useClass: MomentDateAdapter }
    ]
})
export class MatDateModule { }
