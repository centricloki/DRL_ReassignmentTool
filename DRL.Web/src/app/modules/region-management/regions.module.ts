import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegionsRoutingModule } from './regions-routing.module';
import { RegionService } from './region.service';
import { RegionListComponent} from './region-list/region-list.component';
import { ManageRegionComponent } from './manage-region/manage-region.component';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { UsersService } from '../user-management/users.service';

@NgModule({
  declarations: 
  [
    ManageRegionComponent,
    RegionListComponent
  ],
  imports: [
    CommonModule,
    RegionsRoutingModule,
    MatRippleModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    GridModule,
    DropDownsModule,
    MatSelectModule
  ],
  providers: [RegionService,UsersService],
  bootstrap: [RegionListComponent]
})
export class RegionsModule { }
