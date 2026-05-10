// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\userPayment-bank-type.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { UserPaymentBankType } from 'src/app/evtech/enums/payments/userPayment-type';

@Pipe({ name: 'userPaymentBankType' })
export class UserPaymentBankTypePipe implements PipeTransform {
    transform(value: UserPaymentBankType): string {
        if (value == UserPaymentBankType.MOKA) return "Moka";
        else if (value == UserPaymentBankType.IYZICO) return "Iyzico";
        else if (value == UserPaymentBankType.PARAM) return "Param";
        else return "";
    }
}
