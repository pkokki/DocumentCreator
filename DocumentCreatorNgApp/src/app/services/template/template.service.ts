import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EnvService } from '../env/env.service';
import { switchMap } from 'rxjs/operators';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TemplateService {

  constructor(
    private envService: EnvService,
    private http: HttpClient
  ) { }

  getTemplates(): Observable<Template[]> {
    return this.envService.get().pipe(
      switchMap(env => {
        return this.http.get<Template[]>(`${env.baseUrl}/templates`).pipe(
          tap(ev => console.log(ev))
        );
      })
    );
  }

  getTemplate(name: string): Observable<Template> {
    return this.envService.get().pipe(
      switchMap(env => {
        return this.http.get<Template>(`${env.baseUrl}/templates/${name}`);
      })
    );
  }
}


export interface Template {
  name: string;
  version: string;
  timestamp: Date;
  fields: TemplateField[] | null;
}

export interface TemplateField {
  name: string;
  parent: string;
  isCollection: boolean;
  content: string;
}