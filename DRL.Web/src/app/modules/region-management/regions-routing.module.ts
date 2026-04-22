import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { RegionListComponent } from './region-list/region-list.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';
import { ManageRegionComponent } from './manage-region/manage-region.component';


export const RegionRoutes: Routes = [
  {
    path: '',
    component: RegionListComponent,
    data: {
      pageTitle: 'Region Management',
      linkCode: 'REGIONS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'create',
    component: ManageRegionComponent,
    data: {
      pageTitle: 'Region Management',
      linkCode: 'REGIONS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(RegionRoutes)
  ]
})
export class RegionsRoutingModule { }
