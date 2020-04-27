import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from './../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EnvService {
  private env: Env = {
    baseUrl: environment.endpoints[0].url,
    activeEndpoint: environment.endpoints[0],
    endpoints: environment.endpoints
  };
  private envObs: BehaviorSubject<Env> = new BehaviorSubject(this.env);

  constructor(
    private http: HttpClient
  ) { }

  getEnv(): Observable<Env> {
    return this.envObs;
  }

  activate(endpointName: string): Env {
    var endpoint = this.env.endpoints.find(x => x.name === endpointName);
    if (endpoint) {
      this.env.activeEndpoint = endpoint; 
      this.env.baseUrl = endpoint.url;
    }
    this.envObs.next(this.env);
    return this.env;
  }

  get<T>(url: string): Observable<T> {
    if (!url.startsWith('/'))
      url = '/' + url;
    return this.http.get<T>(`${this.env.baseUrl}${url}`);
  }

  post<T>(url: string, body: {}): Observable<T> {
    if (!url.startsWith('/'))
      url = '/' + url;
    return this.http.post<T>(`${this.env.baseUrl}${url}`, body);
  }
}

export interface Env {
  baseUrl: string;
  activeEndpoint: {name: string; url: string;};
  endpoints: {name: string; url: string;}[];
}