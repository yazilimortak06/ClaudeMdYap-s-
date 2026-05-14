// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\chargeDevice\charge-device-instant-state-enum.ts
export enum ChargeDeviceInstantState {
    AVAILABLE = "AVAILABLE",
    UNAVAILABLE = "UNAVAILABLE",
    FAULTED = "FAULTED"
}

export const chargeDeviceInstantStateMapping: Record<keyof typeof ChargeDeviceInstantState, string> = {
    AVAILABLE: "Uygun",
    UNAVAILABLE: "Uygun Degil",
    FAULTED: "Arizali"
};
