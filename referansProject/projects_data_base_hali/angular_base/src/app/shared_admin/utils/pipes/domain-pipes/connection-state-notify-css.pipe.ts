// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\connection-state-notify-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'connectionStateNotifyCss' })
export class ConnectionStateNotifyCssPipe implements PipeTransform {
    transform(value: boolean): string {
        if (value == true) return "ntf__box_success";
        else if (value == false) return "ntf__box_alert";
        else return "ntf__box_success";
    }
}
