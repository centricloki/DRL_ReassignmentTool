import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamsRoutingModule } from './teams-routing.module';
import { TeamListComponent } from './team-list/team-list.component';
import { TeamsService } from './teams.service';
import { ManageTeamComponent } from './manage-team/manage-team.component';
import { MatRippleModule, MatTableModule, MatPaginatorModule, MatSortModule, MatSelectModule, MatInputModule, MatListModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { GridModule } from '@progress/kendo-angular-grid';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { AssignTeamUserComponent } from './assign-team-user/assign-team-user.component';
import { UsersService } from '../user-management/users.service';

@NgModule({
  declarations:
   [
     TeamListComponent,
     ManageTeamComponent,
     AssignTeamUserComponent
    ],
  imports: [
    CommonModule,
    RouterModule,
    TeamsRoutingModule,
    MatRippleModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatListModule,
    MatSelectModule,
    GridModule,
    DropDownsModule
  ],
  providers: [TeamsService,UsersService],
  bootstrap: [ManageTeamComponent]
})
export class TeamsModule { }
