import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { TemplateService, Template, TemplateField } from 'src/app/services/template/template.service';

@Component({
  selector: 'app-template-detail',
  templateUrl: './template-detail.component.html'
})
export class TemplateDetailComponent implements OnInit {
  
  templateFields: TemplateField[] = [];
  template: Template = <Template>{};

  constructor(
    private route: ActivatedRoute,
    private templateService: TemplateService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => { 
        var name = params.get('name');
        var version = params.get('version');
        console.log(version);
        return this.templateService.getTemplate(name, version); 
      })
    ).subscribe(data => {
      Object.assign(this.template, data);
      this.templateFields.push(...data.fields);
    });
  }

}
