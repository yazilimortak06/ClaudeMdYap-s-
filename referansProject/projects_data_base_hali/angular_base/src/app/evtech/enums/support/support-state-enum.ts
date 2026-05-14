// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\support\support-state-enum.ts
export enum SupportState {
    WAITING = "WAITING",
    ANSWERED = "ANSWERED",
    CLOSED = "CLOSED"
}

export const supportStateMapping: Record<keyof typeof SupportState, string> = {
    WAITING: "Beklemede",
    ANSWERED: "Cevaplandi",
    CLOSED: "Tamamlandi"
};
