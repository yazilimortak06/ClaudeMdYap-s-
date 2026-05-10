// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\partials\dialogs\yardim-dialog\yardim-dialog.component.ts
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'yardim-dialog',
    templateUrl: './yardim-dialog.component.html'
})
export class YardimDialogComponent implements OnInit {
    viewLoading: boolean = false;

    constructor(
        public dialogRef: MatDialogRef<YardimDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any
    ) { }

    ngOnInit() { }

    onNoClick(): void { this.dialogRef.close(); }
}
