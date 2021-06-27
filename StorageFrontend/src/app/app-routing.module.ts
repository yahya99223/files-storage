import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AddFileComponent} from './AddFile/AddFile.component';
import {ListComponent} from './list/list.component';
const routes: Routes = [  
  {path: '', redirectTo: 'list', pathMatch: 'full'},
  {path: 'add', component: AddFileComponent},
  {path: 'list', component: ListComponent}
  ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }