// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\wallet\wallet-pull-money-state.ts
export enum WalletPullMoneyState {
    WAITING = "WAITING",
    COMPLETED = "COMPLETED",
    CANCELLED = "CANCELLED"
}

export const walletPullMoneyStateMapping: Record<keyof typeof WalletPullMoneyState, string> = {
    WAITING: "Bekliyor",
    COMPLETED: "Tamamlandi",
    CANCELLED: "Iptal Edildi"
};
