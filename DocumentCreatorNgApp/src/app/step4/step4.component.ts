import { Component, OnInit } from '@angular/core';
import { State } from '../services/state/state.service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-step4',
  templateUrl: './step4.component.html',
  styleUrls: ['./step4.component.css']
})
export class Step4Component implements OnInit {
  
  constructor(public state: State, private http: HttpClient, private sanitizer: DomSanitizer) { }

  submitError;
  submitResponse;
  uploading = false;

  ngOnInit(): void {
  }

  submit() {
    this.uploading = true;
    this.submitError = null;
    this.submitResponse = null;
    const url = this.state.apiBaseUrl + '/templates/' + this.state.templateName + '/mappings/' + this.state.mappingName+ '/document';
    const body = this.state.testPayload;
    const headers = new HttpHeaders({
      'Content-Type' : 'application/json',
      'Cache-Control': 'no-cache'
    }); 
    this.http.post<HttpResponse<Blob>>(url, body, { headers: headers, observe: 'response', responseType: 'blob' as 'json'}).subscribe(
      response => {
        this.uploading = false;
        // see https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Expose-Headers
        var contentDisposition = response.headers.get('content-disposition');
        var filename = contentDisposition.split('filename=')[1].split(';')[0] ?? 'document.docx';
        var contentType = response.headers.get('content-type');
        this.downLoadFile(response.body, contentType, filename);
      },
      err => { 
        console.log(err);
        this.uploading = false;
        this.submitError = err.message;
      }
    );
  }

  private downLoadFile(data: any, contentType: string, filename: string) {
    let blob = new Blob([data], { type: contentType});
    let url = window.URL.createObjectURL(blob);
    var a = document.createElement("a");
    a.href = url;
    a.download = filename;
    a.click();
  }
}
