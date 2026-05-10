// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\evtech\enums\authentication\panel-admin-user-type-enum.ts
export enum PanelAdminUserType {
    ADMIN_USER = "ADMIN_USER",
    COMPANY_ADMIN_USER = "COMPANY_ADMIN_USER",
    ROOT_ADMIN_USER = "ROOT_ADMIN_USER",
    TEST_ADMIN_USER = "TEST_ADMIN_USER"
}

export enum AdminManagmentType {
    MAIN_ADMIN = "MAIN_ADMIN",
    COMPANY_ADMIN = "COMPANY_ADMIN",
    TEST_ADMIN_USER = "TEST_ADMIN_USER"
}

export const adminManagmentTypeMapping: Record<keyof typeof AdminManagmentType, string> = {
    MAIN_ADMIN: "Genel Admin",
    COMPANY_ADMIN: "Firma Admin",
    TEST_ADMIN_USER: "Test Admin"
};
