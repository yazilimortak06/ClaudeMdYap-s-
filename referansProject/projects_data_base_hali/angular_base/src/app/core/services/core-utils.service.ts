// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\services\core-utils.service.ts
import { Injectable } from '@angular/core';

@Injectable()
export class CoreUtilsService {
    constructor() { }

    public async readFile(file: File): Promise<string | ArrayBuffer> {
        return new Promise<string | ArrayBuffer>((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = e => { return resolve((e.target as FileReader).result); };
            reader.onerror = e => {
                console.error(`FileReader failed on file ${file.name}.`);
                return reject(null);
            };
            if (!file) {
                console.error('No file to read.');
                return reject(null);
            }
            reader.readAsDataURL(file);
        });
    }

    getMonths(): {} {
        return [
            { name: "Ocak", value: 1 },
            { name: "Subat", value: 2 },
            { name: "Mart", value: 3 },
            { name: "Nisan", value: 4 },
            { name: "Mayis", value: 5 },
            { name: "Haziran", value: 6 },
            { name: "Temmuz", value: 7 },
            { name: "Agustos", value: 8 },
            { name: "Eylul", value: 9 },
            { name: "Ekim", value: 10 },
            { name: "Kasim", value: 11 },
            { name: "Aralik", value: 12 }
        ];
    }
}
