import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
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

  getBaseUrl(): string {
    return this.env.baseUrl;
  }
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

  download(url: string): void {
    if (!url.startsWith('/'))
      url = '/' + url;
    const headers = new HttpHeaders({
      'Content-Type' : 'application/json',
      'Cache-Control': 'no-cache'
    }); 
    this.http.get<Blob>(`${this.env.baseUrl}${url}`, { headers: headers, observe: 'response', responseType: 'blob' as 'json'}).subscribe(
      response => {
        // see https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Expose-Headers
        var contentDisposition = response.headers.get('content-disposition');
        var filename = contentDisposition.split('filename=')[1].split(';')[0] ?? 'document.docx';
        var contentType = response.headers.get('content-type');
        // prepare object url in anchor and click it
        let blob = new Blob([response.body], { type: contentType});
        let url = window.URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.href = url;
        a.download = filename;
        a.click();
      },
      err => { 
        console.log(err);
      }
    );
  }


}

export interface Env {
  baseUrl: string;
  activeEndpoint: {name: string; url: string;};
  endpoints: {name: string; url: string;}[];
}