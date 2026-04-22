import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersService } from './users.service';
import { UserListComponent } from './user-list/user-list.component';
import { ManageUserComponent } from './manage-user/manage-user.component';
import { UsersRoutingModule } from './users-routing.module';
import { RouterModule } from '@angular/router';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule, MatInputModule, MatListModule, MatExpansionPanel, MatExpansionModule, MatCheckboxModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GridModule, FilterMenuModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

@NgModule({
  declarations:
    [
      ManageUserComponent,
      UserListComponent,
      
    ],
  imports: [
    CommonModule,
    UsersRoutingModule,
    RouterModule,
    MatRippleModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatListModule,
    MatSelectModule,
    MatExpansionModule,
    MatCheckboxModule,
    GridModule,
    DropDownsModule,
  FilterMenuModule,
  NgxMatSelectSearchModule
  ],
  providers: [UsersService],
  bootstrap: [ManageUserComponent]
})
export class UsersModule { }
