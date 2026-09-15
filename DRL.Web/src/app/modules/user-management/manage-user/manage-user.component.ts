import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { UserModel } from '../../../Models/UserModel';
import { FormControl, NgForm } from '@angular/forms';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { TeamModel } from 'src/app/Models/TeamModel';
import { UsersService } from '../users.service';
import { AppConstant } from '../../../app.constants';
import { ToasterService } from 'angular2-toaster';
import { Observable, Subject, Subscription } from 'rxjs';
import { map, startWith, takeUntil, debounceTime, distinctUntilChanged, filter, finalize } from 'rxjs/operators';
import { LookupItemModel } from 'src/app/Models/LookupItemModel';
import { ZoneModel } from 'src/app/Models/ZoneModel';
import { RoleModel } from 'src/app/Models/RoleModel';
import { RegionModel } from 'src/app/Models/RegionModel';

declare var $: any;

@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrls: ['./manage-user.component.css']
})
export class ManageUserComponent implements OnInit, OnDestroy {

  constructor(private _commonLookupData: CommonService,
    private _router: Router,
    private _usersService: UsersService,
    public _appConstant: AppConstant,
    private _toasterService: ToasterService) { }

  ReportsToList: Array<any>;
  RoleList: Array<any>;
  TeamList: Array<TeamModel>;
  StatusTypeList: Array<any>;
  avpList: Array<any>;
  bdList: Array<any>;
  titleText: string;
  btnText: string;
  SugarCRMUser = new UserModel();
  @ViewChild('formUser') userInfoForm: NgForm;

  myItems: TeamModel[] = [];
  bdTerritoryList: TeamModel[] = [];
  regionTerritoryList: TeamModel[] = [];
  userZones: ZoneModel[] = [];
  selectedZoneId: number;
  allZones: ZoneModel[] = [];
  teamModel = new TeamModel();
  avpRole: RoleModel = new RoleModel();
  bdRole: RoleModel = new RoleModel();
  regionManagerRole: RoleModel = new RoleModel();
  zoneManagerRole: RoleModel = new RoleModel();
  regionList: Array<any>;
  private unsubscribe$ = new Subject<void>();

  teamSearchControl = new FormControl('');
  userDefaultTeamId: string = "0";
  defTeamSearchControl = new FormControl('');
  filteredTeamList: Observable<any[]>;
  filteredDefTeamList: Observable<any[]>;
  private pinValidationSub: Subscription | null = null;
  bdRoleId: any = null;
  tmRoleId: any = null;
  avpRoleId: any = null;
  rmRoleId: any = null;
  zmRoleId: any = null;
  private loaderCount: number = 0;
  private loaderInterval: any = null;
  private loadedRegionId: number | null = null;
  private loadedBDId: number | null = null;
  private loadedZoneId: number | null = null;
  // Zone Manager specific
  zmTerritoryList: TeamModel[] = [];   // territories from selected zone (Default Territory only)
  zmRegionList: Array<any> = [];       // regions belonging to the selected zone
  userRegions: RegionModel[] = [];     // assigned regions (like userZones for AVP)
  selectedRegionIdForZM: string = '';  // selected region in Assign Region panel

  ngOnDestroy() {
    this._appConstant.userId = undefined;
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    if (this.pinValidationSub)
      this.pinValidationSub.unsubscribe();
    this.clearLoaderInterval();
    this.loaderCount = 0;
    if (typeof $ !== 'undefined') {
      $('.ajax-loading').hide();
    }
  }
  ngOnInit() {
    this.titleText = "Create User";
    this.btnText = "Save";

    this.GetAllRoles();
    this.getAllTerritories();
    this.GetAllUsers();
    this.GetAllStatusTypeList();
    this.GetAllAVPs();
    this.GetAllBDs();
    this.GetAllRegions();
    this.getAllZones();
    this.getAVPRole();
    this.getbdRole();
    this.getRegionManagerRole();
    this.getZoneManagerRole();
    if (this._appConstant.userId != '' && this._appConstant.userId != null) {
      this.titleText = "Edit User";
      this.btnText = "Update";
      this.GetUser();
    }

    this.filteredTeamList = this.teamSearchControl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterTeams(value || ''))
    );

    this.filteredDefTeamList = this.defTeamSearchControl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterDefTeams(value || ''))
    );
  }

  triggerEditValidation(): void {
    if (this.pinValidationSub)
      this.pinValidationSub.unsubscribe();

    Promise.resolve().then(() => {
      // Safe access without optional chaining
      if (!this.userInfoForm || !this.userInfoForm.controls) return;

      const pinControl = this.userInfoForm.controls['pin'];
      if (!pinControl) return;

      // ✅ Safe subscription with RxJS operators
      this.pinValidationSub = pinControl.valueChanges.pipe(
        // Wait 300ms after user stops typing
        debounceTime(300),
        // Only emit if value actually changed (ignores same value re-emissions)
        distinctUntilChanged(),
        // Only process when control is enabled AND has a value
        filter(() => !pinControl.disabled && pinControl.value != null && pinControl.value !== '')
      ).subscribe(() => {
        // ⚠️ DO NOT call updateValueAndValidity() here - causes infinite loop!
        // Angular auto-runs validation on value change. Just mark as touched to show UI.
        if (pinControl.invalid) {
          pinControl.markAsTouched(); // Triggers red border + error display
          pinControl.markAsDirty();
        }
      });
    });
  }

  private filterTeams(value: string): any[] {
    // Handle undefined/null TeamList
    if (!this.TeamList || !Array.isArray(this.TeamList)) {
      return [];
    }

    // Handle undefined/null search value
    if (!value) {
      return this.TeamList;
    }

    const filterValue = value.toLowerCase();
    return this.TeamList.filter(team => {
      // Handle undefined/null team or team.name
      if (!team || !team.name) {
        return false;
      }
      return team.name.toLowerCase().includes(filterValue);
    });
  }

  private filterDefTeams(value: string): any[] {
    let sourceList: TeamModel[] = [];
    if (this.isBDManager) {
      sourceList = this.bdTerritoryList || [];
    } else if (this.isRegionManager) {
      sourceList = this.regionTerritoryList || [];
    } else if (this.isZoneManager) {
      // Zone Manager: Default Territory shows territories from selected zone
      sourceList = this.zmTerritoryList || [];
    } else {
      sourceList = this.TeamList || [];
    }

    if (!sourceList || !Array.isArray(sourceList)) {
      return [];
    }

    if (!value) {
      return sourceList;
    }

    const filterValue = value.toLowerCase();
    return sourceList.filter(team => {
      if (!team || !team.name) {
        return false;
      }
      return team.name.toLowerCase().includes(filterValue);
    });
  }


  get isPinActive(): boolean {
    return this.SugarCRMUser.userId != null && this.SugarCRMUser.userId !== '';
  }
  get isPinRequired(): boolean {
    return this.isPinActive;
  }
  get isPinDisabled(): boolean {
    return !this.isPinActive;
  }

  // Allow only numeric keypress (0-9)
  onPinKeypress(event: KeyboardEvent): boolean {
    const charCode = event.which || event.keyCode;
    // Allow: backspace, delete, tab, escape, enter
    if ([8, 9, 13, 27, 46].includes(charCode)) {
      return true;
    }
    // Allow: Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
    if ((event.ctrlKey || event.metaKey) && [65, 67, 86, 88].includes(charCode)) {
      return true;
    }
    // Block non-numeric characters
    if (charCode < 48 || charCode > 57) {
      event.preventDefault();
      return false;
    }
    // Block if already 4 digits
    const input = event.target as HTMLInputElement;
    if (input.value.length >= 4) {
      event.preventDefault();
      return false;
    }
    return true;
  }

  // Sanitize input: remove non-digits, enforce max 4
  onPinInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const digitsOnly = input.value.replace(/\D/g, '').slice(0, 4);
    input.value = digitsOnly;
    this.SugarCRMUser.pin = digitsOnly; // Sync with model
  }

  canDeactivate(): Promise<boolean> | boolean {
    return this.userInfoForm.dirty && this.userInfoForm.touched;
  };
  GetAllStatusTypeList() {
    this.StatusTypeList = [{ "recordId": true, "value": "Active" }, { "recordId": false, "value": "Inactive" }]
  }
  GetAllRoles() {
    this._commonLookupData.GetActiveRoles().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.RoleList = data.data;

      // Find IDs by name - no hardcode
      const bd = this.RoleList.find(r => r.value.toLowerCase().includes('bd manager'));
      const tm = this.RoleList.find(r => r.value.toLowerCase().includes('territory manager'));
      const avp = this.RoleList.find(r => r.value.toLowerCase() === 'avp' || r.value.toLowerCase().includes('avp'));
      const rm = this.RoleList.find(r => r.value && (r.value.toLowerCase().includes('region manager') || r.value.toLowerCase().includes('regional manager')));
      const zm = this.RoleList.find(r => r.value && r.value.toLowerCase().includes('zone manager'));

      this.bdRoleId = bd ? String(bd.recordId) : null;
      this.tmRoleId = tm ? String(tm.recordId) : null;
      this.avpRoleId = avp ? String(avp.recordId) : null;
      this.rmRoleId = rm ? String(rm.recordId) : null;
      this.zmRoleId = zm ? String(zm.recordId) : null;
      this.checkAndTriggerRoleData();
    });
  }
  GetAllUsers() {
    this._commonLookupData.GetAllUsers().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);

      this.ReportsToList = data.data;
    });
  }
  GetAllAVPs() {
    this._commonLookupData.GetAllAVPs().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.avpList = data.data;
    });
  }
  GetAllBDs() {
    this._commonLookupData.GetAllBDs().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.bdList = data.data;
    });
  }
  GetAllRegions() {
    this._commonLookupData.GetAllRegionList().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.regionList = data.data;
    });
  }

  getAVPRole() {
    this._usersService.getRoleByName('AVP').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.avpRole = data.data as RoleModel;
      this.checkAndTriggerRoleData();
    });
  }

  getbdRole() {
    this._usersService.getRoleByName('BD Manager').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.bdRole = data.data as RoleModel;
      this.checkAndTriggerRoleData();
    });
  }

  getRegionManagerRole() {
    this._usersService.getRoleByName('Region Manager').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.regionManagerRole = data.data as RoleModel;
      this.checkAndTriggerRoleData();
    });
  }

  getZoneManagerRole() {
    this._usersService.getRoleByName('Zone Manager').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.zoneManagerRole = data.data as RoleModel;
      this.checkAndTriggerRoleData();
    });
  }

  checkAndTriggerRoleData(): void {
    if (!this.SugarCRMUser || !this.SugarCRMUser.roleId) {
      return;
    }

    if (this.isBDManager && this.SugarCRMUser.bdid && this.SugarCRMUser.bdid !== '0') {
      this.onBDChange(undefined);
    }

    if (this.isRegionManager && this.SugarCRMUser.regionId && this.SugarCRMUser.regionId !== '0') {
      this.onRegionChange(undefined);
    }

    if (this.isAVPManager) {
      this.onAVPChange(undefined);
    }

    // Zone Manager: trigger zone territory load for Default Territory dropdown
    if (this.isZoneManager && this.SugarCRMUser.zoneId && this.SugarCRMUser.zoneId !== '0') {
      this.onZoneChange(undefined);
    }

    this.defTeamSearchControl.updateValueAndValidity();
  }

  GetUser() {
    this.showLoader();
    this._usersService.GetUser(this._appConstant.userId).pipe(
      takeUntil(this.unsubscribe$),
      finalize(() => this.hideLoader())
    ).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.SugarCRMUser = data.data;
      this.SugarCRMUser.managerId = (this.SugarCRMUser.managerId != null && this.SugarCRMUser.managerId != '') ? this.SugarCRMUser.managerId.toString() : '0';
      this.SugarCRMUser.pin = (this.SugarCRMUser.pin != null && this.SugarCRMUser.pin != '') ? this.SugarCRMUser.pin.toString() : '0';
      this.SugarCRMUser.roleId = (this.SugarCRMUser.roleId != null && this.SugarCRMUser.roleId != '') ? this.SugarCRMUser.roleId.toString() : '';
      this.SugarCRMUser.bdid = (this.SugarCRMUser.bdid != null && this.SugarCRMUser.bdid != '') ? this.SugarCRMUser.bdid.toString() : '';
      this.SugarCRMUser.avpid = (this.SugarCRMUser.avpid != null && this.SugarCRMUser.avpid != '') ? this.SugarCRMUser.avpid.toString() : '';
      this.SugarCRMUser.regionId = (this.SugarCRMUser.regionId != null && this.SugarCRMUser.regionId != '') ? this.SugarCRMUser.regionId.toString() : '';
      this.SugarCRMUser.zoneId = (this.SugarCRMUser.zoneId != null && this.SugarCRMUser.zoneId !== '') ? this.SugarCRMUser.zoneId.toString() : '';
      this.SugarCRMUser.defaultTeamId = (this.SugarCRMUser.defaultTeamId != null && this.SugarCRMUser.defaultTeamId !== '') ? this.SugarCRMUser.defaultTeamId.toString() : '';
      this.userDefaultTeamId = this.SugarCRMUser.defaultTeamId;

      if (this.SugarCRMUser.teams && Array.isArray(this.SugarCRMUser.teams)) {
        this.SugarCRMUser.teams.forEach(t => {
          if (t && t.teamId != null) {
            t.teamId = t.teamId.toString();
          }
        });
      }
      this.myItems = this.SugarCRMUser.teams || [];

      // Zone Manager: restore assigned regions from API
      if (this.SugarCRMUser.regions && Array.isArray(this.SugarCRMUser.regions)) {
        this.userRegions = this.SugarCRMUser.regions;
      } else {
        this.userRegions = [];
      }

      this.checkAndTriggerRoleData();
      this.defTeamSearchControl.updateValueAndValidity();
      this.triggerEditValidation();
    }, (error: any) => {
      this._toasterService.pop('error', 'Error', error.message || 'Failed to load user');
    });

    this.teamModel = new TeamModel();
  }

  cancelUserClick() {
    if (this.canDeactivate()) {
      this._commonLookupData.customConfirm('Are you sure you want to continue? Any unsaved changes will be lost.', (result: any) => {
        if (result) {
          this._appConstant.userId = '';
          this.userInfoForm.reset();
          this._router.navigate(['/users']);
        }
      });
    } else {
      this._appConstant.userId = '';
      this._router.navigate(['/users']);
    }

  }
  saveUser() {
    if (this.myItems.length == 0) {
      this.teamModel = new TeamModel();
      this.teamModel.teamId = '0';
      this.teamModel.createdBy = "0";
      this.myItems.push(this.teamModel);
    }
    if (this._appConstant.userId != '' && this._appConstant.userId != null) {
      if (this.SugarCRMUser.managerId == this._appConstant.userId) {
        this._toasterService.pop('error', 'Error', "Selected Reports to is invalid as user can't be his/her own manager. Please select another.");
        return false;
      }
      if (this.SugarCRMUser.isActive == false) {
        this._usersService.GetAllUsersByManagerId(this._appConstant.userId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          var roleData = this._commonLookupData.parseData(res);
          if (roleData.data.length > 0) {
            this._toasterService.pop('error', 'Error', "User can not be deactivated as it is assigned to one of the user");
            return false;
          }
          else {
            this.manageUser();
          }
        });
      }
      else {
        this.manageUser();
      }
    }
    else {
      this.manageUser();
    }

  }

  manageUser() {
    //this.SugarCRMUser.createdBy = localStorage["userName"];
    this.SugarCRMUser.createdDate = new Date();
    //this.SugarCRMUser.updatedBy = localStorage["userName"];
    this.SugarCRMUser.updatedDate = new Date();
    this.SugarCRMUser.userId = this._appConstant.userId;
    this.SugarCRMUser.teamID = this.teamModel.teamId;
    this.SugarCRMUser.managerId = this.SugarCRMUser.managerId == '' ? '0' : this.SugarCRMUser.managerId;
    this.SugarCRMUser.roleId = this.SugarCRMUser.roleId == '' ? '0' : this.SugarCRMUser.roleId;
    this.SugarCRMUser.defaultTeamId = this.SugarCRMUser.defaultTeamId == '' ? '' : this.SugarCRMUser.defaultTeamId;
    this.SugarCRMUser.bdid = this.SugarCRMUser.bdid == '' ? '0' : this.SugarCRMUser.bdid;
    this.SugarCRMUser.avpid = (this.SugarCRMUser.avpid == '' || this.SugarCRMUser.avpid == null) ? '0' : this.SugarCRMUser.avpid;
    this.SugarCRMUser.regionId = this.SugarCRMUser.regionId == '' ? '0' : this.SugarCRMUser.regionId;
    this.SugarCRMUser.zoneId = (this.SugarCRMUser.zoneId == '' || this.SugarCRMUser.zoneId == null) ? '0' : this.SugarCRMUser.zoneId;
    this.SugarCRMUser.territoryId = this.SugarCRMUser.territoryId == '' ? '0' : this.SugarCRMUser.territoryId;

    if (this.myItems.length == 1 && this.myItems[0].teamId == "0") {
      this.myItems.splice(0, 1);
    }

    // Territory assignment: only needed for TM, BD, RM. For ZM and AVP it's not needed (ZM uses regions, AVP uses zones)
    if (this.isZoneManager || this.isAVPManager || (this.avpRole && this.SugarCRMUser.roleId == this.avpRole.roleId)) {
      this.SugarCRMUser.teams = [];
      this.SugarCRMUser.territoryId = "0";
    } else {
      this.SugarCRMUser.teams = this.myItems;
      if (this.SugarCRMUser.teams.length > 0) {
        this.SugarCRMUser.territoryId = "";
        for (var i = 0; i < this.SugarCRMUser.teams.length; i++) {
          this.SugarCRMUser.territoryId = this.SugarCRMUser.territoryId + this.SugarCRMUser.teams[i].teamId + ",";
        }
      } else {
        this.SugarCRMUser.territoryId = "0";
      }
    }
    this.SugarCRMUser.zones = this.userZones;
    // Zone Manager: pass assigned regions (maps to @RegionIds in SP)
    this.SugarCRMUser.regions = this.isZoneManager ? this.userRegions : [];

    if (this._appConstant.userId == "0" || this._appConstant.userId == null || this._appConstant.userId == '') {
      this.SugarCRMUser.pin = Math.floor(1000 + Math.random() * 9000).toString();
      this.SugarCRMUser.createdBy = "1";
      this.SugarCRMUser.updatedBy = "1";
      this.SugarCRMUser.createdDate = new Date();
    }
    else {
      this.SugarCRMUser.updatedBy = "1";
      this.SugarCRMUser.updatedDate = new Date();
    }
    this._usersService.ManageUser(this.SugarCRMUser).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data != null && data != "" && data.isSuccess) {
        this._appConstant.userId = '';
        this._toasterService.pop('success', 'Success', data.message);
        this._router.navigate(['/users']);
      }
      else {
        this._toasterService.pop('error', 'error', data.message);
      }
    }
      , (error: any) => {
        this._toasterService.pop('error', 'Error', error.message);
      });
  }
  addUserToTeam() {
    // Check if the user's role is 'territory manager' and they already have one territory
    if (this.SugarCRMUser.roleId == this.tmRoleId && this.myItems.length >= 1) {
      this._toasterService.pop('error', 'Error', "A Territory Manager can only be assigned to one territory.");
      return; // Exit the function early if the condition is met
    }

    if (this.teamModel.teamId && this.teamModel.teamId != '') {
      if (this.myItems.find(x => x.teamId == this.teamModel.teamId)) {
        this._toasterService.pop('error', 'Error', "Team already exist");
      }
      else {
        const team = this.TeamList.find(x => x.teamId == this.teamModel.teamId);

        if (team && this.SugarCRMUser.roleId == this.bdRole.roleId && team.bdid && Number(team.bdid) != 0 && String(team.bdid) !== this.SugarCRMUser.bdid) {
          this.teamModel.createdBy = "0";
          this.teamModel.createdDate = new Date();
          this.teamModel.updatedBy = "0";
          this.teamModel.updateDate = new Date();
          this.teamModel.name = team.name;
          this.myItems.push(
            this.teamModel
          );
        } else if (team) {
          this.teamModel.createdBy = "0";
          this.teamModel.createdDate = new Date();
          this.teamModel.updatedBy = "0";
          this.teamModel.updateDate = new Date();
          this.teamModel.name = team.name;
          if (this.SugarCRMUser.roleId == this.bdRole.roleId) {
            this.teamModel.bdid = this.SugarCRMUser.bdid;
          }
          this.myItems.push(
            this.teamModel
          );
        }
        this.teamModel = new TeamModel();
      }
    }
    else {
      this._toasterService.pop('error', 'Error', "Please select team");
    }

    // Update default team dropdown
    this.defTeamSearchControl.updateValueAndValidity();
  }
  deleteTeamDetail(i) {
    this._commonLookupData.confirmDialog('Are you sure you want to delete this team?', (result: any) => {
      if (result) {
        const deleted = this.myItems[i];
        this.myItems.splice(i, 1);
        if (deleted && String(deleted.teamId) === String(this.SugarCRMUser.defaultTeamId)) {
          this.SugarCRMUser.defaultTeamId = '';
          this.userDefaultTeamId = '';
        }
        this.defTeamSearchControl.updateValueAndValidity();
      }
      this.teamModel = new TeamModel();
    });
  }

  addUserZone() {
    if (this.selectedZoneId && this.selectedZoneId != 0) {
      if (this.userZones.find(x => x.zoneId == this.selectedZoneId)) {
        this._toasterService.pop('error', 'Error', "Zone already exist");
      }
      else {
        let selectedZone = this.allZones.find(x => x.zoneId == this.selectedZoneId);
        if (selectedZone.avpid && selectedZone.avpid != 0 && selectedZone.avpid.toString() != this.SugarCRMUser.avpid) {
          this._commonLookupData.confirmDialog('This zone is assigned to another AVP. Do you want to override?', (result: any) => {
            if (result) {
              this.userZones.push(selectedZone);
            }
          });
        }
        else {
          this.userZones.push(selectedZone);
        }
      }
      this.selectedZoneId = undefined;
    }
  }

  deleteUserZone(index: number) {
    this._commonLookupData.confirmDialog('Are you sure you want to delete this zone?', (result: any) => {
      if (result) {
        this.userZones.splice(index, 1);
      }
    });
  }

  getAllZones(): void {
    this._usersService.GetAllZones().pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      this.allZones = (data.data || []) as ZoneModel[];
    });
  }

  getAllAVPZones(avpid: number): void {
    this._usersService.GetAllZonesForAVP(avpid).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      this.userZones = (data.data || []) as ZoneModel[];
    });
  }

  getAllTerritories(): void {
    this._usersService.GetAllTerritories().pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      const rawList = (data.data || []) as TeamModel[];
      this.TeamList = rawList.map(t => ({
        ...t,
        teamId: t && t.teamId != null ? t.teamId.toString() : ''
      }));
      this.teamSearchControl.updateValueAndValidity();
      this.defTeamSearchControl.updateValueAndValidity();
    });
  }

  loadUserTerritories(userId: number): void {
    this._usersService.GetAllTerritoriesForUser(userId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      this.myItems = (data.data || []) as TeamModel[];
      this.teamModel.teamId = '';
    });
  }

  onRoleChange(event: any): void {
    // Clear fields based on role
    this.SugarCRMUser.bdid = this.isBDManager ? this.SugarCRMUser.bdid : '';
    this.SugarCRMUser.avpid = this.isAVPManager ? this.SugarCRMUser.avpid : '';
    this.SugarCRMUser.regionId = this.isRegionManager ? this.SugarCRMUser.regionId : '';
    this.SugarCRMUser.zoneId = this.isZoneManager ? this.SugarCRMUser.zoneId : '';

    this.myItems = [];
    this.bdTerritoryList = [];
    this.regionTerritoryList = [];
    this.userZones = [];
    this.userRegions = [];
    this.zmTerritoryList = [];
    this.zmRegionList = [];
    this.selectedRegionIdForZM = '';
    this.loadedRegionId = null;
    this.loadedBDId = null;
    this.loadedZoneId = null;
    this.SugarCRMUser.defaultTeamId = '';
    this.userDefaultTeamId = '';
    this.defTeamSearchControl.updateValueAndValidity();

    // Load territories based on role
    if (this.isBDManager) {
      // For BD Manager, if a BD is already selected, apply filtering
      if (this.SugarCRMUser.bdid && this.SugarCRMUser.bdid !== '0' && this.SugarCRMUser.bdid !== '') {
        this.onBDChange(undefined); // Apply BD-based filtering
      } else {
        this.bdTerritoryList = [];
        this.defTeamSearchControl.updateValueAndValidity();
      }
    } else if (this.isRegionManager) {
      // For Region Manager, if a Region is already selected, apply filtering
      if (this.SugarCRMUser.regionId && this.SugarCRMUser.regionId !== '0' && this.SugarCRMUser.regionId !== '') {
        this.onRegionChange(undefined);
      } else {
        this.regionTerritoryList = [];
        this.defTeamSearchControl.updateValueAndValidity();
      }
    } else if (this.isAVPManager) {
      // For AVP role, zones will be handled separately
    } else if (this.isZoneManager) {
      // For Zone Manager: zone drives Default Territory and Assign Region list
      this.zmTerritoryList = [];
      this.zmRegionList = [];
      this.userRegions = [];
      this.selectedRegionIdForZM = '';
      if (this.SugarCRMUser.zoneId && this.SugarCRMUser.zoneId !== '0' && this.SugarCRMUser.zoneId !== '') {
        this.onZoneChange(undefined);
      } else {
        this.defTeamSearchControl.updateValueAndValidity();
      }
    } else {
      // For all other roles (including TM), just ensure the dropdown has all territories
      if (!this.TeamList || this.TeamList.length === 0) {
        this.getAllTerritories();
      }
    }
  }

  onDefaultTeamChange(event: any): void {
    const defaultTeamId = this.SugarCRMUser.defaultTeamId;
    if (!this.isRegionManager && !this.isBDManager && !this.isZoneManager) {
      if ((this.userDefaultTeamId != "") && (this.userDefaultTeamId != defaultTeamId)) {
        const previousItem = this.myItems.find(x => String(x.teamId) === String(this.userDefaultTeamId));
        if (previousItem) {
          const itemIndex = this.myItems.indexOf(previousItem);
          this.myItems.splice(itemIndex, 1);
        }
      }
    }
    this.userDefaultTeamId = defaultTeamId;
    if (defaultTeamId && !this.myItems.some(x => String(x.teamId) === String(defaultTeamId))) {
      this.teamModel.teamId = defaultTeamId;
      this.addUserToTeam();
    }
  }

  onAVPChange(event: any): void {
    let avpId = Number(this.SugarCRMUser.avpid);
    if (!isNaN(avpId)) {
      this.getAllAVPZones(avpId);
    }
    else {
      this.userZones = [];
    }
  }

  onBDChange(event: any): void {
    const bdid = Number(this.SugarCRMUser.bdid);

    // Only apply BD filtering for BD Managers, not for Territory Managers
    if (this.isBDManager) { // Only for BD Manager role
      if (!isNaN(bdid) && bdid > 0) {
        if (event === undefined && this.loadedBDId === bdid && this.bdTerritoryList && this.bdTerritoryList.length > 0) {
          return;
        }
        this.loadedBDId = bdid;
        this.showLoader();
        // Fetch territories for the selected BD to auto-populate Assign Team and Default Territory dropdowns
        this._usersService.GetAllTerritoriesForBD(bdid).pipe(
          takeUntil(this.unsubscribe$),
          finalize(() => this.hideLoader())
        ).subscribe(res => {
          const data = this._commonLookupData.parseData(res);

          const rawBDList = (data.data || []) as TeamModel[];
          const bdTeams = rawBDList.map(t => ({
            ...t,
            teamId: t && t.teamId != null ? t.teamId.toString() : ''
          }));

          // Default Territory dropdown uses this list exclusively
          this.bdTerritoryList = bdTeams;

          if (event === undefined) {
            // Initial load: bring all territories of the BD into myItems, plus any existing saved teams
            const teamMap = new Map<string, TeamModel>();
            bdTeams.forEach(t => teamMap.set(String(t.teamId), { ...t }));
            if (this.myItems && this.myItems.length > 0) {
              this.myItems.forEach(t => {
                if (t && t.teamId && !teamMap.has(String(t.teamId))) {
                  teamMap.set(String(t.teamId), { ...t });
                }
              });
            }
            this.myItems = Array.from(teamMap.values());
          } else {
            // User explicitly changed the BD dropdown:
            // Reset myItems strictly to the new BD's territories!
            this.myItems = bdTeams.map(t => ({ ...t }));
            // Also reset defaultTeamId if it does not belong to the new BD
            if (this.SugarCRMUser.defaultTeamId && !bdTeams.some(t => String(t.teamId) === String(this.SugarCRMUser.defaultTeamId))) {
              this.SugarCRMUser.defaultTeamId = '';
              this.userDefaultTeamId = '';
            }
          }

          // Trigger filter updates for the reactive form controls
          this.defTeamSearchControl.updateValueAndValidity();
        }, (error: any) => {
          this._toasterService.pop('error', 'Error', error.message || 'Failed to load territories for BD');
        });
      } else {
        this.loadedBDId = null;
        // If BD is cleared, clear myItems, bdTerritoryList, and defaultTeamId
        this.myItems = [];
        this.bdTerritoryList = [];
        this.SugarCRMUser.defaultTeamId = '';
        this.userDefaultTeamId = '';
        this.defTeamSearchControl.updateValueAndValidity();
      }
    }
    // For Territory Managers, do nothing - they should see all territories regardless of BD selection
  }

  onRegionChange(event: any): void {
    const regionId = Number(this.SugarCRMUser.regionId);

    if (this.isRegionManager) {
      if (!isNaN(regionId) && regionId > 0) {
        if (event === undefined && this.loadedRegionId === regionId && this.regionTerritoryList && this.regionTerritoryList.length > 0) {
          return;
        }
        this.loadedRegionId = regionId;
        this.showLoader();
        // Fetch territories for the selected Region
        this._usersService.GetAllTerritoriesForRegion(regionId).pipe(
          takeUntil(this.unsubscribe$),
          finalize(() => this.hideLoader())
        ).subscribe(res => {
          const data = this._commonLookupData.parseData(res);

          const rawList = (data.data || []) as TeamModel[];
          const regionTeams = rawList.map(t => ({
            ...t,
            teamId: t && t.teamId != null ? t.teamId.toString() : ''
          }));

          // Default Territory dropdown uses this list exclusively
          this.regionTerritoryList = regionTeams;

          if (event === undefined) {
            // Initial load: bring all territories of the region into myItems, plus any existing saved teams
            const teamMap = new Map<string, TeamModel>();
            regionTeams.forEach(t => teamMap.set(String(t.teamId), { ...t }));
            if (this.myItems && this.myItems.length > 0) {
              this.myItems.forEach(t => {
                if (t && t.teamId && !teamMap.has(String(t.teamId))) {
                  teamMap.set(String(t.teamId), { ...t });
                }
              });
            }
            this.myItems = Array.from(teamMap.values());
          } else {
            // User explicitly changed the Region dropdown:
            // Reset myItems strictly to the new region's territories!
            this.myItems = regionTeams.map(t => ({ ...t }));
            // Also reset defaultTeamId if it does not belong to the new region
            if (this.SugarCRMUser.defaultTeamId && !regionTeams.some(t => String(t.teamId) === String(this.SugarCRMUser.defaultTeamId))) {
              this.SugarCRMUser.defaultTeamId = '';
              this.userDefaultTeamId = '';
            }
          }

          this.defTeamSearchControl.updateValueAndValidity();
        }, (error: any) => {
          this._toasterService.pop('error', 'Error', error.message || 'Failed to load territories for region');
        });
      } else {
        this.loadedRegionId = null;
        // If Region is cleared, clear myItems, regionTerritoryList, and defaultTeamId
        this.myItems = [];
        this.regionTerritoryList = [];
        this.SugarCRMUser.defaultTeamId = '';
        this.userDefaultTeamId = '';
        this.defTeamSearchControl.updateValueAndValidity();
      }
    }
  }

  // Update territory data
  UpdateTerritory() {
    this.SugarCRMUser.regionId = this.SugarCRMUser.regionId == '' ? '0' : this.SugarCRMUser.regionId;
    this.SugarCRMUser.zoneId = this.SugarCRMUser.zoneId == '' ? '0' : this.SugarCRMUser.zoneId;
    this.SugarCRMUser.avpid = this.SugarCRMUser.zoneId == '' ? '0' : this.SugarCRMUser.avpid;
    this.SugarCRMUser.bdid = this.SugarCRMUser.bdid == '' ? '0' : this.SugarCRMUser.bdid;
    this.SugarCRMUser.avpid = this.SugarCRMUser.bdid == '' ? '0' : this.SugarCRMUser.avpid;
    this._usersService.UpdateUserTerritory(this.SugarCRMUser.userId, this.SugarCRMUser.territoryId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data != null && data != "" && data.isSuccess) {
        this._toasterService.pop('success', 'Success', data.message);
      }
      else {
        this._toasterService.pop('error', 'Error', data.message);
      }
    }, (error: any) => {
      this._toasterService.pop('error', 'Error', error.message);
    });
  }

  // Helper for HTML and Component
  get showBDDropdown(): boolean {
    return this.isBDManager;
  }
  get showRegionDropdown(): boolean {
    return this.isRegionManager;
  }
  get isBDManager(): boolean {
    if (!this.SugarCRMUser || !this.SugarCRMUser.roleId) return false;
    const currentRoleId = String(this.SugarCRMUser.roleId);
    return (this.bdRoleId != null && currentRoleId === String(this.bdRoleId))
      || (this.bdRole && this.bdRole.roleId != null && this.bdRole.roleId !== '' && currentRoleId === String(this.bdRole.roleId));
  }
  get isTMManager(): boolean {
    if (!this.SugarCRMUser || !this.SugarCRMUser.roleId) return false;
    const currentRoleId = String(this.SugarCRMUser.roleId);
    return (this.tmRoleId != null && currentRoleId === String(this.tmRoleId));
  }
  get isAVPManager(): boolean {
    if (!this.SugarCRMUser || !this.SugarCRMUser.roleId) return false;
    const currentRoleId = String(this.SugarCRMUser.roleId);
    return (this.avpRoleId != null && currentRoleId === String(this.avpRoleId))
      || (this.avpRole && this.avpRole.roleId != null && this.avpRole.roleId !== '' && currentRoleId === String(this.avpRole.roleId));
  }
  get isRegionManager(): boolean {
    if (!this.SugarCRMUser || !this.SugarCRMUser.roleId) return false;
    const currentRoleId = String(this.SugarCRMUser.roleId);
    return (this.rmRoleId != null && currentRoleId === String(this.rmRoleId))
      || (this.regionManagerRole && this.regionManagerRole.roleId != null && this.regionManagerRole.roleId !== '' && currentRoleId === String(this.regionManagerRole.roleId));
  }
  get isZoneManager(): boolean {
    if (!this.SugarCRMUser || !this.SugarCRMUser.roleId) return false;
    const currentRoleId = String(this.SugarCRMUser.roleId);
    return (this.zmRoleId != null && currentRoleId === String(this.zmRoleId))
      || (this.zoneManagerRole && this.zoneManagerRole.roleId != null && this.zoneManagerRole.roleId !== '' && currentRoleId === String(this.zoneManagerRole.roleId));
  }
  get showZoneDropdown(): boolean {
    return this.isZoneManager;
  }

  // Zone Manager: fetches territories (for Default Territory) and regions (for Assign Region) for selected zone
  onZoneChange(event: any): void {
    const zoneId = Number(this.SugarCRMUser.zoneId);
    if (this.isZoneManager) {
      if (!isNaN(zoneId) && zoneId > 0) {
        // 1. Fetch regions belonging to the selected Zone
        this._commonLookupData.GetRegionsByZoneId(zoneId).pipe(
          takeUntil(this.unsubscribe$)
        ).subscribe(res => {
          const data = this._commonLookupData.parseData(res);
          this.zmRegionList = data.data || [];

          // Pre-populate userRegions with all regions from the selected Zone
          if (event === undefined) {
            // On load: if userRegions is empty, auto-populate all regions of the zone
            if (!this.userRegions || this.userRegions.length === 0) {
              this.userRegions = this.zmRegionList.map(r => {
                const reg = new RegionModel();
                reg.regionId = String(r.recordId);
                reg.regioname = r.value;
                return reg;
              });
            } else {
              // Ensure region names are populated for existing userRegions
              const regionMap = new Map<string, string>();
              this.zmRegionList.forEach(r => regionMap.set(String(r.recordId), r.value));
              this.userRegions.forEach(ur => {
                if (!ur.regioname && regionMap.has(String(ur.regionId))) {
                  ur.regioname = regionMap.get(String(ur.regionId));
                }
              });
            }
          } else {
            // When user explicitly changes Zone: auto-populate all regions of the new Zone
            this.userRegions = this.zmRegionList.map(r => {
              const reg = new RegionModel();
              reg.regionId = String(r.recordId);
              reg.regioname = r.value;
              return reg;
            });
          }
        }, (error: any) => {
          // Fallback: filter from loaded regionList by code (ZoneId)
          if (this.regionList && this.regionList.length > 0) {
            this.zmRegionList = this.regionList.filter(r => String(r.code) === String(zoneId));
            if (!this.userRegions || this.userRegions.length === 0 || event !== undefined) {
              this.userRegions = this.zmRegionList.map(r => {
                const reg = new RegionModel();
                reg.regionId = String(r.recordId);
                reg.regioname = r.value;
                return reg;
              });
            }
          } else {
            this.zmRegionList = [];
          }
        });

        // 2. Fetch territories for Default Territory dropdown
        if (event === undefined && this.loadedZoneId === zoneId && this.zmTerritoryList && this.zmTerritoryList.length > 0) {
          return; // already loaded territories, skip re-fetch
        }
        this.loadedZoneId = zoneId;
        this.showLoader();
        this._usersService.GetAllTerritoriesForZone(zoneId).pipe(
          takeUntil(this.unsubscribe$),
          finalize(() => this.hideLoader())
        ).subscribe(res => {
          const data = this._commonLookupData.parseData(res);
          const rawList = (data.data || []) as TeamModel[];
          this.zmTerritoryList = rawList.map(t => ({
            ...t,
            teamId: t && t.teamId != null ? t.teamId.toString() : ''
          }));
          // If current defaultTeamId is no longer in the new zone's territory list, clear it
          if (event !== undefined && this.SugarCRMUser.defaultTeamId &&
              !this.zmTerritoryList.some(t => String(t.teamId) === String(this.SugarCRMUser.defaultTeamId))) {
            this.SugarCRMUser.defaultTeamId = '';
            this.userDefaultTeamId = '';
          }
          this.defTeamSearchControl.updateValueAndValidity();
        }, (error: any) => {
          this._toasterService.pop('error', 'Error', error.message || 'Failed to load territories for zone');
        });
      } else {
        this.loadedZoneId = null;
        this.zmTerritoryList = [];
        this.zmRegionList = [];
        this.selectedRegionIdForZM = '';
        this.SugarCRMUser.defaultTeamId = '';
        this.userDefaultTeamId = '';
        this.defTeamSearchControl.updateValueAndValidity();
      }
    }
  }

  // Zone Manager: add a region to the assigned regions list
  addUserRegion(): void {
    if (this.selectedRegionIdForZM && this.selectedRegionIdForZM !== '' && this.selectedRegionIdForZM !== '0') {
      if (this.userRegions.find(r => String(r.regionId) === String(this.selectedRegionIdForZM))) {
        this._toasterService.pop('error', 'Error', 'Region already exists');
      } else {
        const sourceList = (this.zmRegionList && this.zmRegionList.length > 0) ? this.zmRegionList : (this.regionList || []);
        const selectedRegion = sourceList.find(r => String(r.recordId) === String(this.selectedRegionIdForZM));
        if (selectedRegion) {
          const newRegion = new RegionModel();
          newRegion.regionId = String(selectedRegion.recordId);
          newRegion.regioname = selectedRegion.value;
          this.userRegions.push(newRegion);
        }
      }
      this.selectedRegionIdForZM = '';
    } else {
      this._toasterService.pop('error', 'Error', 'Please select a region');
    }
  }

  // Zone Manager: remove a region from the assigned regions list
  deleteUserRegion(index: number): void {
    this._commonLookupData.confirmDialog('Are you sure you want to delete this region?', (result: any) => {
      if (result) {
        this.userRegions.splice(index, 1);
      }
    });
  }

  compareTeams(t1: any, t2: any): boolean {
    if (t1 == null || t2 == null) {
      return t1 === t2;
    }
    return String(t1) === String(t2);
  }

  private showLoader(): void {
    this.loaderCount++;
    if (typeof $ !== 'undefined') {
      $('.ajax-loading').show();
      if (!this.loaderInterval) {
        this.loaderInterval = setInterval(() => {
          if (this.loaderCount > 0 && typeof $ !== 'undefined') {
            $('.ajax-loading').show();
          } else {
            this.clearLoaderInterval();
          }
        }, 50);
      }
    }
  }

  private hideLoader(): void {
    this.loaderCount = Math.max(0, this.loaderCount - 1);
    if (this.loaderCount === 0) {
      this.clearLoaderInterval();
      if (typeof $ !== 'undefined') {
        $('.ajax-loading').hide();
      }
    }
  }

  private clearLoaderInterval(): void {
    if (this.loaderInterval) {
      clearInterval(this.loaderInterval);
      this.loaderInterval = null;
    }
  }
}
