import { Component, OnInit, ViewChild, Input, OnDestroy } from '@angular/core';
import { UsersService } from '../../user-management/users.service';
import { CommonService } from 'src/app/services/common.service';
import { TeamModel } from 'src/app/Models/TeamModel';
import { CustomerReassignmentModel } from 'src/app/Models/CustomerReassignmentModel';
import { AppConstant } from 'src/app/app.constants';
import { RowArgs, GridDataResult, DataStateChangeEvent } from '@progress/kendo-angular-grid';
import { ToasterService } from 'angular2-toaster';
import { UserReassignmentModel, TeamReassignmentModel } from 'src/app/Models/UserReassignmentModel';
import { CustomersService } from '../customers.service';
import { CustomerRequestModel } from 'src/app/Models/CustomerRequestModel';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { FormControl } from '@angular/forms';
import { Observable, of, Subject } from 'rxjs';
import { finalize, map, startWith, switchMap, takeUntil, tap } from 'rxjs/operators';
import { State, process } from '@progress/kendo-data-query';
import { Router } from '@angular/router';
import { CustomerimportService } from '../customerimport.service';
declare var $: any;

@Component({
  selector: 'app-customer-reassignment',
  templateUrl: './customer-reassignment.component.html',
  styleUrls: ['./customer-reassignment.component.css']
})
export class CustomerReassignmentComponent implements OnInit, OnDestroy {
  SugarCRMUser = new UserReassignmentModel();
  teamModel = new TeamModel();
  CustomerModel = new CustomerReassignmentModel();
  UsersList: Array<any>;
  StateList: Array<any>;
  AccountTypeList: Array<any>;
  TerritoryList: Array<any>;
  TeamList: Array<any>;
  UpdateTeamList: Array<any>;
  AddTeamList: Array<any>;
  RemoveTeamList: Array<any>;
  CityList: Array<any>;
  CustomerList: any[] = [];
  rowsSelected: string[] = [];
  allRowsSelected: boolean = false;
  customerRequestModel = new CustomerRequestModel();
  teamAssignmentModel = new TeamReassignmentModel();
  data: Object[];
  skip = this._appConstant.skip;
  pageSize = this._appConstant.pageSize;
  RoleList: Array<any>;
  // Track checked state for each role
  checkedRoles: { [key: string]: boolean } = {};
  id: string[] = [];
  customerIds: number[] = [];
  gridView: GridDataResult = {
    data: this.CustomerList.slice(this.skip, this.skip + this.pageSize),
    total: this.CustomerList.length
  };
  state: State = {
    skip: 0,
    take: this.pageSize
  };
  showGrid: boolean = true;
  ExcludeISBKRRoleList: string[] = [];
  isGridFilterable: boolean = true;

  @Input() noDataFoundLabel: string = "no record found";
  @Input() placeholderLabel = 'Find Territory';
  @ViewChild(TooltipDirective) public tooltipDir: TooltipDirective;
  private unsubscribe$ = new Subject<void>();

  public searchTerritoryfilterCtrl: FormControl = new FormControl();
  public filteredSearchTerritoryList: Observable<any[]>;
  public UpdateTerritoryfilterCtrl: FormControl = new FormControl();
  public filteredUpdateTerritoryList: Observable<any[]>;
  public AddTerritoryfilterCtrl: FormControl = new FormControl();
  public filteredAddTerritoryList: Observable<any[]>;
  public RemoveTerritoryfilterCtrl: FormControl = new FormControl();
  public filteredRemoveTerritoryList: Observable<any[]>;
  public searchTeamfilterCtrl: FormControl = new FormControl();
  public filteredSearchTeamList: Observable<any[]>;

  constructor(private _usersService: UsersService, private _commonLookupData: CommonService
    , public _appConstant: AppConstant, private _toasterService: ToasterService
    , private _customersService: CustomersService, private _router: Router
    , private _customerimportService: CustomerimportService) {
  }

  ngOnInit() {
    this._appConstant.currentPage = 0;
    this.GetAllUsers();
    this.GetAllTerritoryList();
    this.getUpdateTerritoryList();
    this.getAddRemoveTeam();
    this.GetAllAccountTypeList();
    this.getCustomerReassignmentRoles();
    this.GetAllStates();
    this.CustomerModel.exactMatch = true;
    this.CustomerModel.partialMatch = false;
    $('a.btnLi').removeClass('active');
    $('#lnkCustomer').addClass('active');

    this.filteredUpdateTerritoryList = this.UpdateTerritoryfilterCtrl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterList(this.UpdateTeamList, value || '', item => item.value))
    );

    this.filteredAddTerritoryList = this.AddTerritoryfilterCtrl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterList(this.AddTeamList, value || '', item => item.value))
    );

    this.filteredRemoveTerritoryList = this.RemoveTerritoryfilterCtrl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterList(this.RemoveTeamList, value || '', item => item.value))
    );

    this.filteredSearchTerritoryList = this.searchTerritoryfilterCtrl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterTerritoryList(value || ''))
    );

    this.filteredSearchTeamList = this.searchTeamfilterCtrl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterList(this.TeamList, value || '', item => item.value))
    );
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }


  private filterList<T>(
    list: T[] | null | undefined,
    searchTerm: string,
    accessor: (item: T) => string | null | undefined
  ): T[] {
    if (!list || !Array.isArray(list)) {
      return [];
    }

    if (!searchTerm) {
      return list;
    }

    const term = searchTerm.toLowerCase();
    return list.filter(item => {
      if (!item) return false;
      const value = accessor(item);
      return value != null && value.toLowerCase().includes(term);
    });
  }


  private filterTerritoryList(value: string): any[] {
    // Handle undefined/null TeamList
    if (!this.TerritoryList || !Array.isArray(this.TerritoryList)) {
      return [];
    }

    // Handle undefined/null search value
    if (!value) {
      return this.TerritoryList;
    }

    const filterValue = value.toLowerCase();
    return this.TerritoryList.filter(team => {
      // Handle undefined/null team or team.name
      if (!team || !team.value) {
        return false;
      }
      return team.value.toLowerCase().includes(filterValue);
    });
  }

  GetAllUsers() {
    this._commonLookupData.GetAllUsers()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        this.UsersList = data.data;
      });
  }
  GetAllCities() {
    if (this.CustomerModel.stateId != '') {
      this._commonLookupData.GetCities(this.CustomerModel.stateId)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(response => {
          var data = this._commonLookupData.parseData(response);
          this.CityList = data.data;
        });
    }
  }
  GetAllStates() {
    this._commonLookupData.GetStates()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        this.StateList = data.data;
      });
  }

  GetAllAccountTypeList() {
    this.AccountTypeList = [{ "recordId": 0, "value": "Select Account Type" }, { "recordId": 1, "value": "Direct" }, { "recordId": 2, "value": "Indirect" }]
  }

  GetAllTerritoryList() {
    this._commonLookupData.GetAllTerritoryList()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        this.TerritoryList = data.data;
        this.TeamList = data.data;
        this.searchTerritoryfilterCtrl.updateValueAndValidity();
        this.searchTeamfilterCtrl.updateValueAndValidity();
      });
  }
  getUpdateTerritoryList() {
    this._commonLookupData.GetAllTerritoryList()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(res => {
        var data = this._commonLookupData.parseData(res);
        if (data.isSuccess) {
          this.UpdateTeamList = data.data;
          this.UpdateTerritoryfilterCtrl.updateValueAndValidity();
        }
      });
  }
  getAddRemoveTeam() {
    this._commonLookupData.GetCustReassignTerritoriesByRoleIds(null)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(res => {
        var data = this._commonLookupData.parseData(res);
        if (data.isSuccess) {
          this.AddTeamList = data.data;
          this.RemoveTeamList = data.data;
          this.AddTerritoryfilterCtrl.updateValueAndValidity();
          this.RemoveTerritoryfilterCtrl.updateValueAndValidity();
        }
      });
  }


  userValChange() {
    if (this.SugarCRMUser.defaultTeamId != null && this.SugarCRMUser.territoryId != 0) {
      this.SugarCRMUser.territoryId = 0;
    }
  }
  territoryValChange() {
    if (this.SugarCRMUser.userId != null && this.SugarCRMUser.userId != 0) {
      this.SugarCRMUser.userId = 0;
    }
  }
  exactValChange(type) {
    if (type == 'exact') {
      this.CustomerModel.exactMatch = true;
      this.CustomerModel.partialMatch = false;
      this.customerRequestModel.customerMatch = true;
    }
    if (type == 'partial') {
      this.CustomerModel.exactMatch = false;
      this.CustomerModel.partialMatch = true;
      this.customerRequestModel.customerMatch = false;
    }
  }

  getCustomerReassignmentRoles() {
    this._commonLookupData.GetCustomerReassignmentRoles()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => {
        var data = this._commonLookupData.parseData(response);
        this.RoleList = data.data;
      });
  }

  getTerritoryFromRole(e) {
    const recordId = e.source.id;
    this.teamAssignmentModel.UpdateTeamId = 0;
    this.teamAssignmentModel.AddTeamId = 0;
    this.teamAssignmentModel.RemoveTeamId = 0;
    // Update the checked state
    this.checkedRoles[recordId] = e.checked;

    if (e.checked == true) {
      this.id.push(recordId);
    }
    else {
      var index = this.id.indexOf(recordId, 0);
      this.id.splice(index, 1);
    }

    if (this.id.length > 0) {
      this._commonLookupData.GetCustReassignTerritoriesByRoleIds(this.id)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(res => {
          var data = this._commonLookupData.parseData(res);
          var parsedData = data.data;
          if (data.isSuccess && parsedData.length > 0) {
            this.UpdateTeamList = parsedData;
            this.AddTeamList = parsedData;
            this.RemoveTeamList = parsedData;
          }
          else {
            this.UpdateTeamList = [];
            this.AddTeamList = [];
            this.RemoveTeamList = [];
          }
          this.UpdateTerritoryfilterCtrl.updateValueAndValidity();
          this.AddTerritoryfilterCtrl.updateValueAndValidity();
          this.RemoveTerritoryfilterCtrl.updateValueAndValidity();
        });
    }
    else {
      this._commonLookupData.GetAllTerritoryList()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(response => {
          var data = this._commonLookupData.parseData(response);
          var parsedData = data.data;
          if (data.isSuccess && parsedData.length > 0) {
            this.UpdateTeamList = parsedData;
          }
          else {
            this.UpdateTeamList = [];
          }
          this.UpdateTerritoryfilterCtrl.updateValueAndValidity();
        });

      this._commonLookupData.GetCustReassignTerritoriesByRoleIds(null)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(response => {
          var data = this._commonLookupData.parseData(response);
          var parsedData = data.data;
          if (data.isSuccess && parsedData.length > 0) {
            this.AddTeamList = parsedData;
            this.RemoveTeamList = parsedData;
          }
          else {
            this.AddTeamList = [];
            this.RemoveTeamList = [];
          }
          this.AddTerritoryfilterCtrl.updateValueAndValidity();
          this.RemoveTerritoryfilterCtrl.updateValueAndValidity();
        });
    }
  }
  public showTooltip(e: MouseEvent): void {
    const element = e.target as HTMLElement;
    if ((element.nodeName === 'TD' || element.nodeName === 'TH') && element.offsetWidth < element.scrollWidth) {
      this.tooltipDir.toggle(element);
    } else {
      this.tooltipDir.hide();
    }
  }
  deleteCustomers() {
    this._commonLookupData.confirmDialog('Are you sure you want to delete?', (result: any) => {
      if (result) {
        var request = {
          "Id": this.customerIds,
          "status": true
        }
        this._customerimportService.DeleteCustomers(request)
          .pipe(takeUntil(this.unsubscribe$)
            , switchMap((res: any) => {
              if (res.isSuccess) {
                this._toasterService.pop('success', 'Success', res.message);

                this.gridView = {
                  data: [],
                  total: 0
                };
                this.customerIds = [];
                this.rowsSelected = [];
                this.allRowsSelected = false;
                this.CustomerList = [];
                return this.GetFilteredCustomersRequest();
              }
              else {
                //this.customerIds = [];
                this._toasterService.pop('error', 'Error', res.message);
                return of(null);
              }
            })
          )
          .subscribe({
            next: (res: any) => this.getResponseAllCustomersCall(res)
            , error: (err) => {
              //this.customerIds = [];
              this._toasterService.pop('error', 'Error', err.message);
            }
          });
      }
    });
  }

    activateCustomers() {
    this._commonLookupData.confirmDialog('Are you sure you want to activate?', (result: any) => {
      if (result) {
        var request = {
          "Id": this.customerIds
        }
        this._customerimportService.ActivateCustomers(request)
          .pipe(takeUntil(this.unsubscribe$)
            , switchMap((res: any) => {
              if (res.isSuccess) {
                this._toasterService.pop('success', 'Success', res.message);

                this.gridView = {
                  data: [],
                  total: 0
                };
                this.customerIds = [];
                this.rowsSelected = [];
                this.allRowsSelected = false;
                this.CustomerList = [];
                return this.GetFilteredCustomersRequest();
              }
              else {
                //this.customerIds = [];
                this._toasterService.pop('error', 'Error', res.message);
                return of(null);
              }
            })
          )
          .subscribe({
            next: (res: any) => this.getResponseAllCustomersCall(res)
            , error: (err) => {
              //this.customerIds = [];
              this._toasterService.pop('error', 'Error', err.message);
            }
          });
      }
    });
  }

  saveCustomerDetails() {
    this._appConstant.currentPage = (this.state.skip / this._appConstant.pageSize);
    if (this.customerIds.length == 0) {
      this._toasterService.pop("error", "Error", "Please select atleast one customer to apply changes");
      return;
    }

    if (this.customerIds.length == this.CustomerList.length && this.CustomerList.length > 1) {
      this._commonLookupData.confirmDialog("Are you sure you want to update " + this.CustomerList.length + " customers?", (result: any) => {
        if (result) {
          this.ManageCustomerDetails();
        }
      })
    }
    else {
      this.ManageCustomerDetails();
    }
  }

  ManageCustomerDetails() {
    var request = {
      "updateTerritoryId": this.teamAssignmentModel.UpdateTeamId,
      "addTeamId": this.teamAssignmentModel.AddTeamId,
      "deleteTeamId": this.teamAssignmentModel.RemoveTeamId,
      "customerIds": this.customerIds
    }
    this._customerimportService.ChangeCustomerDetails(request)
      .pipe(
        takeUntil(this.unsubscribe$),
        switchMap((res: any) => {
          if (res.isSuccess) {
            this._toasterService.pop('success', 'Success', this.customerIds.length + " customers updated successfully");
            this.clearAllRoleCheckboxes();
            this.teamAssignmentModel.UpdateTeamId = 0;
            this.teamAssignmentModel.AddTeamId = 0;
            this.teamAssignmentModel.RemoveTeamId = 0;
            this.getUpdateTerritoryList();
            this.getAddRemoveTeam();
            /**Reset grid records but not column filter */
            this.gridView = {
              data: [],
              total: 0
            };
            this.customerIds = [];
            this.rowsSelected = [];
            this.allRowsSelected = false;
            this.CustomerList = [];
            // Trigger the second API call after first API
            this.customerRequestModel.page = 0;
            this.customerRequestModel.userId = this.SugarCRMUser.userId;
            this.customerRequestModel.territoryId = this.SugarCRMUser.territoryId;
            this.customerRequestModel.address = this.CustomerModel.address;
            this.customerRequestModel.customerName = this.CustomerModel.customerName;
            this.customerRequestModel.accountType = this.CustomerModel.accountTypeId != null ? this.CustomerModel.accountTypeId : 0;
            this.customerRequestModel.includeDeleted = this.CustomerModel.includeDeleted;
            this.customerRequestModel.city = this.CustomerModel.cityId;
            this.customerRequestModel.state = this.CustomerModel.stateId;
            this.customerRequestModel.zipCode = this.CustomerModel.zipCode;
            return this._customersService.GetAllCustomers(this.customerRequestModel);
          }
          else {
            // this.customerIds = [];
            this._toasterService.pop('error', 'Error', res.message);
            return of(null);
          }
        })
      )
      .subscribe({
        next: (resultOfGetAllCustomers) => {
          if (resultOfGetAllCustomers) {
            var data = this._commonLookupData.parseData(resultOfGetAllCustomers);
            this.CustomerList = data.data;
            this.selectAllRows(false);
            if (this.state && this.state.skip)
              this.state.skip = 0;
            // this.state = {
            //   skip: this._appConstant.pageSize * this._appConstant.currentPage,
            //   take: this.pageSize
            // };
            this.loadItems();
          }
        },
        error: (err) => {
          //this.customerIds = [];
          this._toasterService.pop('error', 'Error', err.message);
        }
      });
  }

  checkIfTokenIsNull(tokenData) {
    if (tokenData == null) {
      this._toasterService.pop("error", "Error", "Token Expired. Please login again");
      this._router.navigate(['/']);
      return;
    }
  }

  Clear() {
    this.CustomerModel = new CustomerReassignmentModel();
    this.SugarCRMUser.userId = 0;
    this.SugarCRMUser.territoryId = 0;
    this.SugarCRMUser.teamId = 0;
    this.CustomerModel.accountTypeId = 0;
    this.CustomerModel.parentNumber = undefined;
    this.teamAssignmentModel.UpdateTeamId = 0;
    this.teamAssignmentModel.AddTeamId = 0;
    this.teamAssignmentModel.RemoveTeamId = 0;
    this.clearAllRoleCheckboxes();
    this.gridView = {
      data: [],
      total: 0
    };
    this.resetGrid();
  }

  Apply() {
    this.resetGrid();
    this._appConstant.currentPage = 0;
    this.GetFilteredCustomerList();
  }

  resetGrid() {
    this.showGrid = false;
    this.customerIds = [];
    this.rowsSelected = [];
    this.allRowsSelected = false;
    this.CustomerList = [];
    const resetState: DataStateChangeEvent = {
      skip: 0, // Reset to first page if desired
      take: this.pageSize, // Keep the current page size
      sort: [], // Clear sorting
      filter: { logic: 'and', filters: [] }, // Clear filters
      group: [] // If you use grouping, clear it too
    };
    // Trigger the dataStateChange event handler with the reset state
    this.dataStateChange(resetState);
    this.loadItems();
    this.selectAllRows(false);
    setTimeout(() => {
      this.isGridFilterable = false; // Disable filtering temporarily
    }, 0);

    setTimeout(() => {
      this.isGridFilterable = true;  // Re-enable filtering, UI will be clean
      this.showGrid = true;
    }, 10); // A short delay is often sufficient
  }

  loadItems(): void {
    this.gridView = process(this.CustomerList, { filter: this.state.filter });
  }

  dataStateChange(state): void {
    console.log('dataStateChange ', state)
    this.state = state;
    this.loadItems();
    this.verifySelectedRows();
  }

  private verifySelectedRows(): void {
    //only run if all selected
    if (this.allRowsSelected) {
      this.customerIds = [];
      //only for filtered records
      if (this.state.filter
        && this.state.filter.filters
        && this.state.filter.filters.length > 0
      ) {
        // any filtered records
        // Apply the same filter to the entire dataset
        if (this.gridView.data.length > 0) {
          this.CustomerList.forEach(element => {
            //add only in filtered record
            if (this.gridView.data.includes(element)) {
              element.checked = true;
              this.customerIds.push(Number(element.customerId));
            }
            else {
              //undo not in filtered record
              element.checked = false;
            }
          });
        } else {
          // if filter return no records
          this.CustomerList.forEach(element => {
            element.checked = false;
          });
          this.rowsSelected = [];
          this.customerIds = [];
        }
      }
      else {
        // on clear all filter select all back
        this.CustomerList.forEach(element => {
          element.checked = true;
          this.customerIds.push(Number(element.customerId));
        });
      }
      console.log('customerIds', this.customerIds)
    }
  }

  rowsSelectedKeys(context: RowArgs): number {
    return context.dataItem.id;
  }

  selectAllRows(e): void {
    this.rowsSelected = [];
    this.customerIds = [];
    if (e.checked) {
      this.allRowsSelected = true;
      this.verifySelectedRows();
    } else {
      this.allRowsSelected = false;
      this.CustomerList.forEach(element => {
        element.checked = false;
      });
      this.rowsSelected = [];
      this.customerIds = [];
    }
  }
  selectRow(e) {
    if (e.checked) {
      this.customerIds.push(e.source.id);
      this.CustomerList.find(x => x.customerId == e.source.id).checked = true;
    }
    else {
      var index = this.customerIds.indexOf(e.source.id, 0);
      this.CustomerList.find(x => x.customerId == e.source.id).checked = false;
      this.customerIds.splice(index, 1)
    }

    if (this.CustomerList.filter(x => x.checked).length == this.CustomerList.length) {
      this.allRowsSelected = true;
    }
    else {
      this.allRowsSelected = false;
    }
  }

  getResponseAllCustomersCall(res: any): void {
    if (res) {
      var data = this._commonLookupData.parseData(res);
      this.CustomerList = data.data;
      if (this.state && this.state.skip)
        this.state.skip = 0;
      this.loadItems();
    }
  }

  GetFilteredCustomerList() {
    if (this.SugarCRMUser.userId == 0 && this.SugarCRMUser.territoryId == 0
      && (this.CustomerModel.customerName == "" || this.CustomerModel.customerName == null)
      && this.CustomerModel.cityId == "" && this.CustomerModel.stateId == ""
      && (this.CustomerModel.zipCode == "" || this.CustomerModel.zipCode == null)
      && (!this.CustomerModel.parentNumber || this.CustomerModel.parentNumber == "")
      && this.SugarCRMUser.teamId == 0) {
      this._toasterService.pop("error", "Error", "Please select atleast one criteria to filter results");
      return;
    }
    this.customerRequestModel.page = 0;
    this.customerRequestModel.userId = this.SugarCRMUser.userId;
    this.customerRequestModel.territoryId = this.SugarCRMUser.territoryId;
    this.customerRequestModel.address = this.CustomerModel.address;
    this.customerRequestModel.customerName = this.CustomerModel.customerName;
    this.customerRequestModel.accountType = this.CustomerModel.accountTypeId != null ? this.CustomerModel.accountTypeId : 0;
    this.customerRequestModel.includeDeleted = this.CustomerModel.includeDeleted;
    this.customerRequestModel.city = this.CustomerModel.cityId;
    this.customerRequestModel.state = this.CustomerModel.stateId;
    this.customerRequestModel.zipCode = this.CustomerModel.zipCode;
    this.customerRequestModel.parentNumber = this.CustomerModel.parentNumber;
    this.customerRequestModel.teamId = this.SugarCRMUser.teamId;

    this.showGrid = false;
    this.gridView = {
      data: [],
      total: 0
    };
    this.customerIds = [];
    this.rowsSelected = [];
    this.allRowsSelected = false;
    this.CustomerList = [];
    this._customersService.GetAllCustomers(this.customerRequestModel)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => { this.getResponseAllCustomersCall(response); this.showGrid = true; });
  }

  GetFilteredCustomersRequest() {
    if (this.SugarCRMUser.userId == 0 && this.SugarCRMUser.territoryId == 0 && this.SugarCRMUser.teamId == 0
      && (this.CustomerModel.customerName == "" || this.CustomerModel.customerName == null)
      && this.CustomerModel.cityId == "" && this.CustomerModel.stateId == ""
      && (this.CustomerModel.zipCode == "" || this.CustomerModel.zipCode == null)
      && (this.CustomerModel.parentNumber == "" || this.CustomerModel.parentNumber == null)
    ) {
      this._toasterService.pop("error", "Error", "Please select atleast one criteria to filter results");
      return;
    }
    this.customerRequestModel.page = 0;
    this.customerRequestModel.userId = this.SugarCRMUser.userId;
    this.customerRequestModel.territoryId = this.SugarCRMUser.territoryId;
    this.customerRequestModel.address = this.CustomerModel.address;
    this.customerRequestModel.customerName = this.CustomerModel.customerName;
    this.customerRequestModel.accountType = this.CustomerModel.accountTypeId != null ? this.CustomerModel.accountTypeId : 0;
    this.customerRequestModel.includeDeleted = this.CustomerModel.includeDeleted;
    this.customerRequestModel.city = this.CustomerModel.cityId;
    this.customerRequestModel.state = this.CustomerModel.stateId;
    this.customerRequestModel.zipCode = this.CustomerModel.zipCode;
    return this._customersService.GetAllCustomers(this.customerRequestModel);
  }

  exportFiltereData(kendoExcelexport: any): void {
    if (kendoExcelexport) {
      kendoExcelexport.save()
    }
  }

  // Function to clear all checkboxes
  clearAllRoleCheckboxes(): void {
    this.RoleList.forEach(role => {
      this.checkedRoles[role.recordId] = false;
    });
  }

}
