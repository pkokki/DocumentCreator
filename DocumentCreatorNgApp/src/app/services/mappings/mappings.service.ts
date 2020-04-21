import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvService } from '../env/env.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MappingsService {

  constructor(
    private envService: EnvService,
  ) { }

  getMappings(templateName?: string, templateVersion?: string): Observable<Mapping[]> {
    return this.envService.get<Mapping[]>(`/mappings?templateName=${templateName}`).pipe(
      tap(ev => {
        console.log('getMappings', ev);
      })
    );
  }
}

export interface Mapping {
  name: string;
  creationDate: Date;
  templates: number;
  documents: number;
}
