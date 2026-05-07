import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';

import { ZoneListComponent } from './zone-list/zone-list.component';
import { ManageZoneComponent } from './manage-zone/manage-zone.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';

// 🗂️ Centralized route configuration
export const ZoneRoutes: Routes = [
  {
    path: '',
    component: ZoneListComponent,
    data: {
      pageTitle: 'Zone Management',
      linkCode: 'ZONES'  // 🔐 ACL: Restrict access by linkCode
    },
    canActivate: [AuthGuardService]
  },
  {
    path: 'create',
    component: ManageZoneComponent,
    data: {
      pageTitle: 'Create Zone',
      linkCode: 'ZONES',
      mode: 'create'  // 💡 Pass mode to component for form logic
    },
    canActivate: [AuthGuardService]
  },
  {
    path: 'edit',            // ← ✅ Added edit route with ID parameter
    component: ManageZoneComponent,
    data: {
      pageTitle: 'Edit Zone',
      linkCode: 'ZONES',
      mode: 'edit'
    },
    canActivate: [AuthGuardService]
  },
  {
    path: '**',                 // ← Optional: catch-all for invalid paths
    redirectTo: '',             // Redirect to list instead of hard 404
    pathMatch: 'full'
  }
];

@NgModule({
  declarations: [],             // ⚠️ Components declared in ZonesModule, NOT here
  imports: [
    CommonModule,
    RouterModule.forChild(ZoneRoutes)  // ✅ Use forChild() in lazy-loaded modules
  ],
  exports: [RouterModule]       // ✅ Export RouterModule for component access to router directives
})
export class ZonesRoutingModule { }