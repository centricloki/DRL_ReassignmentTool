import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { CommonService } from 'src/app/services/common.service';
import { Router } from '@angular/router';
import { AppConstant } from 'src/app/app.constants';
import { RegionService } from '../region.service';
import { ManageRegionComponent } from '../manage-region/manage-region.component';
import { ToasterService } from 'angular2-toaster';
import { ENTRequestModel } from 'src/app/Models/UserModel';
import { UsersService } from '../../user-management/users.service';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { RegionModel } from 'src/app/Models/RegionModel';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-region-list',
  templateUrl: './region-list.component.html',
  styleUrls: ['./region-list.component.css']
})
export class RegionListComponent implements OnInit, OnDestroy {

  constructor(private _commonLookupData: CommonService, private _router: Router, public _appConstant: AppConstant, private _regionService: RegionService,
    private _toasterService: ToasterService, private _userService: UsersService) { }
  private unsubscribe$ = new Subject<void>();

  regionList: any[] = [];
  CopyRegionList: any[] = [];

  gridView: GridDataResult = {
    data: this.regionList.slice(0, this._appConstant.pageSize),
    total: this.regionList.length
  };
  state: State = {
    skip: 0,
    take: this._appConstant.pageSize
  };

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  ngOnInit() {
    this.GetAllRegion();
  }

  GetAllRegion() {
    this._regionService.GetAllRegion().pipe(takeUntil(this.unsubscribe$)).subscribe(data => {
      if (data.ok) {
        var Data = JSON.stringify(data._body);
        var Obj = JSON.parse(Data);
        var data = JSON.parse(Obj);
        if (data.isSuccess) {
          this.regionList = data.data;
          this.CopyRegionList = data.data;
          this.state = {
            skip: 0,
            take: this._appConstant.pageSize
          };
          this.loadItems();
        }
      }
    });
  }

  loadItems(): void {
    this.gridView = process(this.regionList, this.state);
  }
  dataStateChange(state): void {
    this.state = state;
    this.loadItems();
  }

  activeInactiveRegion(regionDetails: RegionModel) {
    var status = regionDetails.isActive ? 'Inactive' : 'active';
    this._commonLookupData.confirmDialog('Are you sure you want to ' + status + ' the region?', (result: any) => {
      if (result) {
        var patchRequest = new ENTRequestModel();
        patchRequest.id = Number(regionDetails.regionId);
        patchRequest.status = !regionDetails.isActive;
        this._regionService.ManageRegionStatus(patchRequest).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
          var data = this._commonLookupData.parseData(res);
          if (data != null && data != "" && data.isSuccess) {
            this._appConstant.roleId = '';
            this._toasterService.pop('success', 'Success', data.message);
            this.GetAllRegion();
          }
          else {
            this._toasterService.pop('error', 'Error', data.message);
          }
        }, (error: any) => {
          this._toasterService.pop('error', 'Error', error.message);
        });
      }
    });
  }


  // deleteRegion(Id) {
  //   this._commonLookupData.confirmDialog('Are you sure you want to delete region?', (result: any) => {
  //     if (result) {
  //       var patchRequest = new ENTRequestModel();
  //       patchRequest.id = Id;
  //       patchRequest.status = true;
  //       this._regionService.DeleteRegion(patchRequest).subscribe(res => {
  //         var data = this._commonLookupData.parseData(res);
  //         if (data != null && data != "" && data.isSuccess) {
  //           this._appConstant.roleId = '';
  //           this._toasterService.pop('success', 'Success', data.message);
  //           this.GetAllRegion();
  //         }
  //         else {
  //           this._toasterService.pop('error', 'Error', data.message);
  //         }
  //       }, (error: any) => {
  //         this._toasterService.pop('error', 'Error', error.message);
  //       });
  //     }
  //   });
  // }
  editRegion(id) {
    this._appConstant.regionId = id;
    this._router.navigate(['/regions/create']);
  };

  addUpdateRegion(sugarRegionData) {
    this._regionService.manageRegion(sugarRegionData).pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
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

  activeInactiveFilterChange(e) {
    this.regionList = this.CopyRegionList;
    if (e.value == 1)//active
    {
      this.regionList = this.regionList.filter(x => x.isActive == 1);
      return this.regionList;
    }
    else if (e.value == 0)//inactive
    {
      this.regionList = this.regionList.filter(x => x.isActive == 0);
      return this.regionList;
    }
    else {
      return this.regionList;
    }
  }
}

