import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { DCTemplate } from '../model';
import { UploadService } from '../upload.service';

@Component({
  selector: 'app-step1',
  templateUrl: './step1.component.html',
  styleUrls: ['./step1.component.css']
})
export class Step1Component implements OnInit {
  @Input() apiUrl: string;
  @Output() onTemplateNameChanged = new EventEmitter<string>();
  @ViewChild('file', { static: false }) file;
  
  template: DCTemplate = {
    name: 'T01'
  };
  uploading = false;
  errorMessage: string;
  fileName: string;
  uploadUrl: string;

  constructor(public uploadService: UploadService) { }

  ngOnInit(): void {
    this.uploadUrl = this.apiUrl + '/templates';
    this.onTemplateNameChanged.emit(this.template.name);
  }

  nameChanged() {
    this.onTemplateNameChanged.emit(this.template.name);
  }

  onFileAdd() {
    const files: FileList = this.file.nativeElement.files;
    if (files.length > 0) {
      const file = files[0];
      this.uploading = true;
      this.errorMessage = null;
      this.fileName = file.name;
      const progress = this.uploadService.upload(this.uploadUrl, this.template, file);
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
