import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { BdService } from '../bd.service';
import { MatDialog } from '@angular/material/dialog';
import { ToastHelper } from 'src/app/helpers/toast.helper';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommonService } from 'src/app/services/common.service';
import { AppConstant } from 'src/app/app.constants';
import { ENTRequestModel } from 'src/app/Models/UserModel';
import { ResponseParser } from 'src/app/helpers/response-parser.helper';

@Component({
  selector: 'app-bd-list',
  templateUrl: './bd-list.component.html',
  styleUrls: ['./bd-list.component.css']
})
export class BdListComponent implements OnInit, OnDestroy {

  bdList: any[] = [];
  CopyBdList: any[] = [];

  displayedColumns: string[] = ['bdName', 'isActive', 'actions'];
  isLoading = false;

  public state: State = {
    skip: 0,
    take: this._appConstantService.pageSize
  };

  public gridView: GridDataResult = {
    data: this.bdList.slice(0, this._appConstantService.pageSize),
    total: this.bdList.length
  };

  private unsubscribe$ = new Subject<void>();

  constructor(
    private bdService: BdService,
    private router: Router,
    private toastr: ToastHelper,
    public dialog: MatDialog,
    private _commonLookupData: CommonService,
    public _appConstantService: AppConstant
  ) { }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit(): void {
    this.GetAllBDs();
  }

  public dataStateChange(state: State): void {
    this.state = state;
    this.loadItems();
  }

  private loadItems(): void {
    this.gridView = process(this.bdList, this.state);
  }

  public activeInactiveFilterChange(event: any): void {
    this._appConstantService.selectedValue = event.value;
    this.state.skip = 0;

    this.bdList = [...this.CopyBdList];

    if (event.value === 1) {
      this.bdList = this.bdList.filter((b: any) => b.isActive === true || b.isActive === 1);
    } else if (event.value === 0) {
      this.bdList = this.bdList.filter((b: any) => b.isActive === false || b.isActive === 0);
    }

    this.loadItems();
  }

  GetAllBDs() {
    this.isLoading = true;

    this.bdService.GetAllBDs().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      const parsedData = ResponseParser.parseLegacyResponse(response);

      if (parsedData && parsedData.isSuccess) {
        this.bdList = parsedData.data || [];
        this.CopyBdList = [...this.bdList];

        this.state = {
          skip: 0,
          take: this._appConstantService.pageSize
        };
        this.loadItems();
      } else {
        const errorMsg = ResponseParser.getErrorMessage(response, 'Failed to load BDs');
        this.toastr.error('Error', errorMsg);
      }
      this.isLoading = false;
    }, error => {
      this.isLoading = false;
      this.toastr.error('Error', 'Failed to load BDs');
    });
  }

  editBD(bdId: string) {
    this._appConstantService.bdId = bdId;
    this.router.navigate(['/bd/edit']);
  }

  toggleBDStatus(bd: any) {
    const status = bd.isActive ? 'deactivate' : 'activate';

    this._commonLookupData.confirmDialog(`Are you sure you want to ${status} Business Division "${bd.bdName}"?`, (result: any) => {
      if (result) {
        const statusRequest = new ENTRequestModel();
        statusRequest.id = bd.bdid; // note: casing matches BDModel
        statusRequest.status = !bd.isActive;

        this.bdService.ManageBDStatus(statusRequest).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          const data = ResponseParser.parseLegacyResponse(res);

          if (data && data.isSuccess) {
            this.toastr.success('Success', data.message);
            bd.isActive = statusRequest.status;
            this.loadItems();
          } else {
            this.toastr.error('Error', (data && data.message) ? data.message : `Failed to ${status} BD`);
          }
        }, (error: any) => {
          this.toastr.error('Error', (error && error.message) ? error.message : `Failed to ${status} BD`);
        });
      }
    });
  }

  addNewBD() {
    this._appConstantService.bdId = '';
    this.router.navigate(['/bd/create']);
  }
}
