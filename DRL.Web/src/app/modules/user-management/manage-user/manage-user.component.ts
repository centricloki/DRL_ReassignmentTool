import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { UserModel } from '../../../Models/UserModel';
import { FormControl, NgForm } from '@angular/forms';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { TeamModel } from 'src/app/Models/TeamModel';
import { UsersService } from '../users.service';
import { AppConstant } from '../../../app.constants';
import { ToasterService } from 'angular2-toaster';
import { Observable, Subject } from 'rxjs';
import { map, startWith, takeUntil } from 'rxjs/operators';
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
  userZones: ZoneModel[] = [];
  selectedZoneId:number;
  allZones: ZoneModel[] = [];
  teamModel = new TeamModel();
  avpRole : RoleModel = new RoleModel();
  bdRole : RoleModel = new RoleModel();
  private unsubscribe$ = new Subject<void>();

  teamSearchControl = new FormControl('');
  defTeamSearchControl = new FormControl('');
  filteredTeamList: Observable<any[]>;
  filteredDefTeamList: Observable<any[]>;


  ngOnDestroy() {
    this._appConstant.userId = undefined;
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
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
      map(value => this.filterTeams(value || ''))
    );
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

  getAVPRole(){
    this._usersService.getRoleByName('AVP').pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.avpRole = data.data as RoleModel;
    });
  }

  getbdRole(){
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
      this.SugarCRMUser.defaultTeamId = !this.SugarCRMUser.defaultTeamId  ? '' : this.SugarCRMUser.defaultTeamId;
      this.myItems = this.SugarCRMUser.teams;
      if(this.SugarCRMUser.roleId == this.avpRole.roleId){
        this.onAVPChange(undefined);
      }
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
    this.SugarCRMUser.defaultTeamId = this.SugarCRMUser.defaultTeamId == '' ? undefined : this.SugarCRMUser.defaultTeamId;
    this.SugarCRMUser.bdid = this.SugarCRMUser.bdid == '' ? '0' : this.SugarCRMUser.bdid;
    this.SugarCRMUser.avpid = this.SugarCRMUser.bdid == '' ? '0' : this.SugarCRMUser.avpid;
    this.SugarCRMUser.territoryId = this.SugarCRMUser.territoryId == '' ? '0' : this.SugarCRMUser.territoryId;

    if (this.myItems.length == 1 && this.myItems[0].teamId == "0") {
      this.myItems.splice(0, 1);
    }
    this.SugarCRMUser.teams = this.myItems;
    this.SugarCRMUser.zones = this.userZones;

    if(this.SugarCRMUser.roleId != this.avpRole.roleId){
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
    if (this.teamModel.teamId && this.teamModel.teamId != '') {
      if (this.myItems.find(x => x.teamId == this.teamModel.teamId)) {
        this._toasterService.pop('error', 'Error', "Team already exist");
      }
      else {
        const team = this.TeamList.find(x => x.teamId == this.teamModel.teamId);

        if (this.SugarCRMUser.roleId == this.bdRole.roleId && team.bdid && team.bdid != 0 && team.bdid.toString() != this.SugarCRMUser.bdid) {
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
        }
        else {
          this.teamModel.createdBy = "0";
          this.teamModel.createdDate = new Date();
          this.teamModel.updatedBy = "0";
          this.teamModel.updateDate = new Date();
          this.teamModel.name = team.name;
          this.myItems.push(
            this.teamModel
          );
        }
      }
      this.teamModel = new TeamModel();
    }
  }
  deleteTeamDetail(i) {
    this._commonLookupData.confirmDialog('Are you sure you want to delete this team?', (result: any) => {
      if (result) {
        this.myItems.splice(i, 1);
      }
      this.teamModel = new TeamModel();
    });
  }

  addUserZone(){
    if(this.selectedZoneId && this.selectedZoneId != 0){
      if (this.userZones.find(x => x.zoneId == this.selectedZoneId)) {
        this._toasterService.pop('error', 'Error', "Zone already exist");
      }
      else{
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

  getAllAVPZones(avpid:number): void {
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

  getAllBDTerritories(bdId:number): void {
    this._usersService.GetAllTerritoriesForBD(bdId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      this.myItems = (data.data || []) as TeamModel[];
      this.teamModel.teamId = '';
    });
  }

  loadUserTerritories(userId:number): void {
    this._usersService.GetAllTerritoriesForUser(userId).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      this.myItems = (data.data || []) as TeamModel[];
      this.teamModel.teamId = '';
    });
  }

  onRoleChange(event: any): void {
    const roleId = this.SugarCRMUser.roleId;
    const userId = Number(this.SugarCRMUser.userId);

    // Clear fields based on role
    this.SugarCRMUser.bdid = roleId === this.bdRole.roleId ? this.SugarCRMUser.bdid : '';
    this.SugarCRMUser.avpid = roleId === this.avpRole.roleId ? this.SugarCRMUser.avpid : '';

    this.myItems = [];
    this.userZones = [];

    // Load territories only if role is neither 18 nor 19
    if (roleId !== this.bdRole.roleId && roleId !== this.avpRole.roleId) {
      if (!isNaN(userId)) {
        this.loadUserTerritories(userId);
      } 
    }
  }

  onAVPChange(event: any): void {
    let avpId = Number(this.SugarCRMUser.avpid);
    if(!isNaN(avpId)){
      this.getAllAVPZones(avpId);
    }
    else{
      this.userZones = [];
    }
  }

  onBDChange(event: any): void {
    let bdId = Number(this.SugarCRMUser.bdid);
    if(!isNaN(bdId)){
      this.getAllBDTerritories(bdId);
    }
    else{
      this.myItems = [];
    }
  }
}
