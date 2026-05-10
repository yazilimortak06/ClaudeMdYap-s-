// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\subheader\subheader.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { SubheaderComponent } from './subheader-component/subheader.component';

@NgModule({
  declarations: [SubheaderComponent],
  imports: [
    CommonModule,
    RouterModule,
    InlineSVGModule,
    NgbDropdownModule
  ],
  exports: [SubheaderComponent],
})
export class SubheaderModule { }
