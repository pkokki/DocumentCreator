import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpRequest,
  HttpEventType,
  HttpResponse
} from '@angular/common/http';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UploadService {

  constructor(private http: HttpClient) { }

  public upload(url: string, data: {}, file: File) : Observable<number> {
    // create a new multipart-form
    const formData: FormData = new FormData();
    for ( var key in data) {
      formData.append(key, data[key]);
    }
    formData.append('FILE', file, file.name);

    // create a http-post request and pass the form
    // tell it to report the upload progress
    const req = new HttpRequest('POST', url, formData, {
      reportProgress: true
    });

    // create a new progress-subject 
    const progress = new Subject<number>();

    // send the http-request and subscribe for progress-updates
    this.http.request(req).subscribe(event => {
      console.log('event', event);
      if (event.type === HttpEventType.UploadProgress) {
        // calculate the progress percentage
        const percentDone = Math.round((100 * event.loaded) / event.total);
        // pass the percentage into the progress-stream
        progress.next(percentDone);
      } else if (event instanceof HttpResponse) {
        progress.complete();
      }
    }, error => {
      console.log('error', error);
      progress.error(error.message);
    });

    return progress.asObservable();
  }
}
