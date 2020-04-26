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

  getMappings(mappingName?: string): Observable<Mapping[]> {
    let url = '/mappings';
    if (mappingName)
      url += `/${mappingName}/templates`;
    return this.envService.get<Mapping[]>(url).pipe(
      tap(ev => {
        console.log('getMappings', ev);
      })
    )
  }

  getMappingVersions(templateName?: string, templateVersion?: string, mappingName?: string): Observable<MappingVersion[]> {
    var url: string;
    if (templateName) {
      url = `/templates/${templateName}`;
      if (templateVersion)
        url += `/versions/${templateVersion}`;
      if (mappingName)
        url += `/mappings/${mappingName}/versions`;
      else
        url += '/mappings';
    }
    else if (mappingName) {
      url = `/mappings/${mappingName}/versions`;
    }
    else
      window.alert(`WTF? ${templateName} ${templateVersion} ${mappingName}`);
    return this.envService.get<MappingVersion[]>(url).pipe(
      tap(ev => {
        console.log('getMappingVersions', ev);
      })
    );
  }
}

export interface Mapping {
  mappingName: string;
  templateName: string;
  timestamp: Date;
  templates: number;
  documents: number;
}

export interface MappingVersion {
  mappingName: string;
  mappingVersion: string;
  templateName: string;
  templateVersion: string;
  fileName: string;
  timestamp: Date;
  size: number;
}