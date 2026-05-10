// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\support\support-type-enum.ts
export enum SupportType {
    MULTIPLE_MESSAGE = "MULTIPLE_MESSAGE",
    SINGLE_MESSAGE = "SINGLE_MESSAGE"
}

export const supportTypeMapping: Record<keyof typeof SupportType, string> = {
    MULTIPLE_MESSAGE: "Coklu Mesaj",
    SINGLE_MESSAGE: "Tekli Mesaj",
};
