// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\shared_admin\utils\pipes\domain-pipes\wallet-pull-money-state-css.pipe.ts
import { Pipe, PipeTransform } from "@angular/core";
import { WalletPullMoneyState } from "src/app/evtech/enums/wallet/wallet-pull-money-state";

@Pipe({ name: 'walletPullMoneyStateCss' })
export class WalletPullMoneyStateCssPipe implements PipeTransform {
    transform(value: WalletPullMoneyState): string {
        if (value == WalletPullMoneyState.WAITING) return 'info';
        else if (value == WalletPullMoneyState.COMPLETED) return 'success';
        else return "danger";
    }
}
