import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UploadService } from '../../../services/upload/upload.service'
import { State } from '../../../services/state/state.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-step3',
  templateUrl: './step3.component.html',
  styleUrls: ['../quickstart.component.css']
})
export class Step3Component implements OnInit {
  @ViewChild('file', { static: false }) file: ElementRef;
  
  uploading = false;

  constructor(
    public state: State, 
    private uploadService: UploadService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
  }

  onFileAdd() {
    const files: FileList = this.file.nativeElement.files;
    if (files.length > 0) {
      const file = files[0];
      this.uploading = true;
      const uploadUrl = this.state.apiBaseUrl + '/templates/' + this.state.templateName + '/mappings/' + this.state.mappingName + '/xls';
      const progress = this.uploadService.upload(uploadUrl, null, file);
      progress.subscribe(end => {
        this.uploading = false;
        this.snackBar.open('Template uploaded succesfully', null, { duration: 2000 });
      }, err => {
        this.uploading = false;
      });
    }
  }

  addFile() {
    this.file.nativeElement.click();
  }

}
