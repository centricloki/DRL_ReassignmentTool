import { Component, OnInit, OnDestroy } from '@angular/core';
import { CustomersService } from '../customers.service';
import { CommonService } from 'src/app/services/common.service';
import { AppConstant } from 'src/app/app.constants';
import { DatePipe } from '@angular/common';
import { ActionHistoryModel } from 'src/app/Models/ActionHistoryModel';
import { DataStateChangeEvent, GridDataResult } from '@progress/kendo-angular-grid';
import { SortDescriptor, State, process } from '@progress/kendo-data-query';
import { FormGroup, FormControl, FormBuilder } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { catchError, debounceTime, subscribeOn, switchMap, takeUntil } from 'rxjs/operators';
declare var $: any;
@Component({
  selector: 'app-action-history',
  templateUrl: './action-history.component.html',
  styleUrls: ['./action-history.component.css'],
  providers: [DatePipe]
})
export class ActionHistoryComponent implements OnInit, OnDestroy {

  constructor(private _commonLookupData: CommonService,
    public _appConstant: AppConstant,
    public _customersService: CustomersService,
    private datePipe: DatePipe, private fb: FormBuilder) { }

  private dataStateSubject: Subject<DataStateChangeEvent> = new Subject<DataStateChangeEvent>();
  private unsubscribe$ = new Subject<void>();
  ActionHistoryList: ActionHistoryModel[] = [];
  CopyActionHistoryList: ActionHistoryModel[] = [];
  gridView: GridDataResult = {
    data: [],
    total: 0
  };
  public skip: number = 0;   // To track current skip
  public pageSize: number = this._appConstant.pageSize;  // Set the page size
  filters = {}; // Store filters globally
  sortColumn = "";
  sortDirection = "";
  state: State = {
    skip: this.skip,
    take: this.pageSize,
    filter: null,
    sort: [],
  };
  waitingTime: number = 800;
  historyForm: FormGroup;

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit() {
    this.initializeForm();
    this.trackFormChanges();
    this.trackPageSortChanges();
    this.refreshGrid();
  }

  initializeForm() {
    this.historyForm = this.fb.group({
      module: [''],
      operation: [''],
      customerNumber: [''],
      accountName: [''],
      team: [''],
      updatedBy: [''],
      updatedDate: ['']
    });
  }

  trackFormChanges() {
    this.historyForm.valueChanges
      .pipe(takeUntil(this.unsubscribe$),
        debounceTime(this.waitingTime)) // Delay in ms (500ms after the last change)
      .subscribe((val: any) => {
        console.log('historyForm val', val)
        this.filters = val;

        if (val.updatedDate) {
          const validDate = this.getValidDateOnly(val.updatedDate);
          if (validDate)
            this.filters['updatedDate'] = validDate;
        }

        this.skip = 0;//reset paging
        // this.sortColumn = this.sortDirection = ""; //reset sorting
        // this.state.sort = [];
        this.refreshGrid();
      });
  }

  trackPageSortChanges() {
    // Apply debouncing on the dataStateChange event
    this.dataStateSubject.pipe(
      takeUntil(this.unsubscribe$),
      debounceTime(200), // Delay in ms (500ms after the last change)
      switchMap((argState: DataStateChangeEvent) => {
        console.log('DataStateChangeEvent :', argState)
        this.state = argState;
        this.skip = argState.skip;
        //this.pageSize = state.take;
        // Make API call with the new state (pagination, filter, sorting)
        return this.GetAllActionHistory()
      }),
      catchError(err => {
        console.error(err);
        return Observable.of([]); // Return empty array on error
      })
    ).subscribe(response => this.formatGridData(response));
  }

  getValidDateOnly(dateString: string): string {
    // Create Date object
    const dateValue = new Date(dateString);

    // Check if the date is valid
    if (!isNaN(dateValue.getTime())) {
      return dateValue.toISOString().split('T')[0]; // Extract YYYY-MM-DD
    } else {
      return null;
    }
  }

  // Function to remove null or empty properties from an object
  removeNullOrEmptyProperties(obj: any): any {
    const filteredObj = Object.keys(obj).reduce((acc, key) => {
      // Only add properties that are not null or empty string
      if (obj[key] !== null && obj[key] !== '') {
        acc[key] = obj[key];
      }
      return acc;
    }, {});

    return filteredObj;
  }

  GetAllActionHistory() {
    const _sortColumn = ((this.state.sort && this.state.sort[0] && this.state.sort[0].field) || "");
    const _sortDirection = ((this.state.sort && this.state.sort[0] && this.state.sort[0].dir) || "");

    if (_sortColumn) {
      if (!_sortDirection) {
        this.sortColumn = this.sortDirection = "";
      }
      else {
        this.sortColumn = _sortColumn;
        this.sortDirection = _sortDirection;
      }
    }


    if (this.sortColumn && (this.sortColumn == 'updateDatestring')) this.sortColumn = 'updatedDate';

    let requestPayload = {
      Skip: (this.skip / this.pageSize) + 1,
      Take: this.pageSize,
      SortColumn: this.sortColumn,
      SortDirection: this.sortDirection,
      Filters: this.removeNullOrEmptyProperties(this.filters), // Send persisted filters
    };

    return this._customersService.GetActionHistory(requestPayload)
      .pipe(takeUntil(this.unsubscribe$));
  }

  formatGridData(response) {
    var offset = new Date().getTimezoneOffset();

    var formattedResponse = this._commonLookupData.parseData(response);
    console.log('formattedResponse: ', formattedResponse);
    if (formattedResponse != null) {
      this.ActionHistoryList = formattedResponse.data.data;
      this.ActionHistoryList.forEach(element => {
        //CST time ism 6 hours minus from UTC time
        var dt = new Date(element.updatedDate).setMinutes((new Date(element.updatedDate).getMinutes() + (-1 * Number(offset))));
        element.updateDatestring = this.datePipe.transform(new Date(dt), "MM/dd/yyyy HH:mm:ss");
      })
      this.CopyActionHistoryList = this.ActionHistoryList;
      this.state = {
        skip: 0,
        //take: this._appConstant.pageSize
      };
      //this.loadItems();
      this.gridView = {
        data: this.ActionHistoryList,
        total: formattedResponse.data.total
      };
    }
  }

  refreshGrid() {
    this.GetAllActionHistory()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(response => this.formatGridData(response));
  }

  loadItems(): void {
    this.gridView = process(this.ActionHistoryList, this.state);
  }
  // Called on dataStateChange to update the state
  dataStateChange(state: DataStateChangeEvent) {
    this.dataStateSubject.next(state); // Emit new state to subject
  }


  clearDateFilter() {
    this.historyForm.get('updatedDate').reset();
  }
}
