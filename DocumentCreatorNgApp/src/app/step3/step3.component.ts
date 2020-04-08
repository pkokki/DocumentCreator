import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UploadService } from '../services/upload/upload.service'
import { State } from '../services/state/state.service';

@Component({
  selector: 'app-step3',
  templateUrl: './step3.component.html',
  styleUrls: ['./step3.component.css']
})
export class Step3Component implements OnInit {
  @ViewChild('file', { static: false }) file: ElementRef;
  
  uploading = false;
  errorMessage: string;
  fileName: string;

  constructor(public state: State, private uploadService: UploadService) { }

  ngOnInit(): void {
  }

  onFileAdd() {
    const files: FileList = this.file.nativeElement.files;
    if (files.length > 0) {
      const file = files[0];
      this.uploading = true;
      this.errorMessage = null;
      this.fileName = file.name;
      const uploadUrl = this.state.apiBaseUrl + '/templates/' + this.state.templateName + '/mappings/' + this.state.mappingName;
      const progress = this.uploadService.upload(uploadUrl, null, file);
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
