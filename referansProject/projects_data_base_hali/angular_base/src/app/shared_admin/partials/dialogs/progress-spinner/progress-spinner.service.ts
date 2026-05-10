// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\progress-spinner\progress-spinner.service.ts
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ProgressSpinnerState } from './progress-spinner.model';

@Injectable()
export class ProgressSpinnerService {
    private loaderSubject = new Subject<ProgressSpinnerState>();
    progressSpinnerState = this.loaderSubject.asObservable();

    constructor() { }

    show() { this.loaderSubject.next(<ProgressSpinnerState>{ show: true }); }
    hide() { this.loaderSubject.next(<ProgressSpinnerState>{ show: false }); }
}
