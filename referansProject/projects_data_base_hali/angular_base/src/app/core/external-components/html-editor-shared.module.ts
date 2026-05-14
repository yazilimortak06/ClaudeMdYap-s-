// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\external-components\html-editor-shared.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreUtilsService } from '../services/core-utils.service';
import { PixdinnHtmlEditorComponent } from './pixdinn-html-editor/pixdinn-html-editor.component';
import { NgxEditorModule } from 'ngx-editor';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    PixdinnHtmlEditorComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    NgxEditorModule
  ],
  exports: [PixdinnHtmlEditorComponent],
  providers: [
    CoreUtilsService,
  ]
})
export class HtmlEditorSharedModule { }
