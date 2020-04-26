import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { MappingsService, Mapping } from 'src/app/services/mappings/mappings.service';

@Component({
  selector: 'app-mappings-table',
  templateUrl: './mappings-table.component.html',
  styleUrls: ['./mappings-table.component.css']
})
export class MappingsTableComponent implements OnInit, AfterViewInit {
  isLoading = true;
  sourceDataCount = 0;

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  // ---------------------------------------------------------------------
  title: string;
  mappingName: string;

  source: Observable<Mapping[]>;
  sourceData: Mapping[] = null;
  pagedData: Mapping[] = [];

  displayedColumns: any;

  constructor(
    private route: ActivatedRoute,
    private mappingsService: MappingsService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.mappingName = params.get('mappingName');
      if (this.mappingName) {
        this.title = `Templates associated with mapping ${this.mappingName}`;
        this.displayedColumns = ['templateName', 'mappingName', 'timestamp', 'documents', 'isActive', 'actions'];
      }
      else {
        this.title = 'All mappings';
        this.displayedColumns = ['mappingName', 'timestamp', 'templates', 'documents', 'isActive', 'actions'];
      }
      this.source = this.mappingsService.getMappings(this.mappingName);
    });
  }

  // ---------------------------------------------------------------------

  ngAfterViewInit() {
    this.source.subscribe(data => {
      this.sourceData = data;
      this.sourceDataCount = data.length;
      this.pageData();
      this.isLoading = false;
    }, () => this.isLoading = false);

    this.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
      this.sortData();
      this.pageData();
    });

    this.paginator.page.subscribe(() => {
      this.pageData();
    });
  }

  /**
   * Paginate the data (client-side). 
   */
  private pageData() {
    const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
    this.pagedData = [...this.sourceData].splice(startIndex, this.paginator.pageSize);
  }

  /**
   * Sort the data (client-side). 
   */
  private sortData() {
    const isAsc = this.sort.direction !== 'desc';
    const propName = this.sort.active;
    this.sourceData = this.sourceData.sort((a, b) => {
      return (a[propName] < b[propName] ? -1 : 1) * (isAsc ? 1 : -1);
    });
  }
}
