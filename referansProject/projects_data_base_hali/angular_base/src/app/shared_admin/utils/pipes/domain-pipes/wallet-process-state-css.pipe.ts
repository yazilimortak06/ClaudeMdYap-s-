// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\wallet-process-state-css.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { WalletProcessState } from 'src/app/evtech/enums/wallet/wallet-process-state';

@Pipe({ name: 'walletProcessStateCss' })
export class WalletProcessStateCssPipe implements PipeTransform {
    transform(value: WalletProcessState): string {
        if (value == WalletProcessState.WAITING) return 'info';
        else if (value == WalletProcessState.COMPLETED) return 'success';
        else if (value == WalletProcessState.FAILED) return 'danger';
        else return "default";
    }
}
