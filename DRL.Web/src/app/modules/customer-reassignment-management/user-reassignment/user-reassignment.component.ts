import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy, Input } from '@angular/core';
import { CommonService } from '../../../services/common.service';
import { AppConstant } from 'src/app/app.constants';
import { ToasterService } from 'angular2-toaster';
import { NgForm, FormControl } from '@angular/forms';
import { UserReassignmentModel, TeamReassignmentModel, LookUpModel } from 'src/app/Models/UserReassignmentModel';
import { RowArgs, PageChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { CustomersService } from '../customers.service';
import { State, process } from '@progress/kendo-data-query';
import { takeUntil } from 'rxjs/operators';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Subject } from 'rxjs/Subject';
import { MatSelect } from '@angular/material';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-user-reassignment',
  templateUrl: './user-reassignment.component.html',
  styleUrls: ['./user-reassignment.component.css']
})
export class UserReassignmentComponent implements OnInit, OnDestroy {

  constructor(private _commonLookupData: CommonService,
    public _appConstant: AppConstant,
    public _customersService: CustomersService,
    private _toasterService: ToasterService,
    public _router: Router) {

  }
  TeamList: Array<any>;
  UpdateTerritoryList: LookUpModel[] = [];
  TerritoryList: LookUpModel[] = [];
  RoleList: Array<any>;
  SugarCRMUser = new UserReassignmentModel();
  UserList: UserReassignmentModel[] = [];
  rowsSelected: number[] = [];
  userIds: number[] = [];
  allRowsSelected: boolean = false;
  @ViewChild('formUser') userInfoForm: NgForm;
  data: Object[];
  // gridView: GridDataResult;

  id: any[] = [];

  gridView: GridDataResult = {
    data: this.UserList.slice(0, this._appConstant.pageSize),
    total: this.UserList.length
  };
  state: State = {
    skip: 0,
    take: this._appConstant.pageSize
  };

  public UpdateTerritoryCtrl: FormControl = new FormControl();
  public UpdateTerritoryfilterCtrl: FormControl = new FormControl();
  public UpdatefilteredTerritorys: ReplaySubject<LookUpModel[]> = new ReplaySubject<LookUpModel[]>(1);

  public AddTerritoryCtrl: FormControl = new FormControl();
  public AddTerritoryfilterCtrl: FormControl = new FormControl();
  public AddfilteredTerritorys: ReplaySubject<LookUpModel[]> = new ReplaySubject<LookUpModel[]>(1);

  public RemoveTerritoryCtrl: FormControl = new FormControl();
  public RemoveTerritoryfilterCtrl: FormControl = new FormControl();
  public RemovefilteredTerritorys: ReplaySubject<LookUpModel[]> = new ReplaySubject<LookUpModel[]>(1);

  private _onDestroy = new Subject<void>();
  @ViewChild('singleSelect') singleSelect: MatSelect;
  @Input() noDataFoundLabel: string = "no record found";


  ngOnInit() {
    this._appConstant.currentPage = 0;

    $("#btnApply").attr("disabled", true);
    this.getAllTerritoryList();
    this.getUpdateTerritoryList();
    this.getCustomerReassignmentRoles();
    this.getUserList();
    this.getTerritoryFromRoleOnPageLoad();
  }

  getCustomerReassignmentRoles() {
    this._commonLookupData.GetCustomerReassignmentRoles().subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.RoleList = data.data;
    });
  }
  getUpdateTerritoryList() {
    this._commonLookupData.GetCustReassignTeamByRoleIds(null).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data.isSuccess && data.data.length > 0) {
        this.UpdateTerritoryList = data.data;
        this.SetUpdateTerritoryValue();

      }
      else {
        this.UpdateTerritoryList = [];
        this.SetUpdateTerritoryValue();

      }
    });
  }
  getAllTerritoryList() {
    this._commonLookupData.GetAllTerritoryList().subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.TeamList = data.data;
    });
  }

  getTerritoryFromRoleOnPageLoad() {
    this._commonLookupData.GetAllTerritoryList().subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      if (data.isSuccess && data.data.length > 0) {
        this.TerritoryList = data.data;

        this.SetUpdateTerritoryValue();
        this.SetAddTerritoryValue();
        this.SetRemoveTerritoryValue();
      }
      else {
        this.TerritoryList = [];
      }
    });
  }

  getTerritoryFromRole(e) {
    if (e.checked == true) {
      this.id.push(e.source.id);
    }
    else {
      var index = this.id.indexOf(e.source.id, 0);
      this.id.splice(index, 1)
    }
    if (this.id.length > 0) {
      this._commonLookupData.GetCustReassignTerritoriesByRoleIds(this.id).subscribe(res => {
        var data = this._commonLookupData.parseData(res);
        if (data.isSuccess && data.data.length > 0) {
          this.TerritoryList = data.data;

          this.SetAddTerritoryValue();
          this.SetRemoveTerritoryValue();
        }
        else {
          this.TerritoryList = [];
        }
      });

      this._commonLookupData.GetCustReassignTeamByRoleIds(this.id).subscribe(res => {
        var data = this._commonLookupData.parseData(res);
        if (data.isSuccess && data.data.length > 0) {
          this.UpdateTerritoryList = data.data;
          this.SetUpdateTerritoryValue();

        }
        else {
          this.UpdateTerritoryList = [];
          this.SetUpdateTerritoryValue();
        }
      });
    }
    else {
      this._commonLookupData.GetAllTerritoryList().subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        if (data.isSuccess && data.data.length > 0) {
          this.TerritoryList = data.data;
          this.SetUpdateTerritoryValue();
          this.SetAddTerritoryValue();
          this.SetRemoveTerritoryValue();
        }
        else {
          this.TerritoryList = [];
        }
      });
    }
  }

  SetUpdateTerritoryValue() {
    // this.UpdateTerritoryCtrl.setValue(this.TerritoryList[0]);

    this.UpdatefilteredTerritorys.next(this.UpdateTerritoryList.slice());

    this.UpdateTerritoryfilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.UpdatefilterTerritory();
      });
  }
  UpdatefilterTerritory() {
    if (!this.TerritoryList) {
      return;
    }
    let search = this.UpdateTerritoryfilterCtrl.value;
    if (!search) {
      this.UpdatefilteredTerritorys.next(this.UpdateTerritoryList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.UpdatefilteredTerritorys.next(
      this.UpdateTerritoryList.filter(x => x.value.toLowerCase().indexOf(search) > -1)
    );
  }

  SetAddTerritoryValue() {
    // this.AddTerritoryCtrl.setValue(this.TerritoryList[0]);

    this.AddfilteredTerritorys.next(this.TerritoryList.slice());

    this.AddTerritoryfilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.AddfilterTerritory();
      });
  }

  AddfilterTerritory() {
    if (!this.TerritoryList) {
      return;
    }
    let search = this.AddTerritoryfilterCtrl.value;
    if (!search) {
      this.AddfilteredTerritorys.next(this.TerritoryList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.AddfilteredTerritorys.next(
      this.TerritoryList.filter(x => x.value.toLowerCase().indexOf(search) > -1)
    );
  }

  SetRemoveTerritoryValue() {
    //this.RemoveTerritoryCtrl.setValue(this.TerritoryList[0]);

    this.RemovefilteredTerritorys.next(this.TerritoryList.slice());

    this.RemoveTerritoryfilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.RemovefilterTerritory();
      });
  }

  RemovefilterTerritory() {
    if (!this.TerritoryList) {
      return;
    }
    let search = this.RemoveTerritoryfilterCtrl.value;
    if (!search) {
      this.RemovefilteredTerritorys.next(this.TerritoryList.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.RemovefilteredTerritorys.next(
      this.TerritoryList.filter(x => x.value.toLowerCase().indexOf(search) > -1)
    );
  }

  // ngAfterViewInit() {
  //   this.setInitialValue();
  // }

  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  // setInitialValue() {
  //   this.filteredTerriotorys.pipe(take(1), takeUntil(this._onDestroy))
  //     .subscribe(() => {
  //       this.singleSelect.compareWith = (a: LookUpModel, b: LookUpModel) => a && b && a.recordId === b.recordId;
  //     });
  // }

  clearClick() {
    this.SugarCRMUser = new UserReassignmentModel();
    this.getUserList();
  }
  apply() {
    this._appConstant.currentPage = 0;
    this.searchTeam();
  }
  searchTeam() {

    this._customersService.GetReassignmentUser(this.SugarCRMUser.defaultTeamId, this.SugarCRMUser.userName).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.UserList = data.data;
      this.UserList.forEach(element => {
        element.checked = false;
      });
      this.state = {
        skip: this._appConstant.pageSize * this._appConstant.currentPage,
        take: this._appConstant.pageSize
      };
      this.loadItems();
    });
  }
  getUserList() {
    this._customersService.GetReassignmentUser(this.SugarCRMUser.defaultTeamId, this.SugarCRMUser.userName).subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.UserList = data.data;
      this.UserList.forEach(element => {
        element.checked = false;
      });
      this.loadItems();
    });
  }

  loadItems(): void {
    this.gridView = process(this.UserList, this.state);
  }

  rowsSelectedKeys(context: RowArgs): number {
    return context.dataItem.id;
  }
  selectAllRows(e): void {
    if (e.checked) {
      this.allRowsSelected = true;
      this.UserList.forEach(element => {
        element.checked = true;
        this.userIds.push(Number(element.userId));
      });
      this.rowsSelected = this.UserList.map(o => o.userId);
    } else {
      this.allRowsSelected = false;
      this.UserList.forEach(element => {
        element.checked = false;
      });
      this.rowsSelected = [];
      this.userIds = [];
    }
  }
  selectRow(e): void {

    if (e.checked) {
      this.userIds.push(e.source.id);
      this.UserList.find(x => x.userId == e.source.id).checked = true;
    }
    else {
      var index = this.userIds.indexOf(e.source.id, 0);
      this.userIds.splice(index, 1);
      this.UserList.find(x => x.userId == e.source.id).checked = false;

    }
    if (this.UserList.filter(x => x.checked == false).length > 0) {
      this.allRowsSelected = false;
    }
    else {
      this.allRowsSelected = true;
    }
  }
  dataStateChange(state): void {
    this.state = state;
    this.loadItems();
  }

  Apply() {
    if (this.userIds.length == 0) {
      this._toasterService.pop("error", "Error", "Please select atleast one user to apply changes");
      return;
    }
    var updateTerritoryId = this.UpdateTerritoryCtrl.value;
    var AddTeamId = this.AddTerritoryCtrl.value;
    var RemoveTeamId = this.RemoveTerritoryCtrl.value;
    var sessionData = sessionStorage.getItem('CurrentUserData');
    var tokenData = JSON.parse(sessionData);

    var request = {
      "updateTerritoryId": updateTerritoryId,
      "addTeamId": AddTeamId,
      "deleteTeamId": RemoveTeamId,
      "userIds": this.userIds

    }
    this._appConstant.currentPage = (this.state.skip / this._appConstant.pageSize);
    this._customersService.ChangeUserDetails(request).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      if (data.isSuccess) {
        this._toasterService.pop('success', 'Success', data.message);
        this.getUserList();
        this.UpdateTerritoryCtrl.setValue('');
        this.AddTerritoryCtrl.setValue('');
        this.RemoveTerritoryCtrl.setValue('');
        this.userIds = [];
        this.rowsSelected = [];
      }
      else {
        this._toasterService.pop('error', 'Error', data.message);
      }
    }, (error: any) => {
      this._toasterService.pop('error', 'Error', error.message);
    });
  }

  dropDownchange() {
    if ((this.UpdateTerritoryCtrl.value != null && this.UpdateTerritoryCtrl.value != 'undefined' && this.UpdateTerritoryCtrl.value != '')
      || (this.AddTerritoryCtrl.value != null && this.AddTerritoryCtrl.value != 'undefined' && this.AddTerritoryCtrl.value != '')
      || (this.RemoveTerritoryCtrl.value != null && this.RemoveTerritoryCtrl.value != 'undefined' && this.RemoveTerritoryCtrl.value != '')) {
      $('#btnApply').removeAttr("disabled");
    }
    else {
      $("#btnApply").attr("disabled", true);
    }
  }
  searchUser($event) {
    if ($event.target.value != "" && $event.target.value != null) {
      this.SugarCRMUser.defaultTeamId = "";
    }
  }
  searchTerritory() {
    if (this.SugarCRMUser.defaultTeamId != "" && this.SugarCRMUser.defaultTeamId != null) {
      this.SugarCRMUser.userName = "";
    }
  }
}
