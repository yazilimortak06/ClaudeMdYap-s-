// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\wrapper-core\datatable-request-core-model.ts
// Pattern: Datatable Request Wrapper — generic filtre + sıralama + sayfalama request modeli

import { T } from "@angular/cdk/keycodes";

export class DatatableRequestWrapperCore<T> {

    constructor(){
      this.data = {} as T;
    }

    data:T;
    orderDirective: string;
    orderProperty: string;
    pageNumber: number;
    recordPerPage: number;
    offset:number;

}
