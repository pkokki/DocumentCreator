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

  getTemplates(): Observable<Template[]> {
    return this.envService.get<Template[]>('/templates').pipe(
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

  getTemplateVersions(name: string): Observable<Template[]> {
    return this.envService.get<Template[]>(`/templates/${name}/versions`);
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
