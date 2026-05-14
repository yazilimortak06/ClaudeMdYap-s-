// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\payments\userPayment-method.ts
export enum UserPaymentMethod {
    DEBIT_CARD = "DEBIT_CARD",
    WALLET = "WALLET"
}

export const userPaymentMethodMapping: Record<keyof typeof UserPaymentMethod, string> = {
    DEBIT_CARD: "Kredi Karti",
    WALLET: "Cuzdan"
};
