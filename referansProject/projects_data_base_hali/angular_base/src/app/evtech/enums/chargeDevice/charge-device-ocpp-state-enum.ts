// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\chargeDevice\charge-device-ocpp-state-enum.ts
export enum ChargeDeviceOcppState {
    AVAILABLE = "AVAILABLE",
    UNAVAILABLE = "UNAVAILABLE",
    FAULTED = "FAULTED"
}

export const chargeDeviceOcppStateMapping: Record<keyof typeof ChargeDeviceOcppState, string> = {
    AVAILABLE: "Uygun",
    UNAVAILABLE: "Uygun Degil",
    FAULTED: "Arizali"
};
