import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { TemplatesTableDataSource } from './templates-table-datasource';
import { TemplateService, Template } from 'src/app/services/template/template.service';

@Component({
  selector: 'app-templates-table',
  templateUrl: './templates-table.component.html',
  styleUrls: ['./templates-table.component.css']
})
export class TemplatesTableComponent implements AfterViewInit, OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTable) table: MatTable<Template>;
  dataSource: TemplatesTableDataSource;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['name', 'version', 'timestamp', 'size', 'isActive', 'actions'];

  constructor(
    private templateService: TemplateService
  ) {}

  ngOnInit() {
    this.dataSource = new TemplatesTableDataSource(this.templateService);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.table.dataSource = this.dataSource;
  }
}
