// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\chargeDeviceReservation\chargeDevice-reservation-state-enum.ts
export enum ChargeDeviceReservationState {
    RECEIVED = "RECEIVED",
    CANCELLED = "CANCELLED",
    COMPLETED = "COMPLETED",
    TIME_EXPIRED = "TIME_EXPIRED"
}

export const chargeDeviceReservationStateMapping: Record<keyof typeof ChargeDeviceReservationState, string> = {
    RECEIVED: "Beklemede",
    CANCELLED: "Iptal Edildi",
    COMPLETED: "Tamamlandi",
    TIME_EXPIRED: "Suresi Doldu"
};
