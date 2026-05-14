// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\external-components\pixdinn-dropzone\pixdinn-dropzone-preview\pixdinn-dropzone-preview.component.ts
import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { NgxDropzonePreviewComponent } from 'ngx-dropzone';
import { DropZoneFiletype } from '../../enums/file-dropzone.enum';
import { FileDropZoneData } from '../../models/file-dropzone-model';

@Component({
  selector: 'custom-dropzone-preview',
  templateUrl: './pixdinn-dropzone-preview.component.html',
  styleUrls: ['./pixdinn-dropzone-preview.component.scss'],
  providers: [
    {
      provide: NgxDropzonePreviewComponent,
      useExisting: CustomDropzonePreviewComponent
    }
  ]
})
export class CustomDropzonePreviewComponent extends NgxDropzonePreviewComponent implements OnInit {

    @Input() fileDropZoneData: FileDropZoneData;
    dropZoneFiletype = DropZoneFiletype;

    constructor(sanitizer: DomSanitizer) {
        super(sanitizer);
    }

    ngOnInit() { }

    downloadFile(event: MouseEvent) {
        event.stopPropagation();
        if (this.fileDropZoneData.downloadFunction != null) {
            this.fileDropZoneData.downloadFunction(this.fileDropZoneData);
        }
    }

    reUpload(event: MouseEvent) {
        event.stopPropagation();
        if (this.fileDropZoneData.reUploadFunction != null) {
            this.fileDropZoneData.reUploadFunction(this.fileDropZoneData);
        }
    }
}
