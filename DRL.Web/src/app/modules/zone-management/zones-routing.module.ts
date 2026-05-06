import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ZoneListComponent } from './zone-list/zone-list.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';
import { ManageZoneComponent } from './manage-zone/manage-zone.component';


export const ZoneRoutes: Routes = [
  {
    path: '',
    component: ZoneListComponent,
    data: {
      pageTitle: 'Zone Management',
      linkCode: 'ZONES'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'create',
    component: ManageZoneComponent,
    data: {
      pageTitle: 'Zone Management',
      linkCode: 'ZONES'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(ZoneRoutes)
  ]
})
export class ZonesRoutingModule { }