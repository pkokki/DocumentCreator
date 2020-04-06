import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { UploadService } from '../upload.service';

@Component({
  selector: 'app-step3',
  templateUrl: './step3.component.html',
  styleUrls: ['./step3.component.css']
})
export class Step3Component implements OnInit {
  @Input() apiUrl: string;
  @Input() templateName: string;
  @Output() onMappingsNameChanged = new EventEmitter<string>();
  @ViewChild('file', { static: false }) file;
  
  mappings;
  uploading = false;
  errorMessage: string;
  fileName: string;
  uploadUrl: string;

  constructor(public uploadService: UploadService) { }

  ngOnInit(): void {
    this.mappings = {
      name: 'M01'
    };
    this.onMappingsNameChanged.emit(this.mappings.name);
  }

  onNameChanged() {
    this.onMappingsNameChanged.emit(this.mappings.name);
  }

  onFileAdd() {
    const files: FileList = this.file.nativeElement.files;
    if (files.length > 0) {
      const file = files[0];
      this.uploading = true;
      this.errorMessage = null;
      this.fileName = file.name;
      this.mappings.templateName = this.templateName;
      this.uploadUrl = this.apiUrl + '/templates/' + this.templateName + '/mappings';
      const progress = this.uploadService.upload(this.uploadUrl, this.mappings, file);
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
