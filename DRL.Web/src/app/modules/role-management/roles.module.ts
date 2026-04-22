import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RolesRoutingModule } from './roles-routing.module';
import { RolesService } from './roles.service';
import { RoleListComponent } from './role-list/role-list.component';
import { ManageRoleComponent } from './manage-role/manage-role.component';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { UsersService } from '../user-management/users.service';

@NgModule({
  declarations: 
  [
    ManageRoleComponent,
    RoleListComponent
  ],
  imports: [
    CommonModule,
    RolesRoutingModule,
    MatRippleModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    GridModule,
    DropDownsModule
  ],
  providers: [RolesService,UsersService],
  bootstrap: [RoleListComponent]
})
export class RolesModule { }
