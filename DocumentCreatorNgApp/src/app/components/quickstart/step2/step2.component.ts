import { Component, OnInit } from '@angular/core';
import { State } from '../../../services/state/state.service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-step2',
  templateUrl: './step2.component.html',
  styleUrls: ['../quickstart.component.css']
})
export class Step2Component implements OnInit {
  withSources = false;
  sourcePayload: string;
  uploading = false;

  constructor(
    public state: State,
    private httpClient: HttpClient
  ) { }

  ngOnInit(): void {
    this
      .httpClient.get('assets/example/ob-direct-payment-request.json', {responseType: "text"})
      .subscribe(text => {
        this.sourcePayload = text;
        this.state.testPayload = text;
      });
  }

  download() {
    const url = this.state.apiBaseUrl+'/templates/'+this.state.templateName+'/mappings/'+this.state.mappingName+'/xls';
    if (this.withSources) {
      const body = {
        sources: [
          { name: 'S1', payload: JSON.parse(this.sourcePayload) }
        ]
      };
      const headers = new HttpHeaders({
        'Content-Type' : 'application/json',
        'Cache-Control': 'no-cache'
      }); 
      this.httpClient.put<HttpResponse<Blob>>(url, body, { headers: headers, observe: 'response', responseType: 'blob' as 'json'}).subscribe(
        response => {
          this.uploading = false;
          // see https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Expose-Headers
          var contentDisposition = response.headers.get('content-disposition');
          var filename = contentDisposition?.split('filename=')[1].split(';')[0] ?? 'mappings.xlsm';
          var contentType = response.headers.get('content-type');
          this.downLoadFile(response.body, contentType, filename);
        },
        err => { 
          console.log(err);
          this.uploading = false;
        }
      );
    }
    else {
      window.open(url, '_blank');
    }
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
