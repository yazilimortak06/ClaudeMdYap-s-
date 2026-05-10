// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\chargeDevice\charge-device-connector-ocpp-state-enum.ts
export enum ChargeDeviceConnectorOcppState {
    AVAILABLE = "AVAILABLE",
    PREPARING = "PREPARING",
    CHARGING = "CHARGING",
    OCCUPIED = "OCCUPIED",
    UNAVAILABLE = "UNAVAILABLE",
    FAULTED = "FAULTED"
}

export const chargeDeviceConnectorOcppStateMapping: Record<keyof typeof ChargeDeviceConnectorOcppState, string> = {
    AVAILABLE: "Uygun",
    PREPARING: "Hazirlaniyor",
    CHARGING: "Sarj Ediyor",
    OCCUPIED: "Mesgul",
    UNAVAILABLE: "Uygun Degil",
    FAULTED: "Arizali"
};
