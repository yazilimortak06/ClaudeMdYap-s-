// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\wallet\wallet-process-state.ts
export enum WalletProcessState {
    WAITING = "WAITING",
    COMPLETED = "COMPLETED",
    FAILED = "FAILED"
}

export const walletProcessStateMapping: Record<keyof typeof WalletProcessState, string> = {
    WAITING: "Bekliyor",
    COMPLETED: "Tamamlandi",
    FAILED: "Basarisiz"
};
