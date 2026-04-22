import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ManageTeamComponent } from './manage-team/manage-team.component';
import { TeamListComponent } from './team-list/team-list.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';
import { AssignTeamUserComponent } from './assign-team-user/assign-team-user.component';

export const TeamRoutes: Routes = [
  {
    path: '',
    component: TeamListComponent,
    data: {
      pageTitle: 'Team Management',
      linkCode: 'TEAMS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'create',
    component: ManageTeamComponent,
    data: {
      pageTitle: 'Team Management',
      linkCode: 'TEAMS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  ,
  {
    path: 'view',
    component: AssignTeamUserComponent,
    data: {
      pageTitle: 'Assign Team User',
      linkCode: 'TEAMS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(TeamRoutes)
  ]
})
export class TeamsRoutingModule { }
