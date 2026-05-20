import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BdRoutingModule } from './bd-routing.module';
import { BdListComponent } from './bd-list/bd-list.component';
import { ManageBdComponent } from './manage-bd/manage-bd.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule, MatExpansionModule, MatInputModule, MatTooltipModule } from '@angular/material';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { UsersService } from '../user-management/users.service';

@NgModule({
  declarations: [BdListComponent, ManageBdComponent],
  imports: [
    CommonModule,
    RouterModule,
    BdRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MatRippleModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSelectModule,
    MatExpansionModule,
    MatInputModule,
    MatTooltipModule,
    GridModule,
    DropDownsModule,
    TooltipModule,
    InputsModule,
    NgxMatSelectSearchModule
  ],
  providers: [UsersService]
})
export class BdManagementModule { }
