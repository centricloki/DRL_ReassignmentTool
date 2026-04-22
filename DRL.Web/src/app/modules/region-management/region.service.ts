import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { RegionModel } from 'src/app/Models/RegionModel';
import { ENTRequestModel } from 'src/app/Models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class RegionService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }


  GetAllRegion() {
    const apiURL = this._appConstant.APIUrl + 'Region/GetAllRegions';
    return this.http.get(apiURL);
  }

  manageRegion(Region: RegionModel) {
    const apiURL = this._appConstant.APIUrl + 'Region/ManageRegion';
    return this.http.post(apiURL, Region).map(response => {
      return response;
    });
  }

  GetRegionDetails(id) {
    const apiURL = this._appConstant.APIUrl + 'Region/GetRegion/';
    return this.http.get(apiURL + id);
  }

  DeleteRegion(Region: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'Region/DeleteRegionbyRegionId';
    return this.http.patch(apiURL, Region)
      .map(response => {
        return response;
      });
  }

  ManageRegionStatus(Region: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'Region/ManageRegionStatus';
    return this.http.patch(apiURL, Region)
      .map(response => {
        return response;
      });
  }
}
