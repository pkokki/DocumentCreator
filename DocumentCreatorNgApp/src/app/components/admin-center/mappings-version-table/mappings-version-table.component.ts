import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { MappingsService, MappingVersion } from 'src/app/services/mappings/mappings.service';

@Component({
  selector: 'app-mappings-version-table',
  templateUrl: './mappings-version-table.component.html',
  styleUrls: ['./mappings-version-table.component.css']
})
export class MappingsVersionTableComponent implements OnInit, AfterViewInit {
  isLoading = true;
  sourceDataCount = 0;

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  // ---------------------------------------------------------------------
  title: string;
  templateName: string;
  templateVersion: string;
  mappingName: string;

  source: Observable<MappingVersion[]>;
  sourceData: MappingVersion[] = null;
  pagedData: MappingVersion[] = [];

  displayedColumns = ['mappingName', 'mappingVersion', 'templateName', 'templateVersion', 'timestamp', 'size', 'actions'];

  constructor(
    private route: ActivatedRoute,
    private mappingsService: MappingsService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.templateName = params.get('templateName');
      this.templateVersion = params.get('templateVersion');
      this.mappingName = params.get('mappingName');
      this.title = this.templateName 
        ? `Mappings associated with template ${this.templateName}` + (this.templateVersion ? ' - version ' + this.templateVersion : '')
        : this.mappingName
        ? `Versions of mapping ${this.mappingName}`
        : 'All mappings';
      this.source = this.mappingsService.getMappingVersions(this.templateName, this.templateVersion, this.mappingName);
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
