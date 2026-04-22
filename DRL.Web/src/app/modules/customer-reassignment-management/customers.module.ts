import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserReassignmentComponent } from './user-reassignment/user-reassignment.component';
import { CustomerReassignmentComponent } from './customer-reassignment/customer-reassignment.component';
import { ActionHistoryComponent } from './action-history/action-history.component';
import { CustomersRoutingModule } from './customers-routing.module';
import { RouterModule } from '@angular/router';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule, MatInputModule, MatListModule, MatExpansionModule, MatCheckboxModule, MatAutocompleteModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GridModule ,ExcelModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { CustomersService } from './customers.service';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { DatePipe } from '@angular/common';
import { UsersService } from '../user-management/users.service';
import { CustomerImportComponent } from './customer-import/customer-import.component';
import { BrandStyleComponent } from './brand-style/brand-style.component';
import { UserReportComponent } from './user-report/user-report.component';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  declarations: [
    UserReassignmentComponent,
    CustomerReassignmentComponent,
    ActionHistoryComponent,
    CustomerImportComponent,
    BrandStyleComponent,
    UserReportComponent
  ],
  imports: [
    CommonModule,
    CommonModule,
    CustomersRoutingModule,
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
    DatePickerModule,
    MatCheckboxModule,
    ExcelModule,
    TooltipModule,
    NgxMatSelectSearchModule,
    MatAutocompleteModule,
    MatTooltipModule
  ],
  providers: [CustomersService,DatePipe,UsersService],
  bootstrap: [CustomerReassignmentComponent]
})
export class CustomersModule { }
