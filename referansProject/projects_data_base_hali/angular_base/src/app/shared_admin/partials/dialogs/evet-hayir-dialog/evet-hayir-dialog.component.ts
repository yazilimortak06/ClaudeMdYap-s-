// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\evet-hayir-dialog\evet-hayir-dialog.component.ts
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-evet-hayir-dialog',
    templateUrl: './evet-hayir-dialog.component.html'
})
export class EvetHayirDialogComponent implements OnInit {
    viewLoading: boolean = false;

    constructor(
        public dialogRef: MatDialogRef<EvetHayirDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) { }

    ngOnInit() { }

    onNoClick(): void { this.dialogRef.close(); }

    onYesClick(): void {
        this.viewLoading = true;
        this.dialogRef.close(true);
    }
}
