import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { TemplateService } from 'src/app/services/template/template.service';

@Component({
  selector: 'app-template-versions',
  templateUrl: './template-versions.component.html'
})
export class TemplateVersionsComponent implements OnInit {
  vm = {
    versions: []
  };

  constructor(
    private route: ActivatedRoute,
    private templateService: TemplateService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => this.templateService.getTemplateVersions(params.get('name')))
    ).subscribe(data => {
      this.vm.versions.splice(0, this.vm.versions.length, ... data);
    });
  }

}
