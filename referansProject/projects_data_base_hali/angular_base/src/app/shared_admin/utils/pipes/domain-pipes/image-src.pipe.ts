// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\image-src.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { environment } from 'src/environments/environment';

@Pipe({ name: 'imageSrc' })
export class ImageSrcPipe implements PipeTransform {
    imageUrl = environment.imageUrl;
    transform(value: string): string {
        return this.imageUrl + value + "&&group=1";
    }
}
