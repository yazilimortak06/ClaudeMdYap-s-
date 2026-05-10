// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\genel-sil-dialog\genel-sil-dialog.component.ts
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'genel-sil-dialog',
    templateUrl: './genel-sil-dialog.component.html'
})
export class GenelSilDialogComponent implements OnInit {
    viewLoading: boolean = false;

    constructor(
        public dialogRef: MatDialogRef<GenelSilDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) { }

    ngOnInit() { }

    onNoClick(): void { this.dialogRef.close(); }

    onYesClick(): void {
        this.viewLoading = true;
        if (this.data.service.sil) {
            this.data.service.sil(this.data.id).subscribe((gelen: any) => {
                this.dialogRef.close(gelen);
            });
        } else {
            this.dialogRef.close();
        }
    }
}
