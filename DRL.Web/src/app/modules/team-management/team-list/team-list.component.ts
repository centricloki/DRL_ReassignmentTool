import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TeamModel } from 'src/app/Models/TeamModel';
import { Router } from '@angular/router';
import { CommonService } from 'src/app/services/common.service';
import { TeamsService } from '../teams.service';
import { AppConstant } from 'src/app/app.constants';
import { ToasterService } from 'angular2-toaster';
import { ENTRequestModel } from 'src/app/Models/UserModel';
import { UsersService } from '../../user-management/users.service';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';


@Component({
  selector: 'app-team-list',
  templateUrl: './team-list.component.html',
  styleUrls: ['./team-list.component.css']
})
export class TeamListComponent implements OnInit, OnDestroy {

  titleText: string;
  TeamList: any[] = [];
  CopyTeamList: any[] = [];
  SugarCRMTeam = new TeamModel();
  @ViewChild('formTeam') TeamInfoForm: NgForm;

  gridView: GridDataResult = {
    data: this.TeamList.slice(0, this._appConstant.pageSize),
    total: this.TeamList.length
  };
  state: State = {
    skip: 0,
    take: this._appConstant.pageSize
  };
  private unsubscribe$ = new Subject<void>();

  constructor(private _commonLookupData: CommonService,
    private _router: Router,
    private _teamService: TeamsService,
    public _appConstant: AppConstant,
    private _toasterService: ToasterService,
    private _usersService: UsersService) { }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit() {
    this.GetAllTerritories();
  }
  canDeactivate(): Promise<boolean> | boolean {
    return this.TeamInfoForm.dirty && this.TeamInfoForm.touched;
  };

  cancelTeamClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this.TeamInfoForm.reset();
          this._router.navigate(['/teams']);
        }
      });
    } else
      this._router.navigate(['/teams']);
  }

  GetAllTerritories() {
    this._teamService.GetAllTerritories().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.TeamList = data.data;
      this.CopyTeamList = data.data;
      this.state = {
        skip: 0,
        take: this._appConstant.pageSize
      };
      this.loadItems();
    });
  }

  deleteTeam(Id) {
    this._commonLookupData.confirmDialog('Are you sure you want to delete team?', (result: any) => {
      if (result) {
        this._usersService.GetAllUsersByTerritoryId(Id).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          var roleData = this._commonLookupData.parseData(res);
          if (roleData.data.length > 0) {
            this._toasterService.pop('error', 'Error', "Team can't be deleted as it's assigned to a user");
            return false;
          }
          else {
            var patchRequest = new ENTRequestModel();
            patchRequest.id = Id;
            patchRequest.status = true;
            this._teamService.DeleteTeam(patchRequest).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
              var data = this._commonLookupData.parseData(res);
              if (data != null && data != "" && data.isSuccess) {
                this._appConstant.teamId = '';
                this._toasterService.pop('success', 'Success', data.message);
                this.GetAllTerritories();
              }
              else {
                this._toasterService.pop('error', 'Error', data.message);
              }
            }, (error: any) => {
              this._toasterService.pop('error', 'Error', error.message);
            });
          }
        }, (error: any) => {
          this._toasterService.pop('error', 'Error', error.message);
          return false;
        });
      }
    });
  }

  editTeam(id) {
    this._appConstant.teamId = id;
    this._router.navigate(['/teams/create']);
  };

  viewTeam(id, teamName) {
    this._appConstant.teamId = id;
    this._appConstant.teamName = teamName;
    this._router.navigate(['/teams/view']);
  }
  activeInactiveTeam(teamModel) {
    var status = teamModel.isActive ? 'Inactive' : 'active';
    this._commonLookupData.confirmDialog('Are you sure you want to ' + status + ' the team?', (result: any) => {
      if (result) {
        if (teamModel.isActive) {
          this._usersService.GetAllUsersByTerritoryId(teamModel.teamId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
            var roleData = this._commonLookupData.parseData(res);
            if (roleData.data.length > 0) {
              this._toasterService.pop('error', 'Error', "Team can't be deleted as it's assigned to a user");
              return false;
            }
            else {
              teamModel.isActive = !teamModel.isActive;
              teamModel.updatedDate = new Date();
              this.addUpdateTeam(teamModel);
            }
          }, (error: any) => {
            this._toasterService.pop('error', 'Error', error.message);
            return false;
          });
        }
        else {
          teamModel.isActive = !teamModel.isActive;
          teamModel.updatedDate = new Date();
          this.addUpdateTeam(teamModel);
        }
      }
    });
  }

  loadItems(): void {
    this.gridView = process(this.TeamList, this.state);
  }
  dataStateChange(state): void {
    this.state = state;
    this.loadItems();
  }

  addUpdateTeam(user) {
    this._teamService.ManageTeam(user).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data != null && data != "" && data.isSuccess) {
        this._appConstant.userId = '';
        this._toasterService.pop('success', 'Success', data.message);
        this.GetAllTerritories();
      }
    }
      , (error: any) => {
        this._toasterService.pop('error', 'Error', error.message);
      });
  }

  activeInactiveFilterChange(e) {
    this.TeamList = this.CopyTeamList;
    if (e.value == 1)//active
    {
      this.TeamList = this.TeamList.filter(x => x.isActive == 1);
      return this.TeamList;
    }
    else if (e.value == 0)//inactive
    {
      this.TeamList = this.TeamList.filter(x => x.isActive == 0);
      return this.TeamList;
    }
    else {
      return this.TeamList;
    }
  }
}

