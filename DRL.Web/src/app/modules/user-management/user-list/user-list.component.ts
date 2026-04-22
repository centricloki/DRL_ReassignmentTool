import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { UserModel, ENTRequestModel } from 'src/app/Models/UserModel';
import { UsersService } from '../users.service';
import { AppConstant } from '../../../app.constants';
import { ToasterService } from 'angular2-toaster';
import { TeamModel } from 'src/app/Models/TeamModel';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

declare var $: any;

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit, OnDestroy {

  constructor(private _commonLookupData: CommonService,
    private _router: Router,
    private _usersService: UsersService,
    public _appConstant: AppConstant,
    private _toasterService: ToasterService) { }

  UserList: any[] = [];
  SugarCRMUser = new UserModel();
  myItems: TeamModel[] = [];
  teamModel = new TeamModel();
  entRequestModel = new ENTRequestModel();
  CopyUserList: any[] = [];

  gridView: GridDataResult = {
    data: this.UserList.slice(0, this._appConstant.pageSize),
    total: this.UserList.length
  };
  state: State = {
    skip: 0,
    take: this._appConstant.pageSize
  };
  private unsubscribe$ = new Subject<void>();

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  ngOnInit() {
    this.GetUserList();
    $('a.btnLi').removeClass('active');
    $('#lnkUser').addClass('active');
  }
  GetUserList() {

    this._usersService.GetUserList().pipe(takeUntil(this.unsubscribe$)).subscribe((response: any) => {
      var data = this._commonLookupData.parseData(response);
      this.UserList = data.data;
      this.CopyUserList = data.data;
      this.state = {
        skip: 0,
        take: this._appConstant.pageSize
      };
      this.loadItems();
    });
  }

  loadItems(): void {
    this.gridView = process(this.UserList, this.state);
  }
  dataStateChange(state): void {
    this.state = state;
    this.loadItems();
  }

  deleteUser(Id) {
    this._commonLookupData.confirmDialog('Are you sure you want to delete user?', (result: any) => {
      if (result) {
        this._usersService.GetAllUsersByManagerId(Id).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          var roleData = this._commonLookupData.parseData(res);
          if (roleData.data.length > 0) {
            this._toasterService.pop('error', 'Error', "User can't be deleted as it's assigned to a user");
            return false;
          }
          else {
            var patchRequest = new ENTRequestModel();
            patchRequest.id = Id;
            patchRequest.status = true;
            this._usersService.DeleteUser(patchRequest).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
              var data = this._commonLookupData.parseData(res);
              if (data != null && data != "" && data.isSuccess) {
                this._appConstant.roleId = '';
                this._toasterService.pop('success', 'Success', data.message);
                this.GetUserList();
              }
              else {
                this._toasterService.pop('error', 'Error', data.message);
              }
            }, (error: any) => {
              this._toasterService.pop('error', 'Error', error.message);
            });
          }
        });
      }
    });
  }

  editUser(id) {
    this._appConstant.userId = id;
    this._router.navigate(['/users/create']);
  };

  activeInactiveUser(id, isActive) {
    var status = isActive ? 'Inactive' : 'active';
    this._commonLookupData.confirmDialog('Are you sure you want to ' + status + ' the user?', (result: any) => {
      if (result) {
        if (isActive == 1) {
          this._usersService.GetAllUsersByManagerId(id).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
            var roleData = this._commonLookupData.parseData(res);
            if (roleData.data.length > 0) {
              this._toasterService.pop('error', 'Error', "User can't be deactivated as it's assigned to a user");
              return false;
            }
            else {
              this.entRequestModel.id = id;
              this.entRequestModel.status = !isActive;
              this._usersService.ManageUserStatus(this.entRequestModel).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
                var data = this._commonLookupData.parseData(res);
                if (data != null && data != "" && data.isSuccess) {
                  this._appConstant.userId = '';
                  this._toasterService.pop('success', 'Success', data.message);
                  this.GetUserList();
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
        else {
          this.entRequestModel.id = id;
          this.entRequestModel.status = !isActive;
          this._usersService.ManageUserStatus(this.entRequestModel).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
            var data = this._commonLookupData.parseData(res);
            if (data != null && data != "" && data.isSuccess) {
              this._appConstant.userId = '';
              this._toasterService.pop('success', 'Success', data.message);
              this.GetUserList();
            }
            else {
              this._toasterService.pop('error', 'Error', data.message);
            }
          }, (error: any) => {
            this._toasterService.pop('error', 'Error', error.message);
          });
        }


      }
    });
  };

  addUpdateUser(user) {
    //user.createdBy = localStorage["userName"];
    user.createdBy = "0";
    user.createdDate = new Date();
    //user.updatedBy = localStorage["userName"];
    user.updatedBy = "0";
    user.updatedDate = new Date();
    this._usersService.ManageUser(user).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data != null && data != "" && data.isSuccess) {
        this._appConstant.userId = '';
        this._toasterService.pop('success', 'Success', data.message);

      }
      else {
        this._toasterService.pop('error', 'Error', data.message);
      }
    }, (error: any) => {
      this._toasterService.pop('error', 'Error', error.message);
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
}
