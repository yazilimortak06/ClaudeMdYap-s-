// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\app\core\bases\base-tree\tree-contro.base.ts
// Pattern: Tree Control Base — CDK FlatTreeControl + MatTreeFlatDataSource ile hierarşik ağaç yapısı,
// parentKey/childKey ile flat list'ten tree oluşturma, checkbox selection yönetimi

import {SelectionModel} from '@angular/cdk/collections';
import {FlatTreeControl} from '@angular/cdk/tree';
import {MatTreeFlatDataSource, MatTreeFlattener} from '@angular/material/tree';
import {BehaviorSubject} from 'rxjs';



export class GeneralItemNode {

    children: GeneralItemNode[];
    nodeKey:string;
    nodeData:Object;

    constructor(nodeKey:string,nodeData:Object){
      this.children = [];
      this.nodeKey = nodeKey;
      this.nodeData = nodeData;
    }
  }

  export class GeneralItemFlatNode {
    nodeKey: string;
    level: number;
    expandable: boolean;
    nodeData:Object;
  }


export class TreeControlBase {

    flatNodeMap = new Map<GeneralItemFlatNode, GeneralItemNode>();
    nestedNodeMap = new Map<GeneralItemNode, GeneralItemFlatNode>();
    selectedParent: GeneralItemFlatNode | null = null;
    treeControl: FlatTreeControl<GeneralItemFlatNode>;
    treeFlattener: MatTreeFlattener<GeneralItemNode, GeneralItemFlatNode>;
    dataSource: MatTreeFlatDataSource<GeneralItemNode, GeneralItemFlatNode>;
    checklistSelection = new SelectionModel<GeneralItemFlatNode>(true /* multiple */);
    getLevel = (node: GeneralItemFlatNode) => node.level;
    isExpandable = (node: GeneralItemFlatNode) => node.expandable;
    getChildren = (node: GeneralItemNode): GeneralItemNode[] => node.children;
    hasChild = (_: number, _nodeData: GeneralItemFlatNode) => _nodeData.expandable;
    hasNoContent = (_: number, _nodeData: GeneralItemFlatNode) => _nodeData.nodeKey === '';
    dataChange = new BehaviorSubject<GeneralItemNode[]>([]);
    get data(): GeneralItemNode[] { return this.dataChange.value; }
    treeData:TreeResultItem[] = [];


    constructor() {
        this.treeFlattener = new MatTreeFlattener(this.transformer, this.getLevel,
        this.isExpandable, this.getChildren);
        this.treeControl = new FlatTreeControl<GeneralItemFlatNode>(this.getLevel, this.isExpandable);
        this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

        this.dataChange.subscribe(data => {
          this.dataSource.data = data;
        });
    }

   setTreeData (treeData:TreeResultItem[]) : void{
     this.treeData = treeData;
     const data = this.buildFileTree(this.treeData, 0);
     this.dataChange.next(data);
  }


   private buildFileTree(obj: TreeResultItem[] , level: number): GeneralItemNode[] {
        let returningTemp:GeneralItemNode[] = [];
        let returning:GeneralItemNode[] = [];

        let findParent = (items:GeneralItemNode[],parentKey:String):GeneralItemNode =>  {
            var finding:GeneralItemNode = null;
            finding = items.filter(ii => ii.nodeKey == parentKey)[0];
            if(finding == undefined){ finding = null; }
            return finding;
        }

        let findParentFromAllElements = (parentKey:String):boolean =>  {
          var finding:boolean = false;
          let findingTemp = obj.filter(ii => ii.key == parentKey)[0];
          if(findingTemp == undefined){ finding = false; }
          else { finding = true; }
          return finding;
        }

        let processTree = ():boolean => {
            var isLinkedAllElements = true;
            obj.forEach((value,index) => {
              if(!value.isLinked){
                if(value.parentKey != null && findParentFromAllElements(value.parentKey)){
                  let finding = findParent(returningTemp,value.parentKey);
                  if(finding != null){
                    obj[index].isLinked = true;
                    let findingElement = new GeneralItemNode(value.key,value.data);
                    finding.children.push(findingElement);
                    returningTemp.push(findingElement);
                  }else {
                    isLinkedAllElements = false;
                  }
                }else {
                  obj[index].isLinked = true;
                  let findingElement = new GeneralItemNode(value.key,value.data);
                  returning.push(findingElement);
                  returningTemp.push(findingElement);
                }
              }
           });
          return isLinkedAllElements;
        }

        while(true){
          if(processTree()){ break; }
        }

        return returning;
      }


    transformer = (node: GeneralItemNode, level: number) => {
      const existingNode = this.nestedNodeMap.get(node);
      const flatNode = existingNode && existingNode.nodeKey === node.nodeKey
          ? existingNode
          : new GeneralItemFlatNode();
      flatNode.nodeKey = node.nodeKey;
      flatNode.level = level;
      flatNode.nodeData = node.nodeData;
      flatNode.expandable = !!node.children?.length;
      this.flatNodeMap.set(flatNode, node);
      this.nestedNodeMap.set(node, flatNode);
      return flatNode;
    }

    /** Whether all the descendants of the node are selected. */
    descendantsAllSelected(node: GeneralItemFlatNode): boolean {
      const descendants = this.treeControl.getDescendants(node);
      const descAllSelected = descendants.length > 0 && descendants.every(child => {
        return this.checklistSelection.isSelected(child);
      });
      return descAllSelected;
    }

    /** Whether part of the descendants are selected */
    descendantsPartiallySelected(node: GeneralItemFlatNode): boolean {
      const descendants = this.treeControl.getDescendants(node);
      const result = descendants.some(child => this.checklistSelection.isSelected(child));
      return result && !this.descendantsAllSelected(node);
    }

    /** Toggle the to-do item selection. Select/deselect all the descendants node */
    todoItemSelectionToggle(node: GeneralItemFlatNode): void {
      this.checklistSelection.toggle(node);
      const descendants = this.treeControl.getDescendants(node);
      this.checklistSelection.isSelected(node)
        ? this.checklistSelection.select(...descendants)
        : this.checklistSelection.deselect(...descendants);
      descendants.forEach(child => this.checklistSelection.isSelected(child));
      this.checkAllParentsSelection(node);
    }

    /** Toggle a leaf to-do item selection. Check all the parents to see if they changed */
    todoLeafItemSelectionToggle(node: GeneralItemFlatNode): void {
      this.checklistSelection.toggle(node);
      this.checkAllParentsSelection(node);
    }

    todoItemSelectionToggleOnlyOneChild(node: GeneralItemFlatNode): void {
      this.checklistSelection.toggle(node);
    }

    /* Checks all the parents when a leaf node is selected/unselected */
    checkAllParentsSelection(node: GeneralItemFlatNode): void {
      let parent: GeneralItemFlatNode | null = this.getParentNode(node);
      while (parent) {
        this.checkRootNodeSelection(parent);
        parent = this.getParentNode(parent);
      }
    }

    /** Check root node checked state and change it accordingly */
    checkRootNodeSelection(node: GeneralItemFlatNode): void {
      const nodeSelected = this.checklistSelection.isSelected(node);
      const descendants = this.treeControl.getDescendants(node);
      const descAllSelected = descendants.length > 0 && descendants.every(child => {
        return this.checklistSelection.isSelected(child);
      });
      if (nodeSelected && !descAllSelected) {
        this.checklistSelection.deselect(node);
      } else if (!nodeSelected && descAllSelected) {
        this.checklistSelection.select(node);
      }
    }

    /* Get the parent node of a node */
    getParentNode(node: GeneralItemFlatNode): GeneralItemFlatNode | null {
      const currentLevel = this.getLevel(node);
      if (currentLevel < 1) { return null; }
      const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;
      for (let i = startIndex; i >= 0; i--) {
        const currentNode = this.treeControl.dataNodes[i];
        if (this.getLevel(currentNode) < currentLevel) { return currentNode; }
      }
      return null;
    }

    getSelectedCheckList(): GeneralItemFlatNode[] {
      return this.checklistSelection.selected;
    }

  }


  export class TreeResultItem {
      key:string;
      parentKey:string;
      data:Object;
      isLinked:boolean;

      constructor(key:string,parentKey:string,data:Object) {
          this.key = key;
          this.parentKey = parentKey;
          this.data = data;
          this.isLinked = false;
      }
  }
