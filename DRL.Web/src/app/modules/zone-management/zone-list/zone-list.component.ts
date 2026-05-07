import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ZoneService } from '../zone.service';
import { ConfirmDialogComponent } from 'src/app/confirm-dialog/confirm-dialog.component';
import { WarningDialogComponent } from 'src/app/warning-dialog/warning-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ToastHelper } from 'src/app/helpers/toast.helper';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommonService } from 'src/app/services/common.service';
import { AppConstant } from 'src/app/app.constants';
import { ENTRequestModel } from 'src/app/Models/UserModel';
import { ResponseParser } from 'src/app/helpers/response-parser.helper'; // ✅ New import

@Component({
  selector: 'app-zone-list',
  templateUrl: './zone-list.component.html',
  styleUrls: ['./zone-list.component.css']
})
export class ZoneListComponent implements OnInit, OnDestroy {

  zoneList: any[] = [];
  CopyZoneList: any[] = [];

  displayedColumns: string[] = ['zoneName', 'avpName', 'isActive', 'actions'];
  isLoading = false;

  // Kendo Grid state management (client-side)
  public state: State = {
    skip: 0,
    take: this._appConstantService.pageSize
  };

  // Grid data result for Kendo binding
  public gridView: GridDataResult = {
    data: this.zoneList.slice(0, this._appConstantService.pageSize),
    total: this.zoneList.length
  };

  private unsubscribe$ = new Subject<void>();

  constructor(
    private zoneService: ZoneService,
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
    this.GetAllZones();
  }

  public dataStateChange(state: State): void {
    this.state = state;
    this.loadItems();
  }

  private loadItems(): void {
    this.gridView = process(this.zoneList, this.state);
  }

  public activeInactiveFilterChange(event: any): void {
    this._appConstantService.selectedValue = event.value;
    this.state.skip = 0;

    this.zoneList = [...this.CopyZoneList];

    if (event.value === 1) {
      this.zoneList = this.zoneList.filter((z: any) => z.isActive === true || z.isActive === 1);
    } else if (event.value === 0) {
      this.zoneList = this.zoneList.filter((z: any) => z.isActive === false || z.isActive === 0);
    }

    this.loadItems();
  }

  /**
   * Load zones - Using ResponseParser helper for reusability
   */
  GetAllZones() {
    this.isLoading = true;

    this.zoneService.GetAllZones().pipe(takeUntil(this.unsubscribe$)).subscribe(response => {
      // ✅ REUSABLE: Use ResponseParser helper instead of inline double-parse
      const parsedData = ResponseParser.parseLegacyResponse(response);

      if (parsedData && parsedData.isSuccess) {
        this.zoneList = parsedData.data || [];
        this.CopyZoneList = [...this.zoneList];

        this.state = {
          skip: 0,
          take: this._appConstantService.pageSize
        };
        this.loadItems();
      } else {
        const errorMsg = ResponseParser.getErrorMessage(response, 'Failed to load zones');
        this.toastr.error('Error', errorMsg);
      }
      this.isLoading = false;
    }, error => {
      this.isLoading = false;
      this.toastr.error('Error', 'Failed to load zones');
    });
  }

  editZone(zoneId: string) {
    this._appConstantService.zoneId = zoneId;
    this.router.navigate(['/zones/edit']);
  }

  // deleteZone(zone: any) {
  //   this._commonLookupData.confirmDialog(`Are you sure you want to delete zone "${zone.zoneName}"?`, (result: any) => {
  //     if (result) {
  //       const deleteRequest = new ENTRequestModel();
  //       deleteRequest.id = zone.zoneId;
  //       deleteRequest.status = true;

  //       this.zoneService.DeleteZone(deleteRequest).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
  //         // ✅ REUSABLE: Use helper for parsing
  //         const data = ResponseParser.parseLegacyResponse(res);

  //         if (data && data.isSuccess) {
  //           this.toastr.success('Success', data.message);
  //           this.GetAllZones();
  //         } else {
  //           this.toastr.error('Error', (data && data.message) ? data.message : 'Failed to delete zone');
  //         }
  //       }, (error: any) => {
  //         this.toastr.error('Error', (error && error.message) ? error.message : 'Failed to delete zone');
  //       });
  //     }
  //   });
  // }

  toggleZoneStatus(zone: any) {
    const status = zone.isActive ? 'deactivate' : 'activate';

    this._commonLookupData.confirmDialog(`Are you sure you want to ${status} zone "${zone.zoneName}"?`, (result: any) => {
      if (result) {
        const statusRequest = new ENTRequestModel();
        statusRequest.id = zone.zoneId;
        statusRequest.status = !zone.isActive;

        this.zoneService.ManageZoneStatus(statusRequest).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          // ✅ REUSABLE: Use helper for parsing
          const data = ResponseParser.parseLegacyResponse(res);

          if (data && data.isSuccess) {
            this.toastr.success('Success', data.message);
            zone.isActive = statusRequest.status;
            this.loadItems();
          } else {
            this.toastr.error('Error', (data && data.message) ? data.message : `Failed to ${status} zone`);
          }
        }, (error: any) => {
          this.toastr.error('Error', (error && error.message) ? error.message : `Failed to ${status} zone`);
        });
      }
    });
  }

  addNewZone() {
    this._appConstantService.zoneId = ''
    this.router.navigate(['/zones/create']);
  }
}