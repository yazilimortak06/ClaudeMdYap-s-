// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\connector\connector-state-enum.ts
export enum ConnectorState {
    ACTIVE = "ACTIVE",
    IN_CARE = "IN_CARE",
    DEFECTIVE = "DEFECTIVE"
}

export const connectorStateMapping: Record<keyof typeof ConnectorState, string> = {
    ACTIVE: "Aktif",
    IN_CARE: "Bakimda",
    DEFECTIVE: "Arizali"
};
