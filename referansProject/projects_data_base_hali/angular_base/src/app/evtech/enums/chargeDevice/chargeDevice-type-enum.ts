// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\chargeDevice\chargeDevice-type-enum.ts
export enum ChargeDeviceState {
    ACTIVE = "ACTIVE",
    IN_CARE = "IN_CARE",
    DEFECTIVE = "DEFECTIVE"
}

export const chargeDeviceStateMapping: Record<keyof typeof ChargeDeviceState, string> = {
    ACTIVE: "Aktif",
    IN_CARE: "Bakimda",
    DEFECTIVE: "Arizali"
};

export enum ChargeDevicePowerType {
    AC = 1,
    DC = 2
}

export const chargeDevicePowerTypeMapping: Record<keyof typeof ChargeDevicePowerType, string> = {
    AC: "Ac",
    DC: "Dc"
};

export enum ChargeDeviceSocketType {
    NORMAL = 1
}

export const chargeDeviceSocketTypeMapping: Record<keyof typeof ChargeDeviceSocketType, string> = {
    NORMAL: ""
};

export enum OcppMark {
    ABB = 1,
    PIWIN = 2
}

export const ocppMarkMapping: Record<keyof typeof OcppMark, string> = {
    ABB: "Abb",
    PIWIN: "Piwin"
};

export enum OcppMessageType {
    SENDER = "SENDER",
    RECEIVED = "RECEIVED"
}

export const ocppMessageTypeMapping: Record<keyof typeof OcppMessageType, string> = {
    SENDER: "Gonderilen",
    RECEIVED: "Alinan"
};
