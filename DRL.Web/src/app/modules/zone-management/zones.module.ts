import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ZonesRoutingModule } from './zones-routing.module';
import { ZoneListComponent } from './zone-list/zone-list.component';
import { ManageZoneComponent } from './manage-zone/manage-zone.component';

@NgModule({
  declarations: [ZoneListComponent, ManageZoneComponent],
  imports: [
    CommonModule,
    ZonesRoutingModule
  ]
})
export class ZonesModule { }
