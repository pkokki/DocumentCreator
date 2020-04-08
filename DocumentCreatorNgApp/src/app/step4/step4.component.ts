import { Component, OnInit } from '@angular/core';
import { State } from '../services/state/state.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
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
    const httpHeaders = new HttpHeaders({
      'Content-Type' : 'application/json',
      'Cache-Control': 'no-cache'
    }); 
    this.http.post(url, body, { headers: httpHeaders, responseType: 'arraybuffer' }).subscribe(
      response => {
        this.uploading = false;
        console.log('response', response);
        this.downLoadFile(response, 'application/vnd.openxmlformats-officedocument.wordprocessingml.document');
      },
      err => { 
        console.log(err);
        this.uploading = false;
        this.submitError = err.message;
      }
    );
  }

  private downLoadFile(data: any, type: string) {
    let blob = new Blob([data], { type: type});
    //let url = this.sanitizer.bypassSecurityTrustResourceUrl(window.URL.createObjectURL(blob));
    let url = window.URL.createObjectURL(blob);
    console.log('URL', url);
    var a = document.createElement("a");
    a.href = url;
    a.download = "xxxx.docx";
    a.click();
    //let pwa = window.open(url, '_blank');
    //if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
    //    window.alert( 'Please disable your Pop-up blocker and try again.');
    //}
  }
}
