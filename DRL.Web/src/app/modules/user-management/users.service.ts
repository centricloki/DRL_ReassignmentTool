import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { HttpParams } from '@angular/common/http';
import { UserModel, ENTRequestModel } from 'src/app/Models/UserModel';
import { RequestOptions, Headers } from '@angular/http';


@Injectable()
export class UsersService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }
  getHeaders() {
    return { 'Content-Type': 'application/json', 'user-id': 'test', 'Access-Control-Allow-Origin': '*', 'Access-Control-Allow-Headers': '*' };
  }
  GetUserList() {
    let headers = new Headers();
    headers.append('Access-Control-Allow-Origin', '*');
    const apiURL = this._appConstant.APIUrl + 'User/GetUserList';
    return this.http.get(apiURL);
    //return this.http.get(apiURL,{withCredentials: true,headers}).toPromise();
  }

  // GetAllUsers() {
  //   const apiURL = this._appConstant.APIUrl + 'User/GetAllUsers';
  //   return this.http.get(apiURL);
  // }

  GetUser(id) {
    let params = new HttpParams();
    params = params.append('userId  ', id);
    const apiURL = this._appConstant.APIUrl + 'User/GetUser/' + id;
    return this.http.get(apiURL);
  }

  ManageUser(user: UserModel) {
    const requestModel = this.cleanAndFormatObject(user);
    // requestModel.createdDate=user.createdDate.toISOString();
    // requestModel.updatedDate=user.updatedDate.toISOString();
    const apiURL = this._appConstant.APIUrl + 'User/ManageUser';
    return this.http.post(apiURL, requestModel)
      .map(response => {
        return response;
      });
    // return this.http.post(apiURL, user)
    //   .map(response => {
    //     return response;
    //   });
  }

  ManageUserStatus(user: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'User/ManageUserStatus';
    return this.http.patch(apiURL, user)
      .map(response => {
        return response;
      });
  }

  DeleteUser(role: ENTRequestModel) {
    let headers = new Headers();
    headers.append('Access-Control-Allow-Origin', '*');

    const apiURL = this._appConstant.APIUrl + 'User/DeleteUserbyUserId';
    return this.http.patch(apiURL, role, { withCredentials: true, headers })
      .map(response => {
        return response;
      });
  }

  GetAllUsersByRoleId(id) {
    let params = new HttpParams();
    params = params.append('roleId  ', id);
    const apiURL = this._appConstant.APIUrl + 'User/GetAllUsersByRoleId/' + id;
    return this.http.get(apiURL, { params: params });
  }

  GetAllUsersByRegionId(id) {
    let params = new HttpParams();
    params = params.append('regionId  ', id);
    const apiURL = this._appConstant.APIUrl + 'User/GetAllUsersByRegionId/' + id;
    return this.http.get(apiURL, { params: params });
  }
  GetAllUsersByTerritoryId(id) {
    let params = new HttpParams();
    params = params.append('territoryId  ', id);
    const apiURL = this._appConstant.APIUrl + 'User/GetAllUsersByTerritoryId/' + id;
    return this.http.get(apiURL, { params: params });
  }

  GetAllUsersByManagerId(id) {
    let params = new HttpParams();
    params = params.append('managerId  ', id);
    const apiURL = this._appConstant.APIUrl + 'User/GetAllUsersByManagerId/' + id;
    return this.http.get(apiURL, { params: params });
  }

  UpdateUserTerritory(userId, territoryId) {
    const apiURL = this._appConstant.APIUrl + 'User/UpdateUserTerritory/' + userId + '/' + territoryId;
    return this.http.patch(apiURL, '')
      .map(response => {
        return response;
      });
  }

  DeleteUserTerritory(userId, territoryId) {
    const apiURL = this._appConstant.APIUrl + 'User/DeleteUserTerritory/' + userId + '/' + territoryId;
    return this.http.patch(apiURL, '')
      .map(response => {
        return response;
      });
  }

  GetUsersByTerritoryIdAndUserId(territoryId, userId) {
    let params = new HttpParams();
    params = params.append('TerritoryId  ', territoryId);
    params = params.append('userId  ', userId);
    const apiURL = this._appConstant.APIUrl + 'User/GetUsersByTerritoryIdAndUserId/' + territoryId + '/' + userId;
    return this.http.get(apiURL, { params: params });
  }

  cleanAndFormatObject(obj: any): any {
    if (Array.isArray(obj)) {
      return obj
        .filter(item => item !== null && item !== "" && item !== undefined)
        .map(item => this.cleanAndFormatObject(item));
    } else if (typeof obj === "object" && obj !== null) {
      return Object.entries(obj)
        .filter(([_, value]) => value !== "" && value !== null && value !== undefined && !(Array.isArray(value) && value.length === 0))
        .reduce((acc, [key, value]) => {
          if (value instanceof Date) {
            acc[key] = value.toISOString(); // Convert Date to ISO string
          } else {
            acc[key] = this.cleanAndFormatObject(value);
          }
          return acc;
        }, {} as any);
    }
    return obj;
  }

  GetAllTerritories() {
    let params = new HttpParams();
    const apiURL = this._appConstant.APIUrl + 'User/Territories';
    return this.http.get(apiURL);
  }

  GetAllZones() {
    let params = new HttpParams();
    const apiURL = this._appConstant.APIUrl + 'User/Zones';
    return this.http.get(apiURL);
  }

  GetAllTerritoriesForBD(bdId:number) {
    let params = new HttpParams();
    const apiURL = this._appConstant.APIUrl + `User/BD/${bdId}/Territories`;
    return this.http.get(apiURL);
  }

  GetAllTerritoriesForUser(userId:number) {
    let params = new HttpParams();
    const apiURL = this._appConstant.APIUrl + `User/${userId}/Territories`;
    return this.http.get(apiURL);
  }

  GetAllZonesForAVP(zoneId:number) {
    let params = new HttpParams();
    const apiURL = this._appConstant.APIUrl + `User/AVP/${zoneId}/Zones`;
    return this.http.get(apiURL);
  }

  getRoleByName(roleName:string){
      const apiURL = this._appConstant.APIUrl + `Role/GetRoleByName/${roleName}`;
      return this.http.get(apiURL);
  }
  
}
