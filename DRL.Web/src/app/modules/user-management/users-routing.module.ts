import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthGuardService } from 'src/app/services/auth-guard.service';
import { RouterModule, Routes } from '@angular/router';
import { ManageUserComponent } from './manage-user/manage-user.component';
import { UserListComponent } from './user-list/user-list.component';

export const UserRoutes: Routes = [
  {
    path: '',
    component: UserListComponent,
    data: {
      pageTitle: 'User Management',
      linkCode: 'USERS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'create',
    component: ManageUserComponent,
    data: {
      pageTitle: 'User Management',
      linkCode: 'USERS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(UserRoutes)
  ]
})
export class UsersRoutingModule { }
