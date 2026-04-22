import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { RoleListComponent } from './role-list/role-list.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';
import { ManageRoleComponent } from './manage-role/manage-role.component';


export const RoleRoutes: Routes = [
  {
    path: '',
    component: RoleListComponent,
    data: {
      pageTitle: 'Role Management',
      linkCode: 'ROLES'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'create',
    component: ManageRoleComponent,
    data: {
      pageTitle: 'Role Management',
      linkCode: 'ROLES'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(RoleRoutes)
  ]
})
export class RolesRoutingModule { }
