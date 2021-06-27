import {Component, OnInit, ViewChild, ElementRef} from '@angular/core';
import {HttpEventType, HttpErrorResponse} from '@angular/common/http';
import {of} from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {FilesService} from '../files.service';
@Component({
  selector: 'app-home',
  templateUrl: './AddFile.component.html',
  styleUrls: ['./AddFile.component.css']
})
export class AddFileComponent implements OnInit {
@ViewChild("fileUpload", {static: false}) fileUpload: ElementRef;
files = [];
errors=[];
constructor(private filesService: FilesService) { }

  ngOnInit(): void {
  }
uploadFile(file) {
    const formData = new FormData();  
    formData.append('file', file.data);  
    file.inProgress = true;
    this.filesService.upload(formData).pipe(
      map(event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            file.progress = Math.round(event.loaded * 100 / event.total);
            break;
          case HttpEventType.Response:
            return event;
        }  
      }),  
      catchError((error: HttpErrorResponse) => {
        file.inProgress = false;
		this.errors.push(error.error);
        return of(`Upload failed: ${file.data.name}`);
      })).subscribe((event: any) => {
        if (typeof (event) === 'object') {
          console.log(event.body);
        }  
      });  
  }
  onClick() {
    const fileUpload = this.fileUpload.nativeElement;fileUpload.onchange = () => {
    for (let index = 0; index < fileUpload.files.length; index++)
    {
     const file = fileUpload.files[index];
     this.files.push({ data: file, inProgress: false, progress: 0});
    }
      this.uploadFiles();
    };
    fileUpload.click();
}
private uploadFiles() {  
    this.fileUpload.nativeElement.value = '';  
    this.files.forEach(file => {  
      this.uploadFile(file);  
    });  
}
}

