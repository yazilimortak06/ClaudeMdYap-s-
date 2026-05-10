// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\progress-spinner\progress-spinner.module.ts
import { NgModule } from '@angular/core';
import { ProgressSpinnerComponent } from './progress-spinner.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProgressSpinnerService } from './progress-spinner.service';

@NgModule({
    imports: [MatProgressSpinnerModule],
    exports: [ProgressSpinnerComponent, MatProgressSpinnerModule],
    declarations: [ProgressSpinnerComponent],
    providers: [ProgressSpinnerService],
})
export class ProgressSpinnerModule { }
