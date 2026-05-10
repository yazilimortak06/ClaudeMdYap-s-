// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\payment-status-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { PaymentStatus } from 'src/app/evtech/enums/payments/userPayment-status';

@Pipe({ name: 'paymentStatusCss' })
export class PaymentStatusCssPipe implements PipeTransform {
    transform(value: PaymentStatus): string {
        if (value == PaymentStatus.SUCCESSFUL) return 'success';
        else if (value == PaymentStatus.WAITING) return 'info';
        else if (value == PaymentStatus.IN_PROVISION) return 'primary';
        else if (value == PaymentStatus.REFUNDED) return 'default';
        else if (value == PaymentStatus.FAILURE) return 'danger';
        else return "danger";
    }
}
