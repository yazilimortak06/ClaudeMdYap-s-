// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\action-natification\action-notification.component.ts
import { Component, Inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { delay } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
    selector: 'action-natification',
    templateUrl: './action-notification.component.html',
    styleUrls: ['./action-notification.component.scss'],
    changeDetection: ChangeDetectionStrategy.Default
})
export class ActionNotificationComponent implements OnInit {
    constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) { }

    ngOnInit() {
        if (!this.data.showUndoButton || (this.data.undoButtonDuration >= this.data.duration)) { return; }
        this.delayForUndoButton(this.data.undoButtonDuration).subscribe(() => {
            this.data.showUndoButton = false;
        });
    }

    delayForUndoButton(timeToDelay) {
        return of('').pipe(delay(timeToDelay));
    }

    public onDismissWithAction() { this.data.snackBar.dismiss(); }
    public onDismiss() { this.data.snackBar.dismiss(); }
}
