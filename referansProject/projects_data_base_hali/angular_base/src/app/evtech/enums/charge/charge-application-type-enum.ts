// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\charge\charge-application-type-enum.ts
export enum ChargeApplicationType {
    EVTECH = "EVTECH",
    SARJALL = "SARJALL",
}

export const chargeApplicationTypeMapping: Record<keyof typeof ChargeApplicationType, string> = {
    EVTECH: "Evtech",
    SARJALL: "SarjAll",
};
