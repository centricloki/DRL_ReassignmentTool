import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { RoleModel } from 'src/app/Models/RoleModel';
import { AppConstant } from 'src/app/app.constants';
import { RolesService } from '../roles.service';
import { ToasterService } from 'angular2-toaster';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manage-role',
  templateUrl: './manage-role.component.html',
  styleUrls: ['./manage-role.component.css']
})
export class ManageRoleComponent implements OnInit, OnDestroy {

  constructor(public _commonLookupData: CommonService, private _router: Router, public _appConstant: AppConstant, private _roleService: RolesService
    , private _toasterService: ToasterService) { }
  ReportsToList: Array<any>;
  RoleList: Array<any>;
  titleText: string;
  navTitleText: string;
  SugarRole = new RoleModel();
  @ViewChild('formRole') roleInfoForm: NgForm;
  private unsubscribe$ = new Subject<void>();

  ngOnDestroy() {
    this._appConstant.roleId = null;
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit() {
    this.titleText = "Role Info";
    if (!this._appConstant.roleId || this._appConstant.roleId == '') {
      this.navTitleText = "Create Role";
    }
    else {
      this.navTitleText = "Edit Role";
      this.getRoleDetails(this._appConstant.roleId);
    }
  }
  canDeactivate(): Promise<boolean> | boolean {
    return this.roleInfoForm.dirty && this.roleInfoForm.touched;
  };

  cancelRoleClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this._appConstant.roleId = '';
          this.roleInfoForm.reset();
          this._router.navigate(['/roles']);
        }
      });
    } else {
      this._appConstant.roleId = '';
      this._router.navigate(['/roles']);
    }

  }

  addUpdateRole(sugarRoleData) {
    sugarRoleData.createdBy = "0";
    sugarRoleData.createdDate = new Date();
    sugarRoleData.updatedBy = "0";
    sugarRoleData.updatedDate = new Date();  
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
  saveRole() {
    this.addUpdateRole(this.SugarRole);
  }
  getRoleDetails(id) {
    this._roleService.GetRoleDetails(id).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res)
      if (data.isSuccess) {
        this.SugarRole = data.data;
      }
    });
  }


}
