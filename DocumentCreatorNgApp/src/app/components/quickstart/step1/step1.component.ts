import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UploadService } from '../../../services/upload/upload.service'
import { State } from '../../../services/state/state.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-step1',
  templateUrl: './step1.component.html'
})
export class Step1Component implements OnInit {
  @ViewChild('file', { static: false }) private file: ElementRef;
  
  uploading = false;

  constructor(
    public state: State, 
    private uploadService: UploadService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
  }

  onFileAdd() {
    console.log(this.file);
    const files: FileList = this.file.nativeElement.files;
    if (files.length > 0) {
      const file = files[0];
      this.uploading = true;
      const progress = this.uploadService.upload(this.state.apiBaseUrl + '/templates', { name: this.state.templateName }, file);
      progress.subscribe(end => {
        this.uploading = false;
        this.snackBar.open('Template uploaded succesfully', null, { duration: 2000 });
      }, err => {
        this.uploading = false;
        //this.errorMessage = err;
      });
    }
  }

  addFile() {
    this.file.nativeElement.click();
  }
}
