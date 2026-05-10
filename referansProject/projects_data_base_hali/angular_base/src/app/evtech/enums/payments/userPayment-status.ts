// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\payments\userPayment-status.ts
export enum PaymentStatus {
    WAITING = "WAITING",
    SUCCESSFUL = "SUCCESSFUL",
    FAILURE = "FAILURE",
    IN_PROVISION = "IN_PROVISION",
    REFUNDED = "REFUNDED"
}

export const paymentStatusMapping: Record<keyof typeof PaymentStatus, string> = {
    WAITING: "Beklemede",
    SUCCESSFUL: "Basarili",
    FAILURE: "Basarisiz",
    IN_PROVISION: "Provizyonda",
    REFUNDED: "Iade",
};
