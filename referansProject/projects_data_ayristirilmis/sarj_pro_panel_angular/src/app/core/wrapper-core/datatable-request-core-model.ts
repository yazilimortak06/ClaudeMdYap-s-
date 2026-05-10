// KAYNAK: E:\Projeler\Angular\SarjAllProPanel\src\app\core\wrapper-core\datatable-request-core-model.ts

export class DatatableRequestWrapperCore<T> {

    constructor() {
        this.data = {} as T;
    }

    data: T;
    orderDirective: string;
    orderProperty: string;
    pageNumber: number;
    recordPerPage: number;
    offset: number;

}
