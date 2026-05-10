// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\bases\base-tree-table\tree-control-datatable-base.ts
// Pattern: Tree Data Table Base — TreeControlBase'i extend eder, paginator + expandable rows + server-side lazy load

import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatPaginator } from '@angular/material/paginator';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { BehaviorSubject } from 'rxjs';
import { TreeControlBase, TreeResultItem, GeneralItemFlatNode } from '../base-tree/tree-contro.base';
import { TreeControlDataTableDataInterface } from './tree-control-table-data-interface';
import { TreeControlDataTableFilter } from './tree-control-table-filter-model';


export class TreeDataTableControlBase extends TreeControlBase {

  public displayedColumns: string[];
  public paginator: MatPaginator;
  public totalRecordSize: number;
  public expandedNodes: Map<string, TupleDataKey> = new Map<string, TupleDataKey>();
  public filterData: TreeControlDataTableFilter;
  public dataTableData: TreeControlDataTableDataInterface[];
  public initialPageNumber = 15;
  public receivedDataFromServer = (callback: any, obj: any) => { };
  public obj: any;

  constructor(
    displayedColumns: string[],
    obj: any,
    callback: any
  ) {
    super();
    this.obj = obj;
    this.displayedColumns = displayedColumns;
    this.filterData = new TreeControlDataTableFilter();
    this.filterData.isMainLevel = true;
    this.receivedDataFromServer = callback;
  }


  public setDataSource(data: TreeControlDataTableDataInterface[],
    totalRecordSize: number,
  ): void {

    this.totalRecordSize = totalRecordSize;

    if (this.filterData.isMainLevel) {
      this.dataTableData = data;
    } else {
      this.dataTableData = this.addSubValue(this.dataTableData, data)
    }

    this.treeData = this.convertData(this.dataTableData);
    this.setTreeData(this.treeData);
  }

  public setTreeData(treeData: TreeResultItem[]): void {
    super.setTreeData(treeData);
  }

  private addSubValue(items: TreeControlDataTableDataInterface[],
    added: TreeControlDataTableDataInterface[]): TreeControlDataTableDataInterface[] {
    let donen = items;
    var isExists = false;
    added.forEach(iiSub => {
      isExists = false;
      items.forEach(ii => {
        if (ii.id == iiSub.id) { isExists = true; }
      });
      if (!isExists) { donen.push(iiSub); }
    });
    return donen;
  }

  private convertData(items: TreeControlDataTableDataInterface[]): TreeResultItem[] {
    let donen: TreeResultItem[] = [];
    items.forEach(ii => {
      donen.push(
        new TreeResultItem(ii.id.toString(),
          ii.parentId == null ? null : ii.parentId.toString(),
          ii
        )
      );
    });
    return donen;
  }


  toggleEvent(data: GeneralItemFlatNode) {
    this.treeControl.toggle(data);
    var dataState: TupleDataKey = new TupleDataKey();
    dataState.state = this.treeControl.isExpanded(data);
    dataState.data = data;
    this.expandedNodes.set(data.nodeKey, dataState);
  }

  isExpandedData(data: GeneralItemFlatNode) {
    return this.treeControl.isExpanded(data);
  }

  isExpandableData(data: GeneralItemFlatNode) {
    return data.expandable;
  }

  treeStateApply() {
    this.expandedNodes.forEach((data) => {
      if (data.state) {
        this.treeControl.dataNodes.forEach(item => {
          if (item.nodeKey == data.data.nodeKey) {
            this.treeControl.expand(item);
          }
        });
      }
    });
  }

  subChildControl(data: GeneralItemFlatNode) {
    let mainData = data.nodeData as TreeControlDataTableDataInterface;
    return mainData.isHasChild ? '' : 'hidden';
  }

  subChildControlBoolean(data: GeneralItemFlatNode) {
    let mainData = data.nodeData as TreeControlDataTableDataInterface;
    return mainData.isHasChild;
  }

  treeExpandClick(data: GeneralItemFlatNode) {
    if (!this.isExpandedData(data)) {
      var mainData = data.nodeData as TreeControlDataTableDataInterface;
      this.initFilterData(false, mainData.id);

      this.receivedDataFromServer(() => {
        this.toggleEvent(data);
        this.treeStateApply()
      }, this.obj);
    } else {
      this.toggleEvent(data);
    }
  }

  initFilterData(isMainLevel: boolean, parentId: number) {
    this.filterData.isMainLevel = isMainLevel;
    this.filterData.parentId = parentId;
  }

}



class TupleDataKey {
  data: GeneralItemFlatNode;
  state: boolean;
}
