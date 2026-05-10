// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\support\support-notification-type-enum.ts
export enum SupportNotificationType {
    CREATE_SUPPORT = "CREATE_SUPPORT",
    USER_SEND_MESSAGE = "USER_SEND_MESSAGE",
    ADMIN_SEND_MESSAGE = "ADMIN_SEND_MESSAGE",
    CLOSE_SUPPORT = "CLOSE_SUPPORT",
    RATE_SUPPORT = "RATE_SUPPORT",
}

export const supportNotificationTypeMapping: Record<keyof typeof SupportNotificationType, string> = {
    CREATE_SUPPORT: "Destek Olusturma",
    USER_SEND_MESSAGE: "Kullanici Mesaji",
    ADMIN_SEND_MESSAGE: "Admin Mesaji",
    CLOSE_SUPPORT: "Destek Kapatma",
    RATE_SUPPORT: "Destek Puanlama"
};
