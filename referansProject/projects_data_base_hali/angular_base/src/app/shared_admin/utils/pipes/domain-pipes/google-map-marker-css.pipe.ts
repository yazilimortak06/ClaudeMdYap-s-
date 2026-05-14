// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\google-map-marker-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'googleMapMarkerCss' })
export class GoogleMapMarkerCssPipe implements PipeTransform {
    transform(value: boolean): string {
        if (value == null) return 'unSelectedMarker';
        else if (value) return 'selectedMarker';
        else return 'unSelectedMarker';
    }
}
