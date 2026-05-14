// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\progress-spinner\progress-spinner.component.ts
import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { ProgressSpinnerState } from './progress-spinner.model';
import { ProgressSpinnerService } from './progress-spinner.service';

@Component({
    selector: 'angular-progress-spinner',
    templateUrl: 'progress-spinner.component.html',
    styleUrls: ['progress-spinner.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProgressSpinnerComponent implements OnInit {
    show = false;
    private subscription: Subscription;

    constructor(
        private cdr: ChangeDetectorRef,
        public srvProgressSpinner: ProgressSpinnerService
    ) { }

    ngOnInit() {
        this.subscription = this.srvProgressSpinner.progressSpinnerState
            .subscribe((state: ProgressSpinnerState) => {
                this.show = state.show;
                this.cdr.detectChanges();
            });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
