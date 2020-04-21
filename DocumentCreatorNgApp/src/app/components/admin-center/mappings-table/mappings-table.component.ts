import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Mapping, MappingsService } from 'src/app/services/mappings/mappings.service';
import { MappingsTableDataSource } from './mappings-table-datasource';

@Component({
  selector: 'app-mappings-table',
  templateUrl: './mappings-table.component.html',
  styleUrls: ['./mappings-table.component.css']
})
export class MappingsTableComponent implements AfterViewInit, OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTable) table: MatTable<Mapping>;
  dataSource: MappingsTableDataSource;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['name', 'creationDate', 'templates', 'documents', 'isActive', 'actions'];
  title: string;
  templateName: string;
  mappingsName: string;

  constructor(
    private route: ActivatedRoute,
    private mappingsService: MappingsService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.templateName = params.get('templateName');
      this.mappingsName = params.get('mappingsName');
      this.title = this.templateName ? `Mappings associated with template ${this.templateName}` : 'All mappings';
      this.dataSource = new MappingsTableDataSource(this.templateName, this.mappingsName, this.mappingsService);
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.table.dataSource = this.dataSource;
  }
}
