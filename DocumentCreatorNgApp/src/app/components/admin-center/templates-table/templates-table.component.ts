import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { TemplateService, Template } from 'src/app/services/template/template.service';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-templates-table',
  templateUrl: './templates-table.component.html',
  styleUrls: ['./templates-table.component.css']
})
export class TemplatesTableComponent implements OnInit {
  dataSource: MatTableDataSource<Template>;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['name', 'version', 'timestamp', 'size', 'isActive', 'actions'];
  title: string;
  templateName: string;

  constructor(
    private route: ActivatedRoute,
    private templateService: TemplateService
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.templateName = params.get('name');
      this.title = this.templateName ? `Versions of template ${this.templateName}` : 'All templates (latest versions)';
      this.dataSource = new MatTableDataSource<Template>(); 
      const operation = this.templateName ? this.templateService.getTemplateVersions(this.templateName) : this.templateService.getTemplates();
      operation.subscribe(data => {
        this.dataSource.data = data;
      });
    });
  }
}
