import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { of, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EnvService {
  private env: Env;

  constructor(
    private http: HttpClient
  ) { }

  get(): Observable<Env> {
    if (this.env)
      return of(this.env);
    return this.http.get<Env>('/assets/env.json')
      .pipe(
        map((data: Env) => {
          this.env = data;
          if (data.endpoints && data.endpoints.length) {
            this.setActiveEndpoint(data.endpoints[0]);
          }
          return this.env;
        })
      )
  }

  activate(endpointName: string): Observable<Env> {
    return this.get()
      .pipe(
        map(env => {
          var endpoint = env.endpoints.find(x => x.name === endpointName);
          this.setActiveEndpoint(endpoint);
          return this.env;
        })
      )
  }

  private setActiveEndpoint(endpoint: { url: any; name: string; }): void {
    if (endpoint) {
      this.env.activeEndpoint = endpoint; 
      this.env.baseUrl = endpoint.url;
    }
  }
}

export interface Env {
  baseUrl: string;
  activeEndpoint: {name: string; url: string;};
  endpoints: [{name: string; url: string;}];
}