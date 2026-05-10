// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\external-components\dropzone-shared.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { CustomDropzonePreviewComponent } from './pixdinn-dropzone/pixdinn-dropzone-preview/pixdinn-dropzone-preview.component';
import { PixdinnDropzoneComponent } from './pixdinn-dropzone/pixdinn-dropzone.component';
import { CoreUtilsService } from '../services/core-utils.service';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    PixdinnDropzoneComponent,
    CustomDropzonePreviewComponent
  ],
  imports: [
    CommonModule,
    NgxDropzoneModule,
    MatButtonModule
  ],
  exports: [PixdinnDropzoneComponent],
  providers: [
    CoreUtilsService
  ]
})
export class DropzoneSharedModule { }
