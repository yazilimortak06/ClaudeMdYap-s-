// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\chargeDevice\charge-device-connector-instant-state-enum.ts
export enum ChargeDeviceConnectorInstantState {
    AVAILABLE = "AVAILABLE",
    UNAVAILABLE = "UNAVAILABLE",
    PREPARING = "PREPARING",
    IN_PROCESS = "IN_PROCESS",
    FAULTED = "FAULTED",
    OCCUPIED = "OCCUPIED"
}

export const chargeDeviceConnectorInstantStateMapping: Record<keyof typeof ChargeDeviceConnectorInstantState, string> = {
    AVAILABLE: "Uygun",
    UNAVAILABLE: "Uygun Degil",
    PREPARING: "Hazirlaniyor",
    IN_PROCESS: "Sarj Ediyor",
    FAULTED: "Arizali",
    OCCUPIED: "Mesgul",
};
