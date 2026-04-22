import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort } from '@angular/material';
import { RoleModel } from 'src/app/Models/RoleModel';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { AppConstant } from 'src/app/app.constants';
import { RolesService } from '../roles.service';
import { ManageRoleComponent } from '../manage-role/manage-role.component';
import { ToasterService } from 'angular2-toaster';
import { ENTRequestModel } from 'src/app/Models/UserModel';
import { UsersService } from '../../user-management/users.service';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})
export class RoleListComponent implements OnInit, OnDestroy {

  constructor(private _commonLookupData: CommonService, private _router: Router, public _appConstant: AppConstant, private _roleService: RolesService, private _toasterService: ToasterService, private _userService: UsersService) { }
  private unsubscribe$ = new Subject<void>();
  roleList: any[] = [];
  CopyRoleList: any[] = [];

  gridView: GridDataResult = {
    data: this.roleList.slice(0, this._appConstant.pageSize),
    total: this.roleList.length
  };
  state: State = {
    skip: 0,
    take: this._appConstant.pageSize
  };


  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit() {
    this.GetAllRoles();
  }

  GetAllRoles() {
    this._roleService.GetAllRoles().pipe(takeUntil(this.unsubscribe$)).subscribe(data => {
      if (data.ok) {
        var Data = JSON.stringify(data._body);
        var Obj = JSON.parse(Data);
        var data = JSON.parse(Obj);
        if (data.isSuccess) {
          this.roleList = data.data;
          this.CopyRoleList = data.data;
          this.state = {
            skip: 0,
            take: this._appConstant.pageSize
          };
          this.loadItems();
        }
      }
    });
  }

  loadItems(): void {
    this.gridView = process(this.roleList, this.state);
  }
  dataStateChange(state): void {
    this.state = state;
    this.loadItems();
  }

  activeInactiveRole(roleDetails: RoleModel) {
    var status = roleDetails.isActive ? 'Inactive' : 'active';
    this._commonLookupData.confirmDialog('Are you sure you want to ' + status + ' the role?', (result: any) => {
      if (result) {
        if (roleDetails.isActive) {
          this._userService.GetAllUsersByRoleId(roleDetails.roleId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
            var roleData = this._commonLookupData.parseData(res);
            if (roleData != null && roleData.isSuccess && roleData.data.length > 0) {
              this._toasterService.pop('error', 'Error', "Role can't be deactivated as it's assigned to a user");
            }
            else {
              roleDetails.isActive = !roleDetails.isActive;
              roleDetails.updatedDate = new Date();
              this.addUpdateRole(roleDetails);
            }
          }, (error: any) => {
            this._toasterService.pop('error', 'Error', error.message);
            return true;
          });
        }
        else {
          roleDetails.isActive = !roleDetails.isActive;
          roleDetails.updatedDate = new Date();
          this.addUpdateRole(roleDetails);
        }
      }
    });
  }


  // deleteRole(Id) {
  //   this._commonLookupData.confirmDialog('Are you sure you want to delete role?', (result: any) => {
  //     if (result) {
  //       this._userService.GetAllUsersByRoleId(Id).subscribe(res => {
  //         var roleData = this._commonLookupData.parseData(res);
  //         if (roleData != null && roleData.isSuccess && roleData.data.length > 0) {
  //           this._toasterService.pop('error', 'Error', "Role can't be deleted as it's assigned to a user");
  //         }
  //         else {
  //           var patchRequest = new ENTRequestModel();
  //           patchRequest.id = Id;
  //           patchRequest.status = true;
  //           this._roleService.DeleteRole(patchRequest).subscribe(res => {
  //             var data = this._commonLookupData.parseData(res);
  //             if (data != null && data != "" && data.isSuccess) {
  //               this._appConstant.roleId = '';
  //               this._toasterService.pop('success', 'Success', data.message);
  //               this.GetAllRoles();
  //             }
  //             else {
  //               this._toasterService.pop('error', 'Error', data.message);
  //             }
  //           }, (error: any) => {
  //             this._toasterService.pop('error', 'Error', error.message);
  //           });
  //         }
  //       }, (error: any) => {
  //         this._toasterService.pop('error', 'Error', error.message);
  //         return true;
  //       });
  //     }
  //   });
  // }
  editRole(id) {
    this._appConstant.roleId = id;
    this._router.navigate(['/roles/create']);
  };

  addUpdateRole(sugarRoleData) {
    this._roleService.manageRole(sugarRoleData).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data != null && data != "" && data.isSuccess) {
        this._appConstant.roleId = '';
        this._toasterService.pop('success', 'Success', data.message);
        this._router.navigate(["/roles"])
      }
      else {
        this._toasterService.pop('error', 'Error', data.message);
      }
    }, (error: any) => {
      this._toasterService.pop('error', 'Error', error.message);
    });
  }

  activeInactiveFilterChange(e) {
    this.roleList = this.CopyRoleList;
    if (e.value == 1)//active
    {
      this.roleList = this.roleList.filter(x => x.isActive == 1);
      return this.roleList;
    }
    else if (e.value == 0)//inactive
    {
      this.roleList = this.roleList.filter(x => x.isActive == 0);
      return this.roleList;
    }
    else {
      return this.roleList;
    }
  }
}

