// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\charge\charge-state-enum.ts
export enum ChargeState {
    PROCESS_STARTING = "PROCESS_STARTING",
    PROCESS_START = "PROCESS_START",
    PROCESS_ENDING = "PROCESS_ENDING",
    PAYMENT_WAIT = "PAYMENT_WAIT",
    PAYMENT_FAIL = "PAYMENT_FAIL",
    COMPLETED = "COMPLETED",
    FAILED = "FAILED",
    CALCULATING = "CALCULATING"
}

export const chargeStateMapping: Record<keyof typeof ChargeState, string> = {
    PROCESS_STARTING: "Islem Baslatiliyor",
    PROCESS_START: "Sarj Oluyor",
    PROCESS_ENDING: "Islem Durduruluyor",
    PAYMENT_WAIT: "Odeme Bekleniyor",
    PAYMENT_FAIL: "Odeme Basarisiz",
    COMPLETED: "Islem Basarili",
    FAILED: "Islem Basarisiz",
    CALCULATING: "Islem Hesaplaniyor"
};
