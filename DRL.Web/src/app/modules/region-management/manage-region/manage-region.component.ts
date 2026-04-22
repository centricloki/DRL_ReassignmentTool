import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { AppConstant } from 'src/app/app.constants';
import { RegionService } from '../region.service';
import { ToasterService } from 'angular2-toaster';
import { RegionModel } from 'src/app/Models/RegionModel';
import { TeamsService } from '../../team-management/teams.service';
import { TeamModel } from 'src/app/Models/TeamModel';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manage-region',
  templateUrl: './manage-region.component.html',
  styleUrls: ['./manage-region.component.css']
})
export class ManageRegionComponent implements OnInit, OnDestroy {

  constructor(public _commonLookupData: CommonService, private _router: Router, public _appConstant: AppConstant, private _regionService: RegionService
    , private _toasterService: ToasterService, private _teamService: TeamsService) { }
  ReportsToList: Array<any>;
  RoleList: Array<any>;
  titleText: string;
  navTitleText: string;
  ZoneList: Array<any>;
  SugarRegion = new RegionModel();
  TeamList: TeamModel[] = [];
  private unsubscribe$ = new Subject<void>();
  @ViewChild('formRegion') regionInfoForm: NgForm;
  
  ngOnDestroy() {
    this._appConstant.regionId = null;
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  ngOnInit() {
    this.GetAllZoneList();
    this.titleText = "Region Info";
    if (!this._appConstant.regionId || this._appConstant.regionId == '') {
      this.navTitleText = "Create Region";
    }
    else {
      this.navTitleText = "Edit Region";
      this.GetTeamListFromRegionId();
      this.getRegionDetails(this._appConstant.regionId);
    }
  }
  canDeactivate(): Promise<boolean> | boolean {
    return this.regionInfoForm.dirty && this.regionInfoForm.touched;
  };

  GetAllZoneList() {
    this._commonLookupData.GetAllZoneList()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        this.ZoneList = data.data;
      });
  }

  cancelRegionClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this._appConstant.regionId = '';
          this.regionInfoForm.reset();
          this._router.navigate(['/regions']);
        }
      });
    } else {
      this._appConstant.regionId = '';
      this._router.navigate(['/regions']);
    }
  }

  addUpdateRegion(sugarRegionData) {

    //sugarRegionData.createdBy = localStorage["userName"];
    sugarRegionData.createdBy ="0"
    sugarRegionData.createdDate = new Date();
    //sugarRegionData.updatedBy = localStorage["userName"];
    sugarRegionData.updatedBy = "0";
    sugarRegionData.updatedDate = new Date();

    this._regionService.manageRegion(sugarRegionData)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(res => {
        var data = this._commonLookupData.parseData(res);
        if (data != null && data != "" && data.isSuccess) {
          this._appConstant.regionId = '';
          this._toasterService.pop('success', 'Success', data.message);
          this._router.navigate(["/regions"])
        }
        else {
          this._toasterService.pop('error', 'Error', data.message);
        }
      }, (error: any) => {
        this._toasterService.pop('error', 'Error', error.message);
      });
  }
  saveRegion() {
    this.addUpdateRegion(this.SugarRegion);
  }
  getRegionDetails(id) {
    this._regionService.GetRegionDetails(id).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res)
      if (data.isSuccess) {
        this.SugarRegion = data.data;
        this.SugarRegion.zoneId = (this.SugarRegion.zoneId != null && this.SugarRegion.zoneId != '') ? this.SugarRegion.zoneId.toString() : '';
      }
    });
  }

  GetTeamListFromRegionId() {
    this._teamService.GetTeamListFromRegionId(this._appConstant.regionId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res)
      if (data.isSuccess) {
        this.TeamList = data.data;
      }
    });
  }
}
