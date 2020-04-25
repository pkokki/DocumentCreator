import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvService } from '../env/env.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TemplateService {

  constructor(
    private envService: EnvService,
  ) { }

  getTemplates(templateName?: string, mappingName?: string): Observable<Template[]> {
    let url: string;
    if (templateName)
      url = `/templates/${templateName}/versions`;
    else if (mappingName) 
      url = `/mappings/${mappingName}/templates`;
    else
      url = '/templates';
    return this.envService.get<Template[]>(url).pipe(
      tap(ev => {
        console.log('getTemplates', ev);
      })
    );
  }

  getTemplate(name: string, version?: string): Observable<Template> {
    if (version)
      return this.envService.get<Template>(`/templates/${name}/versions/${version}`);
    else
      return this.envService.get<Template>(`/templates/${name}`);
  }
}


export interface Template {
  name: string;
  version: string;
  timestamp: Date;
  size: number;
  fields: TemplateField[] | null;
}

export interface TemplateField {
  name: string;
  parent: string;
  isCollection: boolean;
  content: string;
}
