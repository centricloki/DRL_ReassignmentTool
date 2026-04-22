import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TeamModel } from 'src/app/Models/TeamModel';
import { Router } from '@angular/router';
import { CommonService } from 'src/app/services/common.service';
import { TeamsService } from '../teams.service';
import { AppConstant } from 'src/app/app.constants';
import { ToasterService } from 'angular2-toaster';
import { UsersService } from '../../user-management/users.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manage-team',
  templateUrl: './manage-team.component.html',
  styleUrls: ['./manage-team.component.css']
})
export class ManageTeamComponent implements OnInit {

  titleText: string;
  SugarCRMTeam = new TeamModel();
  RegionList: Array<any>;
  TeamStatusList: Array<any>;
  private unsubscribe$ = new Subject<void>();
  @ViewChild('formTeam') TeamInfoForm: NgForm;


  constructor(private _commonLookupData: CommonService,
    private _router: Router,
    private _teamService: TeamsService,
    public _appConstant: AppConstant,
    private _toasterService: ToasterService, private _usersService: UsersService) { }

  ngOnDestroy() {
    this._appConstant.teamId = null;
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  ngOnInit() {
    if (!this._appConstant.teamId || this._appConstant.teamId == '') {
      this.titleText = "Create Team";
      this.SugarCRMTeam.teamStatusId = 'true';
    }
    else {
      this.titleText = "Edit Team";
    }

    this.GetAllRegionList();
    this.GetAllTeamStatusList();
    if (this._appConstant.teamId != '' && this._appConstant.teamId != null) {
      this.GetTerritory();
    }
  }

  GetAllTeamStatusList() {
    this.TeamStatusList = [{ "recordId": "true", "value": "Active" }, { "recordId": "false", "value": "Inactive" }]
  }
  canDeactivate(): Promise<boolean> | boolean {
    return this.TeamInfoForm.dirty && this.TeamInfoForm.touched;
  };

  GetAllRegionList() {
    this._commonLookupData.GetAllRegionList().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.RegionList = data.data;
    });
  }

  cancelTeamClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this._appConstant.teamId = '';
          this.TeamInfoForm.reset();
          this._router.navigate(['/teams']);
        }
      });
    } else {
      this._appConstant.teamId = '';
      this._router.navigate(['/teams']);
    }

  }

  saveTeam() {
    this.SugarCRMTeam.teamId = this._appConstant.teamId;
    if (this.SugarCRMTeam.regionId == '') {
      this.SugarCRMTeam.regionId = '0';
    }
    this.SugarCRMTeam.isActive = this.SugarCRMTeam.teamStatusId == "true" ? true : false;
    if (this._appConstant.teamId != null && this._appConstant.teamId != '0' && this._appConstant.teamId != '') {
      if (!this.SugarCRMTeam.isActive) {
        this._usersService.GetAllUsersByTerritoryId(this.SugarCRMTeam.teamId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          var roleData = this._commonLookupData.parseData(res);
          if (roleData.data.length > 0) {
            this._toasterService.pop('error', 'Error', "Team can not be deactivated as it is assigned to one of the user");
            return false;
          }
          else {
            this.ManageTeam();
          }
        }, (error: any) => {
          this._toasterService.pop('error', 'Error', error.message);
          return false;
        });
      }
      else {
        this.ManageTeam();
      }
    }
    else {
      this.ManageTeam();
    }
  }
  ManageTeam() {
    //Added by Senthil Ramadoss on 5/13/2020
    //console.log(this.SugarCRMTeam);
    //this.SugarCRMTeam.createdBy = localStorage["userName"];
    this.SugarCRMTeam.createdBy =this.SugarCRMTeam.updatedBy= "0";
    this.SugarCRMTeam.createdDate = new Date();
    //this.SugarCRMTeam.updatedBy = localStorage["userName"];
    this.SugarCRMTeam.updateDate = new Date();
    this._teamService.ManageTeam(this.SugarCRMTeam).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data != null && data != "" && data.isSuccess) {
        this._toasterService.pop('success', 'Success', data.message);
        this._router.navigate(['/teams']);
      }
      else {
        this._toasterService.pop('error', 'Error', data.message);
      }
    }
      , (error: any) => {
        this._toasterService.pop('error', 'Error', error.message);
      });
  }

  GetTerritory() {
    this._teamService.GetTerritory(this._appConstant.teamId).pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.SugarCRMTeam = data.data;
      this.SugarCRMTeam.regionId = this.SugarCRMTeam.regionId != null ? this.SugarCRMTeam.regionId.toString() : '';
      this.SugarCRMTeam.teamStatusId = this.SugarCRMTeam.isActive == true ? "true" : "false"
    });
  }
}

