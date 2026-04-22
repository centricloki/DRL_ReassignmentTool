import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { RoleModel } from 'src/app/Models/RoleModel';
import { Observable } from 'rxjs/Observable';
import { Response, RequestOptions, Headers } from '@angular/http';
import { ENTRequestModel } from 'src/app/Models/UserModel';

@Injectable({
  providedIn: 'root'
})
export class RolesService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }


  GetAllRoles() {
    const apiURL = this._appConstant.APIUrl + 'Role/GetAllRoles';
    return this.http.get(apiURL);
  }

  manageRole(Role: RoleModel) {
    const apiURL = this._appConstant.APIUrl + 'Role/ManageRole';
    return this.http.post(apiURL, Role).map(response => {
      return response;
    });
  }

  GetRoleDetails(id) {
    const apiURL = this._appConstant.APIUrl + 'Role/GetRole/';
    return this.http.get(apiURL + id);
  }

  DeleteRole(role: ENTRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'Role/DeleteRolebyRoleId';
    return this.http.patch(apiURL, role)
      .map(response => {
        return response;
      });
  }
}
