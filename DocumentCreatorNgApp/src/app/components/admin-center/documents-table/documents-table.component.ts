import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { DocumentService, Document, PagedResults } from 'src/app/services/document/document.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of, Observable } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-documents-table',
  templateUrl: './documents-table.component.html',
  styleUrls: ['./documents-table.component.css']
})
export class DocumentsTableComponent implements AfterViewInit {
  displayedColumns = ['id', 'timestamp', 'templateName', 'templateVersion', 'mappingName', 'mappingVersion', 'size'];
  title: string;

  templateName: string;
  templateVersion: string;
  mappingsName: string;
  mappingsVersion: string;
  filters: string[] = [];
  
  isLoadingResults = true;
  totalDocuments = 0;
  documents: Document[] = [];

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator)  paginator: MatPaginator;

  constructor(
    private route: ActivatedRoute,
    private documentService: DocumentService
  ) {
  }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params: ParamMap) => {

      this.templateName = params.get('templateName');
      if (this.templateName) this.filters.push(`templateName=${this.templateName}`);

      this.templateVersion = params.get('templateVersion');
      this.templateVersion = this.templateVersion == 'all' || !this.templateName ? null : this.templateVersion;
      if (this.templateVersion) this.filters.push(`templateVersion=${this.templateVersion}`);
      
      this.mappingsName = params.get('mappingsName');
      if (this.mappingsName) this.filters.push(`mappingsName=${this.mappingsName}`);

      this.mappingsVersion = params.get('mappingsVersion');
      this.mappingsVersion = this.mappingsVersion == 'all' || !this.mappingsName ? null : this.mappingsVersion;
      if (this.mappingsVersion) this.filters.push(`mappingsVersion=${this.mappingsVersion}`);

      this.title = this.templateName ? `Documents associated with template ${this.templateName}` : 'All documents';
    });
  }

  ngAfterViewInit() {
    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.refresh(this.sort.active, this.sort.direction, this.paginator.pageIndex, this.paginator.pageSize);
        }),
        map(data => {
          this.isLoadingResults = false;
          this.totalDocuments = data.total;
          return data.results;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          return of(<Document[]>[]);
        })
      ).subscribe(data => this.documents = data);
  }
  
  refresh(orderBy: string = "id", sortDirection: string, pageIndex: number, pageSize: number): Observable<PagedResults<Document>> {
    const isDesc = sortDirection == "desc";

    return this.documentService.getDocuments(
      this.filters,
      pageIndex + 1,
      pageSize,
      orderBy,
      isDesc
    );
  }
}
