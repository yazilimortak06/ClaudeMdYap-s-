// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\wrapper-core\server-result-core-model.ts
export class ServerResultCoreModel {
    resultType: ServerResultType;
    errorCode: number;
    errorMessage: string;
}

enum ServerResultType {
    Ok = "Ok",
    Error = "Error",
    ValidationError = "ValidationError",
    Unauthorized = "Unauthorized",
    Exception = "Exception"
}
