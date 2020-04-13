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
            this.env.baseUrl = data.endpoints[0].url;
          }
          return this.env;
        })
      )
  }
}

export interface Env {
  baseUrl: string;
  endpoints: [{name: string; url: string;}];
}