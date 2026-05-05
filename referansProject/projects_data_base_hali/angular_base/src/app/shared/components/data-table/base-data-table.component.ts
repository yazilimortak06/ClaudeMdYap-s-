import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  type?: 'text' | 'date' | 'number' | 'badge' | 'actions';
  width?: string;
}

export interface TableAction {
  icon: string;
  label: string;
  color?: 'primary' | 'accent' | 'warn';
  action: string;
}

@Component({
  selector: 'app-base-data-table',
  template: `
    <div class="table-container">
      <table mat-table [dataSource]="dataSource" matSort (matSortChange)="onSort($event)">
        <ng-container *ngFor="let col of columns" [matColumnDef]="col.key">
          <th mat-header-cell *matHeaderCellDef [mat-sort-header]="col.sortable ? col.key : ''">
            {{ col.label }}
          </th>
          <td mat-cell *matCellDef="let row">
            <ng-container [ngSwitch]="col.type">
              <span *ngSwitchCase="'date'">{{ row[col.key] | date: 'dd.MM.yyyy HH:mm' }}</span>
              <div *ngSwitchCase="'actions'" class="actions-cell">
                <button
                  mat-icon-button
                  *ngFor="let action of actions"
                  [color]="action.color || 'primary'"
                  [matTooltip]="action.label"
                  (click)="onAction(action.action, row)"
                >
                  <mat-icon>{{ action.icon }}</mat-icon>
                </button>
              </div>
              <span *ngSwitchDefault>{{ row[col.key] }}</span>
            </ng-container>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
      </table>

      <mat-paginator
        [length]="totalCount"
        [pageSize]="pageSize"
        [pageIndex]="pageIndex"
        [pageSizeOptions]="pageSizeOptions"
        (page)="onPage($event)"
        showFirstLastButtons
      ></mat-paginator>
    </div>
  `,
  styles: [`
    .table-container { width: 100%; overflow: auto; }
    .actions-cell { display: flex; gap: 4px; }
    table { width: 100%; }
  `]
})
export class BaseDataTableComponent implements OnChanges {
  @Input() data: unknown[] = [];
  @Input() columns: TableColumn[] = [];
  @Input() actions: TableAction[] = [];
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() pageIndex = 0;
  @Input() pageSizeOptions = [5, 10, 25, 50];

  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() sortChange = new EventEmitter<Sort>();
  @Output() actionClick = new EventEmitter<{ action: string; row: unknown }>();

  dataSource = new MatTableDataSource<unknown>();

  get displayedColumns(): string[] {
    return this.columns.map((c) => c.key);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['data']) {
      this.dataSource.data = this.data;
    }
  }

  onPage(event: PageEvent): void {
    this.pageChange.emit(event);
  }

  onSort(sort: Sort): void {
    this.sortChange.emit(sort);
  }

  onAction(action: string, row: unknown): void {
    this.actionClick.emit({ action, row });
  }
}
