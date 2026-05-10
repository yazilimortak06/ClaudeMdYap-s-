// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\userPayment-method.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { UserPaymentMethod } from 'src/app/evtech/enums/payments/userPayment-method';

@Pipe({ name: 'userPaymentMethod' })
export class UserPaymentMethodPipe implements PipeTransform {
    transform(value: UserPaymentMethod): string {
        if (value == UserPaymentMethod.DEBIT_CARD) return "Kredi Karti";
        else if (value == UserPaymentMethod.WALLET) return "Cuzdan";
        else return "";
    }
}
