import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TemplateService, Template } from 'src/app/services/template/template.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-templates-table',
  templateUrl: './templates-table.component.html',
  styleUrls: ['./templates-table.component.css']
})
export class TemplatesTableComponent implements OnInit, AfterViewInit {
  isLoading = true;
  sourceDataCount = 0;

  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  // ---------------------------------------------------------------------
  title: string;
  templateName: string;
  mappingName: string;

  source: Observable<Template[]>;
  sourceData: Template[] = null;
  pagedData: Template[] = [];

  displayedColumns = ['name', 'version', 'timestamp', 'size', 'isActive', 'actions'];

  constructor(
    private route: ActivatedRoute,
    private templateService: TemplateService
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.templateName = params.get('templateName');
      this.mappingName = params.get('mappingName');
      this.title = this.templateName ? 
        `Versions of template ${this.templateName}` : 
        this.mappingName ?
          `Templates available for ${this.mappingName}` :
          'All templates (latest versions)';
      this.source =  this.templateService.getTemplates(this.templateName, this.mappingName);
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


