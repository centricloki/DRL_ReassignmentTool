import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { HttpParams } from '@angular/common/http';
import { TeamModel } from 'src/app/Models/TeamModel';
import { ENTRequestModel } from 'src/app/Models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class TeamsService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }

  GetAllTerritories() {
    const apiURL = this._appConstant.APIUrl + 'Territory/GetAllTerritories';
    return this.http.get(apiURL);
  }

  GetTerritory(id) {
    let params = new HttpParams();
    params = params.append('TerritoryId  ', id);
    const apiURL = this._appConstant.APIUrl + 'Territory/GetTerritory/' + id;
    return this.http.get(apiURL, { params: params });
  }

  ManageTeam(TeamModel:TeamModel) {
    const apiURL = this._appConstant.APIUrl + 'Territory/ManageTerritory';
    return this.http.post(apiURL,TeamModel)
      .map(response => {
        return response;
      });
  }

  DeleteTeam(team: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'Territory/DeleteTerritorybyTerritoryId';
    return this.http.patch(apiURL, team)
      .map(response => {
        return response;
      });
  }
  GetTeamListFromRegionId(id) {
    const apiURL = this._appConstant.APIUrl + 'Territory/GetTeamListFromRegionId/';
    return this.http.get(apiURL + id);
  }

}
