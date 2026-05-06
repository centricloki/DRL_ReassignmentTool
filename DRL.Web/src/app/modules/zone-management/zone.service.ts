import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { ZoneModel } from 'src/app/Models/ZoneModel';
import { ENTRequestModel } from 'src/app/Models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class ZoneService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }


  GetAllZones() {
    const apiURL = this._appConstant.APIUrl + 'Zone/GetAllZones';
    return this.http.get(apiURL);
  }

  manageZone(Zone: ZoneModel) {
    const apiURL = this._appConstant.APIUrl + 'Zone/ManageZone';
    return this.http.post(apiURL, Zone).map(response => {
      return response;
    });
  }

  GetZoneDetails(id) {
    const apiURL = this._appConstant.APIUrl + 'Zone/GetZone/';
    return this.http.get(apiURL + id);
  }

  DeleteZone(Zone: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'Zone/DeleteZonebyZoneId';
    return this.http.patch(apiURL, Zone)
      .map(response => {
        return response;
      });
  }

  ManageZoneStatus(Zone: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'Zone/ManageZoneStatus';
    return this.http.patch(apiURL, Zone)
      .map(response => {
        return response;
      });
  }
}