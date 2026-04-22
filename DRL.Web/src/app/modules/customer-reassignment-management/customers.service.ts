import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/services/Httpservice';
import { AppConstant } from 'src/app/app.constants';
import { CustomerRequestModel } from 'src/app/Models/CustomerRequestModel';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }

  GetActionHistory(request: any) {
    const apiURL = this._appConstant.APIUrl + 'CustomerReassignment/ActionHistory';
    console.log('request: ', request);
    return this.http.post(apiURL, request);
  }

  GetAllCustomers(requestModel: CustomerRequestModel) {
    const apiURL = this._appConstant.APIUrl + 'CustomerReassignment/Customers';
    return this.http.post(apiURL, requestModel);
  }

  GetReassignmentUser(territoryId, userName) {
    territoryId = (territoryId == "" ? "0" : territoryId);
    let params = new HttpParams();
    params = params.append('page', "0");
    params = params.append('pageSize', "0");
    params = params.append('territory', territoryId == "" ? "0" : territoryId);
    params = params.append('userName', userName == "" || userName == null ? null : userName);
    const apiURL = this._appConstant.APIUrl + 'CustomerReassignment/Users/0/0/' + territoryId + '/' + (userName == "" || userName == null ? null : userName);
    return this.http.get(apiURL, { params: params });
  }


  DeleteCustomers(customerIds) {
    const apiURL = this._appConstant.APIUrl + 'CustomerReassignment/DeleteCustomers';
    return this.http.patch(apiURL, customerIds)
      .map(response => {
        return response;
      });
  }

  ChangeUserDetails(request) {
    const apiURL = this._appConstant.APIUrl + 'CustomerReassignment/ChangeUserDetails';
    return this.http.post(apiURL, request)
      .map(response => {
        return response;
      });
  }
}
