import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { BDModel } from 'src/app/Models/BDModel';
import { ENTRequestModel } from 'src/app/Models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class BdService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }

  GetAllBDs() {
    const apiURL = this._appConstant.APIUrl + 'BD/GetAllBDs';
    return this.http.get(apiURL);
  }

  manageBD(bd: BDModel) {
    const apiURL = this._appConstant.APIUrl + 'BD/ManageBD';
    return this.http.post(apiURL, bd).map(response => {
      return response;
    });
  }

  GetBDDetails(id) {
    const apiURL = this._appConstant.APIUrl + 'BD/GetBD/';
    return this.http.get(apiURL + id);
  }

  DeleteBD(bd: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'BD/DeleteBDbyBDId';
    return this.http.patch(apiURL, bd)
      .map(response => {
        return response;
      });
  }

  ManageBDStatus(bd: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'BD/ManageBDStatus';
    return this.http.patch(apiURL, bd)
      .map(response => {
        return response;
      });
  }
}
