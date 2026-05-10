// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\stationManagment\stationManagment-type.ts
export enum StationManagmentType {
    AC = "AC",
    DC = "DC",
    ALL = "ALL"
}

export const stationManagmentTypeMapping: Record<keyof typeof StationManagmentType, string> = {
    AC: "Ac",
    DC: "Dc",
    ALL: "Tumu"
};
