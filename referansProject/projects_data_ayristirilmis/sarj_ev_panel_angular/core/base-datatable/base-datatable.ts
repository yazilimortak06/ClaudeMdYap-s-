// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\bases\base-datatable\base-datatable.ts
// Pattern: Base Datatable Class — generic DataTableBase<TFilter, DataModel>
// MatTableDataSource, MatPaginator, MatSort yönetimi, callback pattern ile veri alımı,
// pagingInit() ile sayfalama/sıralama merge, getFilterData() ile request wrapper oluşturma

import { BaseDataTableBaseModel } from 'src/app/core/bases/base-datatable/base-datatable-base-model';
import {SelectionModel} from '@angular/cdk/collections';
import {FlatTreeControl} from '@angular/cdk/tree';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import {MatTreeFlatDataSource, MatTreeFlattener} from '@angular/material/tree';
import {BehaviorSubject, merge} from 'rxjs';
import { tap } from 'rxjs/operators';
import { DatatableRequestWrapperCore } from '../../wrapper-core/datatable-request-core-model';
import { DatatableResponseWrapperCore } from '../../wrapper-core/datatable-result-core.model';
import { trigger, state, style, transition, animate } from '@angular/animations';


export class DataTableBase<TFilter,DataModel extends BaseDataTableBaseModel>   {

    showingColumnNames: string[] = [];
    dataList: MatTableDataSource<DataModel> = new MatTableDataSource();
    totalRecordSize: number;
    paginatorTable: MatPaginator;
    sortTable: MatSort;
    searchCriter: DatatableRequestWrapperCore<TFilter> = new DatatableRequestWrapperCore();
    obj:any
    receivedDataFromServer = (callback:any,obj:any) => {};

    constructor(showingColumnNames:string[],
                obj:any,
                callback:any
                ){

          this.showingColumnNames = showingColumnNames;
          this.obj = obj;
          this.receivedDataFromServer = callback;

    }

    public pagingInit(
        sortTable:MatSort,
        paginatorTable:MatPaginator
    ){
        this.sortTable = sortTable;
        this.paginatorTable = paginatorTable;

        this.sortTable.sortChange.subscribe(
            () => {
              this.paginatorTable.pageIndex = 0
            }
          );
          merge(this.sortTable.sortChange, this.paginatorTable.page)
            .pipe(
              tap(() => {
                this.dataCallback();
              })
            )
            .subscribe(() => { }, () => null);
    }

    public dataCallback(){
        this.receivedDataFromServer(() => {

        },this.obj);
    }

    public setDataSource (response:DatatableResponseWrapperCore<DataModel[]>
                    ) : void{

        this.totalRecordSize = response.recordCount;
        this.dataList.data = response.data;

    }

    public getFilterData(filterData:TFilter):DatatableRequestWrapperCore<TFilter>{

        this.searchCriter.data = filterData;

        if (this.sortTable) {
            this.searchCriter.orderDirective = this.sortTable.direction;
            this.searchCriter.orderProperty = this.sortTable.active;
        }

        if (this.paginatorTable) {
            this.searchCriter.recordPerPage = this.paginatorTable.pageSize;
            this.searchCriter.pageNumber = this.paginatorTable.pageIndex;
        }else
        {
            this.searchCriter.recordPerPage = 10;
            this.searchCriter.pageNumber = 0;

        }

        return this.searchCriter;
    }

}
