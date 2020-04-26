import { Component, AfterViewInit, ViewChild } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of, Observable } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { DocumentService, Document, PagedResults } from 'src/app/services/document/document.service';

@Component({
  selector: 'app-documents-table',
  templateUrl: './documents-table.component.html',
  styleUrls: ['./documents-table.component.css']
})
export class DocumentsTableComponent implements AfterViewInit {
  displayedColumns = ['documentId', 'timestamp', 'templateName', 'templateVersion', 'mappingName', 'mappingVersion', 'size', 'link'];
  criteria: string;

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
    this.route.paramMap.subscribe((params: ParamMap) => {

      this.templateName = params.get('templateName');
      if (this.templateName) this.filters.push(`templateName=${this.templateName}`);

      this.templateVersion = params.get('templateVersion');
      this.templateVersion = this.templateVersion == 'all' || !this.templateName ? null : this.templateVersion;
      if (this.templateVersion) this.filters.push(`templateVersion=${this.templateVersion}`);
      
      this.mappingsName = params.get('mappingName');
      if (this.mappingsName) this.filters.push(`mappingsName=${this.mappingsName}`);

      this.mappingsVersion = params.get('mappingVersion');
      this.mappingsVersion = this.mappingsVersion == 'all' || !this.mappingsName ? null : this.mappingsVersion;
      if (this.mappingsVersion) this.filters.push(`mappingsVersion=${this.mappingsVersion}`);

      this.criteria = this.filters.length ? this.filters.join(', ') : 'all';
    });
  }

  ngAfterViewInit() {
    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    // Create an output Observable which concurrently emits all values from every given input Observable.
    merge(this.sort.sortChange, this.paginator.page)
      // Create a chain of functional operators
      .pipe(
        // Returns an Observable that emits the items you specify as arguments before it begins to emit items emitted by the source Observable.
        startWith({}),
        // Projects each source value to an Observable which is merged in the output Observable, emitting values only from the most recently projected Observable.
        switchMap(() => {
          this.isLoadingResults = true;
          return this.refresh(this.sort.active, this.sort.direction, this.paginator.pageIndex, this.paginator.pageSize);
        }),
        // Applies a given project function to each value emitted by the source Observable, and emits the resulting values as an Observable.
        map(data => {
          this.isLoadingResults = false;
          this.totalDocuments = data.total;
          return data.results;
        }),
        // Catches errors on the observable to be handled by returning a new observable or throwing an error.
        catchError(() => {
          this.isLoadingResults = false;
          return of(<Document[]>[]);
        })
      )
      .subscribe(data => this.documents = data);
  }
  
  refresh(orderBy: string = "documentId", sortDirection: string, pageIndex: number, pageSize: number): Observable<PagedResults<Document>> {
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
