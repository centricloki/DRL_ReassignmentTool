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
import { map, startWith, takeUntil, debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { LookupItemModel } from 'src/app/Models/LookupItemModel';
import { ZoneModel } from 'src/app/Models/ZoneModel';
import { RoleModel } from 'src/app/Models/RoleModel';

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
  userZones: ZoneModel[] = [];
  selectedZoneId: number;
  allZones: ZoneModel[] = [];
  teamModel = new TeamModel();
  avpRole: RoleModel = new RoleModel();
  bdRole: RoleModel = new RoleModel();
  private unsubscribe$ = new Subject<void>();

  teamSearchControl = new FormControl('');
  userDefaultTeamId: string = "0";
  defTeamSearchControl = new FormControl('');
  filteredTeamList: Observable<any[]>;
  filteredDefTeamList: Observable<any[]>;
  private pinValidationSub: Subscription | null = null;
  bdRoleId: any = null;
  tmRoleId: any = null;

  ngOnDestroy() {
    this._appConstant.userId = undefined;
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
    if (this.pinValidationSub)
      this.pinValidationSub.unsubscribe();
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
    this.getAllZones();
    this.getAVPRole();
    this.getbdRole();
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
    let sourceList = this.TeamList;
    if (this.SugarCRMUser && this.bdRole && this.SugarCRMUser.roleId == this.bdRole.roleId) {
      sourceList = this.bdTerritoryList;
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

      this.bdRoleId = bd ? bd.recordId : null;
      this.tmRoleId = tm ? tm.recordId : null;
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

  getAVPRole() {
    this._usersService.getRoleByName('AVP').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.avpRole = data.data as RoleModel;
    });
  }

  getbdRole() {
    this._usersService.getRoleByName('BD Manager').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.bdRole = data.data as RoleModel;
    });
  }

  GetUser() {

    this._usersService.GetUser(this._appConstant.userId).pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.SugarCRMUser = data.data;
      this.SugarCRMUser.managerId = (this.SugarCRMUser.managerId != null && this.SugarCRMUser.managerId != '') ? this.SugarCRMUser.managerId.toString() : '0';
      this.SugarCRMUser.pin = (this.SugarCRMUser.pin != null && this.SugarCRMUser.pin != '') ? this.SugarCRMUser.pin.toString() : '0';
      this.SugarCRMUser.roleId = (this.SugarCRMUser.roleId != null && this.SugarCRMUser.roleId != '') ? this.SugarCRMUser.roleId.toString() : '';
      this.SugarCRMUser.bdid = (this.SugarCRMUser.bdid != null && this.SugarCRMUser.bdid != '') ? this.SugarCRMUser.bdid.toString() : '';
      this.SugarCRMUser.avpid = (this.SugarCRMUser.avpid != null && this.SugarCRMUser.avpid != '') ? this.SugarCRMUser.avpid.toString() : '';
      this.SugarCRMUser.defaultTeamId = !this.SugarCRMUser.defaultTeamId ? '' : this.SugarCRMUser.defaultTeamId;
      this.userDefaultTeamId = this.SugarCRMUser.defaultTeamId;
      this.myItems = this.SugarCRMUser.teams;
      
      // ✅ NEW: If editing a BD Manager user with an existing BD, trigger onBDChange to correctly filter dropdowns immediately
      if (this.SugarCRMUser.roleId == this.bdRole.roleId && this.SugarCRMUser.bdid) {
        this.onBDChange(undefined);
      }
      
      if (this.SugarCRMUser.roleId == this.avpRole.roleId) {
        this.onAVPChange(undefined);
      }
      this.triggerEditValidation();
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
    this.SugarCRMUser.avpid = this.SugarCRMUser.bdid == '' ? '0' : this.SugarCRMUser.avpid;
    this.SugarCRMUser.territoryId = this.SugarCRMUser.territoryId == '' ? '0' : this.SugarCRMUser.territoryId;

    if (this.myItems.length == 1 && this.myItems[0].teamId == "0") {
      this.myItems.splice(0, 1);
    }
    this.SugarCRMUser.teams = this.myItems;
    this.SugarCRMUser.zones = this.userZones;

    if (this.SugarCRMUser.roleId != this.avpRole.roleId) {
      if (this.SugarCRMUser.teams.length > 0) {
        this.SugarCRMUser.territoryId = "";
        for (var i = 0; i < this.SugarCRMUser.teams.length; i++) {
          this.SugarCRMUser.territoryId = this.SugarCRMUser.territoryId + this.SugarCRMUser.teams[i].teamId + ",";
        }
      } else {
        this.SugarCRMUser.territoryId = "0";
      }
    }

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
          //this._commonLookupData.confirmDialog('This territory is assigned to another BD manager. Do you want to override?', (result: any) => {
          //if (result) {
          this.teamModel.createdBy = "0";
          this.teamModel.createdDate = new Date();
          this.teamModel.updatedBy = "0";
          this.teamModel.updateDate = new Date();
          this.teamModel.name = team.name;
          this.myItems.push(
            this.teamModel
          );
          //}
          // });
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
        this.myItems.splice(i, 1);
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
      this.TeamList = (data.data || []) as TeamModel[];
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
    const roleId = this.SugarCRMUser.roleId;

    // Clear fields based on role
    this.SugarCRMUser.bdid = roleId === this.bdRole.roleId ? this.SugarCRMUser.bdid : '';
    this.SugarCRMUser.avpid = roleId === this.avpRole.roleId ? this.SugarCRMUser.avpid : '';

    // ALWAYS keep the Default Territory in myItems when switching roles (for any role)
    let defaultTeam: TeamModel = null;
    if (this.SugarCRMUser.defaultTeamId) {
      defaultTeam = this.myItems.find(x => x.teamId === this.SugarCRMUser.defaultTeamId) 
                 || (this.TeamList ? this.TeamList.find(x => x.teamId === this.SugarCRMUser.defaultTeamId) : null);
    }

    this.myItems = defaultTeam ? [defaultTeam] : [];
    this.bdTerritoryList = [];
    this.userZones = [];
    this.defTeamSearchControl.updateValueAndValidity();

    // Load territories based on role
    if (roleId === this.bdRole.roleId) {
      // For BD Manager, if a BD is already selected, apply filtering
      if (this.SugarCRMUser.bdid && this.SugarCRMUser.bdid !== '0' && this.SugarCRMUser.bdid !== '') {
        this.onBDChange(undefined); // Apply BD-based filtering
      } else {
        // If no BD is selected yet, load all territories so user can select a BD
        this.getAllTerritories();
      }
    } else if (roleId === this.avpRole.roleId) {
      // For AVP role, zones will be handled separately
    } else {
      // For all other roles (including TM), just ensure the dropdown has all territories
      this.getAllTerritories();
    }
  }

  onDefaultTeamChange(event: any): void {
    const defaultTeamId = this.SugarCRMUser.defaultTeamId;
    if ((this.userDefaultTeamId != "") && (this.userDefaultTeamId != defaultTeamId)) {
      const previousItem = this.myItems.find(x => x.teamId == this.userDefaultTeamId);
      if (previousItem) {
        const itemIndex = this.myItems.indexOf(previousItem);
        this.myItems.splice(itemIndex, 1);
      }
    }
    if (!this.myItems.find(x => x.teamId == defaultTeamId)) {
      this.teamModel.teamId = defaultTeamId;
      this.userDefaultTeamId = defaultTeamId;
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
    const roleId = this.SugarCRMUser.roleId;
    
    // Only apply BD filtering for BD Managers, not for Territory Managers
    if (roleId == this.bdRole.roleId) { // Only for BD Manager role
      if (!isNaN(bdid) && bdid > 0) {
        // Fetch territories for the selected BD to auto-populate Assign Team and Default Territory dropdowns
        this._usersService.GetAllTerritoriesForBD(bdid).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          const data = this._commonLookupData.parseData(res);
          
          // Populate myItems and bdTerritoryList with the territories of the selected BD
          this.bdTerritoryList = (data.data || []) as TeamModel[];
          this.myItems = [...this.bdTerritoryList];
          
          // Trigger filter updates for the reactive form controls
          this.defTeamSearchControl.updateValueAndValidity();
        });
      } else {
        // If BD is cleared, clear myItems and bdTerritoryList
        this.myItems = [];
        this.bdTerritoryList = [];
        this.defTeamSearchControl.updateValueAndValidity();
      }
    }
    // For Territory Managers, do nothing - they should see all territories regardless of BD selection
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

  // Helper for HTML
  get showBDDropdown(): boolean {
    return this.SugarCRMUser.roleId == this.bdRoleId
      || this.SugarCRMUser.roleId == this.tmRoleId;
  }
  get isBDManager(): boolean {
    return this.SugarCRMUser.roleId == this.bdRoleId;
  }
  get isTMManager(): boolean {
    return this.SugarCRMUser.roleId == this.tmRoleId;
  }
}
