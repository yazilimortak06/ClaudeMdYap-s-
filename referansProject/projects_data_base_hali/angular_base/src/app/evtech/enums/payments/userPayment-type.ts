// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\payments\userPayment-type.ts
export enum UserPaymentBankType {
    IYZICO = "IYZICO",
    PARAM = "PARAM",
    MOKA = "MOKA"
}

export const userPaymentBankTypeMapping: Record<keyof typeof UserPaymentBankType, string> = {
    IYZICO: "Iyzico",
    PARAM: "Param",
    MOKA: "Moka"
};
