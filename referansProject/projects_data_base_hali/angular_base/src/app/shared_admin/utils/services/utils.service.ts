// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\services\utils.service.ts
import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActionNotificationComponent } from '../../partials/dialogs/action-natification/action-notification.component';
import { EvetHayirDialogComponent } from '../../partials/dialogs/evet-hayir-dialog/evet-hayir-dialog.component';
import { GenelSilDialogComponent } from '../../partials/dialogs/genel-sil-dialog/genel-sil-dialog.component';
import { YardimDialogComponent } from '../../partials/dialogs/yardim-dialog/yardim-dialog.component';
import { EnumMessageType } from '../enums/message-type.enum';
import { ServerResultModel } from '../wrapper-models/server.result.model';

@Injectable()
export class UtilsService {
    constructor(private snackBar: MatSnackBar, private dialog: MatDialog) { }

    roundAccurately(number, decimalPlaces) {
        return Number(Math.round(Number(number + "e" + decimalPlaces)) + "e-" + decimalPlaces);
    }

    showActionNotification(
        message: string,
        type: EnumMessageType = EnumMessageType.Info,
        duration: number = 3000,
        showCloseButton: boolean = true,
        showUndoButton: boolean = false,
        undoButtonDuration: number = 3000,
        verticalPosition: 'top' | 'bottom' = 'top',
    ) {
        return this.snackBar.openFromComponent(ActionNotificationComponent, {
            duration: duration,
            panelClass: [
                type == EnumMessageType.Success ? 'green-snackbar' :
                    type == EnumMessageType.Info ? 'blue-snackbar' :
                        type == EnumMessageType.Error ? 'red-snackbar' :
                            type == EnumMessageType.Warning ? 'yellow-snackbar' : 'mat-snack-bar-container'
            ],
            data: {
                message,
                snackBar: this.snackBar,
                showCloseButton,
                showUndoButton,
                undoButtonDuration,
                verticalPosition,
                type,
                action: 'Undo'
            },
            verticalPosition
        });
    }

    generalDeleteDialog(title: string = '', description: string = '', waitDesciption: string = '', service: any, id: number) {
        return this.dialog.open(GenelSilDialogComponent, {
            data: { title, description, waitDesciption, service, id },
            width: '440px'
        });
    }

    yesNoDialog(title: string = '', description: string = '', waitDesciption: string = '') {
        return this.dialog.open(EvetHayirDialogComponent, {
            data: { title, description, waitDesciption },
            width: '440px'
        });
    }

    helperDialog(description: string = '') {
        event.stopPropagation();
        return this.dialog.open(YardimDialogComponent, {
            data: { description },
            width: '440px'
        });
    }

    convertStringListToString(mesajList: []): string {
        let mesaj: string = "<ul>";
        mesajList.forEach((element: string) => { mesaj += "<li>" + element + "</li>"; });
        return mesaj + "</ul>";
    }

    getServerErrorRequest(data: any): ServerResultModel {
        let dataError = data as HttpErrorResponse;
        return dataError.error as ServerResultModel;
    }

    showServerError(data: ServerResultModel) {
        this.showActionNotification(data.errorMessage, EnumMessageType.Error);
    }

    parseSelectValueJson(jsonString) {
        return JSON.parse(jsonString);
    }

    parseBoolean(value): boolean {
        try {
            value = value + "";
            return JSON.parse(value.toLowerCase());
        } catch (err) {
            return false;
        }
    }

    isEmptyOrSpaces(str) {
        return str === null || str.trim() === '';
    }
}
