import { DataSource } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { map, tap } from 'rxjs/operators';
import { Observable, merge, of } from 'rxjs';
import { Template, TemplateService } from 'src/app/services/template/template.service';

/**
 * Data source for the TemplatesTable view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
export class TemplatesTableDataSource extends DataSource<Template> {
  data: Template[] = [];
  paginator: MatPaginator;
  sort: MatSort;

  constructor(
    private templateService: TemplateService
  ) {
    super();
  }

  /**
   * Connect this data source to the table. The table will only update when
   * the returned stream emits new items.
   * @returns A stream of the items to be rendered.
   */
  connect(): Observable<Template[]> {
    console.log('connect');
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    // on sort or paginate events, load a new page
    merge(this.sort.sortChange, this.paginator.page).pipe(
        tap(() => {
          return this.getPagedData(this.getSortedData([...this.data]));
        })
    )
    .subscribe();

    return this.templateService.getTemplates().pipe(
      tap(data => this.data = data)
    );
  }

  /**
   *  Called when the table is being destroyed. Use this function, to clean up
   * any open connections or free any held resources that were set up during connect.
   */
  disconnect() {}

  /**
   * Paginate the data (client-side). If you're using server-side pagination,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getPagedData(data: Template[]): Template[] {
    const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
    console.log('getPagedData', startIndex, this.paginator.pageSize, JSON.stringify(data.map(o=> o.size)));
    return data.splice(startIndex, this.paginator.pageSize);
  }

  /**
   * Sort the data (client-side). If you're using server-side sorting,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getSortedData(data: Template[]): Template[] {
    if (!this.sort.active || this.sort.direction === '') {
      return data;
    }
    var sortedData = data.sort((a, b) => {
      const isAsc = this.sort.direction === 'asc';
      switch (this.sort.active) {
        case 'name': return compare(a.name, b.name, isAsc);
        case 'version': return compare(a.version, b.version, isAsc);
        case 'timestamp': return compare(a.timestamp, b.timestamp, isAsc);
        case 'size': return compare(a.size, b.size, isAsc);
        default: return 0;
      }
    });
    console.log('getSortedData', JSON.stringify(sortedData.map(o=> o.size)));
    this.data = sortedData;
    return sortedData;
  }
}

/** Simple sort comparator for example ID/Name columns (for client-side sorting). */
function compare(a: string | number | Date, b: string | number | Date, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
