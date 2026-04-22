import { Component, OnInit, ViewChild } from '@angular/core';
import { UsersService } from '../../user-management/users.service';
import { AppConstant } from '../../../app.constants';
import { CommonService } from 'src/app/services/common.service';
import { UserModel } from 'src/app/Models/UserModel';
import { NgForm } from '@angular/forms';
import { ToasterService } from 'angular2-toaster';
import { Router } from '@angular/router';

@Component({
  selector: 'app-assign-team-user',
  templateUrl: './assign-team-user.component.html',
  styleUrls: ['./assign-team-user.component.css']
})
export class AssignTeamUserComponent implements OnInit {

  constructor(private _commonLookupData: CommonService,
    private _router: Router,
    private _usersService: UsersService,
    public _appConstant: AppConstant,
    private _toasterService: ToasterService) { }

  UserList: any[] = [];
  CopyUserList: any[] = [];
  ManagerList:any[]=[];
  CopyManagerList:any[]=[];
  ReportsToList: Array<any>;
  SugarCRMUser = new UserModel();
  @ViewChild('formUser') userInfoForm: NgForm;

  ngOnInit() {
    this.GetAllUsersByTerritoryId();
    this.GetAllUsers();
  }

  GetAllUsersByTerritoryId() {
    this._usersService.GetAllUsersByTerritoryId(this._appConstant.teamId).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.UserList = data.data;
      this.CopyUserList = data.data;
    });
  }

  GetAllUsers() {
    this._commonLookupData.GetAllUsers().subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.ReportsToList = data.data;
    });
  }

  activeInactiveFilterChange(e) {
    this.UserList = this.CopyUserList;
    if (e.value == 1)//active
    {
      this.UserList = this.UserList.filter(x => x.isActive == 1);
      return this.UserList;
    }
    else if (e.value == 0)//inactive
    {
      this.UserList = this.UserList.filter(x => x.isActive == 0);
      return this.UserList;
    }
    else {
      return this.UserList;
    }
  }

  UpdateUserTerritory() {
    if (this.UserList.find(x => x.userId == this.SugarCRMUser.managerId && x.isTerritoryUser == true)) {
      this._toasterService.pop('error', 'Error', "User already exist in team");
    }
    else {
      this._usersService.UpdateUserTerritory(this.SugarCRMUser.managerId, this._appConstant.teamId).subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        if (data != null && data != "" && data.isSuccess) {
          this._toasterService.pop('success', 'Success', data.message);
          this.GetAllUsersByTerritoryId();
        }
        else {
          this._toasterService.pop('error', 'error', data.message);
        }
      }
        , (error: any) => {
          this._toasterService.pop('error', 'Error', error.message);
        });
    }
  }

  DeleteUserTerritory(userId) {
    this._commonLookupData.confirmDialog('Are you sure you want to remove user?', (result: any) => {
      if (result) {
        this._usersService.DeleteUserTerritory(userId, this._appConstant.teamId).subscribe(response => {
          var data = this._commonLookupData.parseData(response);
          if (data != null && data != "" && data.isSuccess) {
            this._toasterService.pop('success', 'Success', data.message);
            this.GetAllUsersByTerritoryId();
          }
          else {
            this._toasterService.pop('error', 'error', data.message);
          }
        }
          , (error: any) => {
            this._toasterService.pop('error', 'Error', error.message);
          });
      }
    });
  }


  canDeactivate(): Promise<boolean> | boolean {
    return this.userInfoForm.dirty && this.userInfoForm.touched;
  };

  cancelClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this._appConstant.teamId = '';
          this.userInfoForm.reset();
          this._router.navigate(['/teams']);
        }
      });
    } else {
      this._appConstant.teamId = '';
      this._router.navigate(['/teams']);
    }

  }

  GetUsersByTerritoryIdAndUserId(e,grid ){
    this._usersService.GetUsersByTerritoryIdAndUserId(this._appConstant.teamId,e.dataItem.userId).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.ManagerList = data.data;
      this.CopyManagerList = data.data;
      this.UserList.forEach((item, idx) => {
        if(idx!=e.index)
        {
          grid.collapseRow(idx);
        }
      })

    });
  }
}
