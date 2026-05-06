import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
<<<<<<< HEAD
import { RouterModule } from '@angular/router';
=======
>>>>>>> 44e73274ef4fe5819e1894c044f07ad55a87e462
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ZonesRoutingModule } from './zones-routing.module';
import { ZoneListComponent } from './zone-list/zone-list.component';
import { ManageZoneComponent } from './manage-zone/manage-zone.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule } from '@angular/material';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { InputsModule } from '@progress/kendo-angular-inputs';
<<<<<<< HEAD

=======
>>>>>>> 44e73274ef4fe5819e1894c044f07ad55a87e462

@NgModule({
  declarations: [ZoneListComponent, ManageZoneComponent],
  imports: [
    CommonModule,
<<<<<<< HEAD
    RouterModule,
=======
>>>>>>> 44e73274ef4fe5819e1894c044f07ad55a87e462
    ZonesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MatRippleModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSelectModule,
    GridModule,
    DropDownsModule,
    TooltipModule,
    InputsModule
  ]
})
export class ZonesModule { }
