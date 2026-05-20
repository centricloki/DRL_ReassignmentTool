import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';

import { BdListComponent } from './bd-list/bd-list.component';
import { ManageBdComponent } from './manage-bd/manage-bd.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';

export const BdRoutes: Routes = [
  {
    path: '',
    component: BdListComponent,
    data: {
      pageTitle: 'BDM',
      linkCode: 'BDS'
    },
    canActivate: [AuthGuardService]
  },
  {
    path: 'create',
    component: ManageBdComponent,
    data: {
      pageTitle: 'Create BDM',
      linkCode: 'BDS',
      mode: 'create'
    },
    canActivate: [AuthGuardService]
  },
  {
    path: 'edit',
    component: ManageBdComponent,
    data: {
      pageTitle: 'Edit BDM',
      linkCode: 'BDS',
      mode: 'edit'
    },
    canActivate: [AuthGuardService]
  },
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full'
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(BdRoutes)
  ],
  exports: [RouterModule]
})
export class BdRoutingModule { }
