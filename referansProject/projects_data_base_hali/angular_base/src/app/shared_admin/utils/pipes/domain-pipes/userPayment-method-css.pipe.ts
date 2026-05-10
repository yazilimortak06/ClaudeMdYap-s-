// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\userPayment-method-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { UserPaymentMethod } from "src/app/evtech/enums/payments/userPayment-method";

@Pipe({ name: 'userPaymentMethodCss' })
export class UserPaymentMethodCssPipe implements PipeTransform {
    transform(value: UserPaymentMethod): string {
        if (value == UserPaymentMethod.DEBIT_CARD) return 'success';
        else if (value == UserPaymentMethod.WALLET) return 'info';
        return "danger";
    }
}
