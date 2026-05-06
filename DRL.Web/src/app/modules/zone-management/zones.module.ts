import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ZonesRoutingModule } from './zones-routing.module';
import { ZoneListComponent } from './zone-list/zone-list.component';
import { ManageZoneComponent } from './manage-zone/manage-zone.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule } from '@angular/material';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { InputsModule } from '@progress/kendo-angular-inputs';

@NgModule({
  declarations: [ZoneListComponent, ManageZoneComponent],
  imports: [
    CommonModule,
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
