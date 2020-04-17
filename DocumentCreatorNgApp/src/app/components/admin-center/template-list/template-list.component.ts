import { Component, OnInit } from '@angular/core';
import { TemplateService } from 'src/app/services/template/template.service';

@Component({
  selector: 'app-template-list',
  templateUrl: './template-list.component.html'
})
export class TemplateListComponent implements OnInit {
  items;

  constructor(
    private templateService: TemplateService
  ) { }

  ngOnInit(): void {
    this.items = this.templateService.getTemplates();
  }

}
