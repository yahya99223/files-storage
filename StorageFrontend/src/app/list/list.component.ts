import {HttpClient} from '@angular/common/http';
import {Component, ViewChild, AfterViewInit} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort, SortDirection} from '@angular/material/sort';
import {merge, Observable, of as observableOf} from 'rxjs';
import {catchError, map, startWith, switchMap} from 'rxjs/operators';
import {FilesService} from '../files.service';
import { FileInfo } from '../FileInfo';
import {MatIconModule} from '@angular/material/icon';
@Component({
  selector: 'list.component',
  styleUrls: ['list.component.css'],
  templateUrl: 'list.component.html',
})
export class ListComponent implements AfterViewInit {
  displayedColumns: string[] = ['order', 'name', 'date', 'url'];
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  data: FileInfo[] = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private filesService: FilesService) {}

  ngAfterViewInit() {
    merge(this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.filesService!.getAll(this.paginator.pageIndex)
            .pipe(catchError(() => observableOf(null)));
        }),
        map(data => {
          this.isLoadingResults = false;
          this.isRateLimitReached = data === null;

          if (data === null) {
            return [];
          }
          this.resultsLength = data.totalCount;
          return data.items;
        })
      ).subscribe(data => this.data = data);
  }
}
