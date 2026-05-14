// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\support\support-sender-type-enum.ts
export enum SupportSenderType {
    USER = "USER",
    GUEST_USER = "GUEST_USER",
    ADMIN = "ADMIN"
}

export const supportSenderTypeMapping: Record<keyof typeof SupportSenderType, string> = {
    USER: "Kullanici",
    GUEST_USER: "Misafir Kullanici",
    ADMIN: "Admin"
};
