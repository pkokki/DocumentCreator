import { Component, OnInit, ViewChild, Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { UploadService } from '../services/upload/upload.service'
import { State } from '../services/state/state.service';

@Component({
  selector: 'app-step1',
  templateUrl: './step1.component.html',
  styleUrls: ['./step1.component.css']
})
export class Step1Component implements OnInit {
  @ViewChild('file', { static: false }) private file: ElementRef;
  
  uploading = false;
  errorMessage: string;
  fileName: string;

  constructor(public state: State, private uploadService: UploadService) { }

  ngOnInit(): void {
  }

  onFileAdd() {
    console.log(this.file);
    const files: FileList = this.file.nativeElement.files;
    if (files.length > 0) {
      const file = files[0];
      this.uploading = true;
      this.errorMessage = null;
      this.fileName = file.name;
      const progress = this.uploadService.upload(this.state.apiBaseUrl + '/templates', { name: this.state.templateName }, file);
      progress.subscribe(end => {
        this.uploading = false;
      }, err => {
        this.uploading = false;
        this.errorMessage = err;
      });
    }
  }

  addFile() {
    this.file.nativeElement.click();
  }
}
