// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\external-components\pixdinn-dropzone\pixdinn-dropzone.component.ts
import { NgxDropzoneComponent } from 'ngx-dropzone';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FileDropZoneData } from '../models/file-dropzone-model';
import { DropZoneFiletype, DropZoneTempImage } from '../enums/file-dropzone.enum';
import { CoreUtilsService } from '../../services/core-utils.service';

@Component({
  selector: 'PixdinnDropzone',
  templateUrl: './pixdinn-dropzone.component.html',
  styleUrls: ['./pixdinn-dropzone.component.scss'],
  providers: [
    {
      provide: NgxDropzoneComponent,
      useExisting: PixdinnDropzoneComponent
    }
  ]
})
export class PixdinnDropzoneComponent implements OnInit {

    fileData: FileDropZoneData[] = [];
    fileDataDeleted: FileDropZoneData[] = [];
    @Input() multiple: boolean;
    @Input() isSingleFileSelectable: boolean;
    @Input() accept: string;
    @Input() title: string;
    index: number = 1;
    onFileSelected: any = (fileData: FileDropZoneData) => { };
    @ViewChild('buttonForBug') bugButton: ElementRef<HTMLElement>;

    constructor(private coreUtilService: CoreUtilsService) { }

    initFileData(fileData: FileDropZoneData[]) {
        this.fileData = fileData;
        this.fileData.forEach(item => {
            item.oldfile = true;
            item.index = this.index;
            this.index = this.index + 1;
        });
        this.bugButtonClick();
    }

    ngOnInit(): void { }

    onSelect(event) {
        if (this.isSingleFileSelectable) {
            this.fileData.forEach(item => {
                if (item.fileId != null && item.fileId > 0) {
                    this.fileDataDeleted.push(item);
                }
            });
            this.fileData = [];
        }
        event.addedFiles.forEach(element => {
            let fileDrop = new FileDropZoneData(DropZoneTempImage.Loading, "", this.getfileType(element));
            fileDrop.setFile(element);
            fileDrop.onUploaded = false;
            fileDrop.fileRelativeName = fileDrop.file.name;
            fileDrop.index = this.index;
            fileDrop.oldfile = false;
            this.index = this.index + 1;
            this.fileData.push(fileDrop);
            if (this.onFileSelected != null) {
                this.onFileSelected(fileDrop);
            }
        });
    }

    onFileUploadInit(fileData: FileDropZoneData) {
        fileData.uploadError = false;
        fileData.url = DropZoneTempImage.Loading;
        this.bugButtonClick();
    }

    onFileUploaded(url, index, guid) {
        this.fileData.forEach(item => {
            if (index == item.index) {
                item.onUploaded = true;
                item.guid = guid;
                item.url = this.getFileUrl(item.fileType, url);
                item.absoluteFileUrl = url;
                item.uploadError = false;
                this.bugButtonClick();
                return;
            }
        });
    }

    onFileUploadedError(index, message) {
        this.fileData.forEach(item => {
            if (index == item.index) {
                if (item.fileType == DropZoneFiletype.DropImage) {
                    this.coreUtilService.readFile(item.file).then(fileContents => {
                        item.url = fileContents.toString();
                        item.uploadError = true;
                        item.uploadErrorMessage = message;
                        this.bugButtonClick();
                    });
                } else {
                    item.url = this.getFileUrl(item.fileType, "");
                    item.uploadError = true;
                    item.uploadErrorMessage = message;
                    this.bugButtonClick();
                }
                return;
            }
        });
    }

    bugButtonClick() {
        this.bugButton.nativeElement.click();
    }

    getInsertedFiles() {
        return this.fileData.filter(f => f.oldfile == false && f.onUploaded == true);
    }

    getDeletedFiles(): FileDropZoneData[] {
        return this.fileDataDeleted;
    }

    onRemove(event) {
        let data = event as FileDropZoneData;
        if (data.fileId != null && data.fileId > 0) {
            this.fileDataDeleted.push(data);
        }
        this.fileData.splice(this.fileData.indexOf(event), 1);
    }

    getfileType(file: File): DropZoneFiletype {
        var dropzoneType = DropZoneFiletype.DropOther;
        if (file.type.startsWith("image")) { dropzoneType = DropZoneFiletype.DropImage; }
        else if (file.type.startsWith("application/pdf")) { dropzoneType = DropZoneFiletype.DropPdf; }
        return dropzoneType;
    }

    getFileUrl(type: DropZoneFiletype, url: string): string {
        if (type == DropZoneFiletype.DropImage) { return url; }
        else if (type == DropZoneFiletype.DropPdf) { return DropZoneTempImage.Pdf; }
        else if (type == DropZoneFiletype.DropOther) { return DropZoneTempImage.Other; }
    }
}
