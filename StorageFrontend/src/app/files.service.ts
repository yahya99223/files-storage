import {HttpClient, HttpEvent, HttpErrorResponse, HttpEventType} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from  'rxjs/operators';
import { Observable } from 'rxjs';
import { FileInfo } from './FileInfo';
@Injectable({
  providedIn: 'root'
})
export class FilesService {
SERVER_URL: string = "https://localhost:5001/files";
constructor(private httpClient: HttpClient) { }
public upload(formData) {
    return this.httpClient.post<any>(this.SERVER_URL, formData, {
      reportProgress: true,
      observe: 'events'
    });
}
getAll(page: number): Observable<FileInfo[]> {	
	   const requestUrl =`${this.SERVER_URL}?page=${page + 1}`;
return this.httpClient.get<FileInfo[]>(requestUrl);
}
}
