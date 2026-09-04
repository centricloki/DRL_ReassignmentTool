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

  titleText: string = ''; // Initialized
  SugarCRMTeam = new TeamModel();
  RegionList: Array<any> = []; // Initialized
  FilteredBDList: Array<any> = []; // BD list filtered by selected region, initialized
  TeamStatusList: Array<any> = []; // Initialized
  // Added property to track if region has been selected
  isRegionSelected: boolean = false;
  private unsubscribe$ = new Subject<void>();
  @ViewChild('formTeam') TeamInfoForm!: NgForm; // Using definite assignment assertion


  constructor(private _commonLookupData: CommonService,
    private _router: Router,
    private _teamService: TeamsService,
    public _appConstant: AppConstant,
    private _toasterService: ToasterService, private _usersService: UsersService) { }

  ngOnDestroy() {
    // Fixed: Check for undefined before setting
    if (this._appConstant.teamId) {
      this._appConstant.teamId = '';
    }
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
    // Fixed: Ensure form is not null and properly checked
    if (this.TeamInfoForm && this.TeamInfoForm.dirty && this.TeamInfoForm.touched) {
      return true;
    }
    return false;
  };

  GetAllRegionList() {
    this._commonLookupData.GetAllRegionList().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.RegionList = data.data;
      this.fetchBDsByRegion(this.SugarCRMTeam.regionId);
    });
  }

  // New method to fetch BDs based on selected region using the dedicated endpoint
  // Uses the new endpoint: GET /api/Lookup/GetBDs/{regionId}
  fetchBDsByRegion(regionId: string) {
    // Reset BD list if regionId is invalid
    if (!regionId || regionId === '' || regionId === '0') {
      this.FilteredBDList = [];
      this.isRegionSelected = false;
      return;
    }

    // Fetch BDs for the selected region
    this._commonLookupData.GetBDsByRegionId(regionId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: response => {
          const data = this._commonLookupData.parseData(response);
          this.FilteredBDList = data.data || [];
          this.isRegionSelected = true;
        },
        error: error => {
          // Handle error case
          this.FilteredBDList = [];
          this.isRegionSelected = false;
          this._toasterService.pop('error', 'Error', 'Failed to fetch BD list');
        }
      });
  }

  // Event handler when region selection changes
  onRegionChange() {
    this.fetchBDsByRegion(this.SugarCRMTeam.regionId);
    // Reset BD selection when region changes
    this.SugarCRMTeam.bdid = '';
  }

  cancelTeamClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this._appConstant.teamId = '';
          if (this.TeamInfoForm) {
            this.TeamInfoForm.reset();
          }
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
    // Handle BD ID properly - if region is selected but no BD is chosen, use '0', otherwise preserve existing value or use '0'
    if (this.SugarCRMTeam.regionId && this.SugarCRMTeam.regionId !== '' && this.SugarCRMTeam.regionId !== '0') {
      // Region is selected, so BD should be considered
      if (!this.SugarCRMTeam.bdid || this.SugarCRMTeam.bdid === '') {
        // If no BD is selected from the dropdown, set it to '0'
        this.SugarCRMTeam.bdid = '0';
      }
    } else {
      // Region is not selected, so BD should be '0'
      this.SugarCRMTeam.bdid = '0';
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
    this.SugarCRMTeam.createdBy = this.SugarCRMTeam.updatedBy = "0";
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
      // Changed handling back to bdid
      this.SugarCRMTeam.bdid = this.SugarCRMTeam.bdid != null ? this.SugarCRMTeam.bdid.toString() : '';
      this.SugarCRMTeam.teamStatusId = this.SugarCRMTeam.isActive == true ? "true" : "false"

      // After loading territory details, fetch BDs based on selected region
      if (this.SugarCRMTeam.regionId &&
        this.SugarCRMTeam.regionId !== '' &&
        this.SugarCRMTeam.regionId !== '0') {
        this.fetchBDsByRegion(this.SugarCRMTeam.regionId);
        // Set isRegionSelected to true since we have a region value for existing team
        this.isRegionSelected = true;
      }
    });
  }
}
