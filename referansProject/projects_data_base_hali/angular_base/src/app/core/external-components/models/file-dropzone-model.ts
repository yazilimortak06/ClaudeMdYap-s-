// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\external-components\models\file-dropzone-model.ts
import { DropZoneFiletype } from "../enums/file-dropzone.enum";

export class FileDropZoneData {
    url: string;
    file: File;
    guid: string;
    fileType: DropZoneFiletype;
    onUploaded: boolean;
    fileRelativeName: string;
    fileId: number;
    index: number;
    oldfile: boolean = false;
    absoluteFileUrl: string;
    uploadError: boolean = false;
    uploadErrorMessage: string;
    reUploadFunction: any = (uploadFile: FileDropZoneData) => { };
    downloadFunction: any = (downloadFile: FileDropZoneData) => { };

    constructor(url: string, guid: string, fileType: DropZoneFiletype) {
        this.url = url;
        this.file = new File([], guid);
        this.fileType = fileType;
    }

    setFile(file: File) {
        this.file = file;
    }

    setRelativeName(relativeName: string) {
        this.fileRelativeName = relativeName;
    }
}
