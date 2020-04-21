import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Mapping, MappingsService } from 'src/app/services/mappings/mappings.service';

@Component({
  selector: 'app-mappings-table',
  templateUrl: './mappings-table.component.html',
  styleUrls: ['./mappings-table.component.css']
})
export class MappingsTableComponent implements OnInit {
  dataSource: MatTableDataSource<Mapping>;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['name', 'creationDate', 'templates', 'documents', 'isActive', 'actions'];
  title: string;
  templateName: string;
  templateVersion: string;
  mappingsName: string;

  constructor(
    private route: ActivatedRoute,
    private mappingsService: MappingsService
  ) { }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params: ParamMap) => {
      this.templateName = params.get('templateName');
      this.templateVersion = params.get('templateVersion');
      this.title = this.templateName ? `Mappings associated with template ${this.templateName}` : 'All mappings';
      this.dataSource = new MatTableDataSource<Mapping>(); 
      const operation = this.mappingsService.getMappings(this.templateName, this.templateVersion);
      operation.subscribe(data => {
        this.dataSource.data = data;
      });
    });
  }
}
