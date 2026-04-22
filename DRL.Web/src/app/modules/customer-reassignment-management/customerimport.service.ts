import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConstant } from 'src/app/app.constants';

export interface BaseResponse<T> {
  data: T;
  isSuccess: boolean;
  message: string;
}

export interface UserReportHierarchyNode {
  nodeId: string;
  parentNodeId: string | null;
  userId: number;
  fullName: string;
  userName: string;
  zoneName: string;
  zoneId: string;
  roleName: string;
  roleId: string;
  entityId: string;
  regionName: string;
  regionId: string;
  bdId: string;
  bdName: string;
  avpName: string;
  avpId: string;
  territoryId: string;
  territoryName: string;
  territoryIds: string[];
  territoryNames: string[];
  territoryFilterText: string;
  avpFilter: string;
  zoneFilter: string;
  regionFilter: string;
  bdFilter: string;
  avpDetail: string;
  zoneDetail: string;
  regionDetail: string;
  bdDetail: string;
  territoryDetailIds: string[];
  children: UserReportHierarchyNode[];
  // Manager-related properties
  managerFullName?: string;
  managerUserName?: string;
  managerRoleName?: string;
  managerRoleId?: string;
  managerAvpName?: string;
  managerAvpId?: string;
  managerZoneName?: string;
  managerZoneId?: string;
  managerRegionName?: string;
  managerRegionId?: string;
  managerBdName?: string;
  managerBdId?: string;
  managerTerritoryName?: string;
  managerTerritoryId?: string;
}

export interface CustomerMaster {
  CustomerName: string;
  EmailId: string;
  Address: string;
  AddressCity: string;
  AddressState: string;
  AddressZipCode: string;
  TerritoryCode: string;
  IsParent: boolean;
  Parent: string;
  AccountType: string;
  AccountClassification: string;
  Latitude: number;
  Longitude: number;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerimportService {

  constructor(private http: HttpClient, private appConstant: AppConstant) { }

  addCustomerMaster(customers: CustomerMaster[]): Observable<BaseResponse<any>> {
    const apiURL = this.appConstant.APIUrl + 'CustomerReassignment/AddCustomers';
    return this.http.post<BaseResponse<any>>(apiURL, customers, { withCredentials: true });
  }

  // Brand style api calls
  getBrandStyleMaster(): Observable<BaseResponse<any>> {
    const apiURL = this.appConstant.APIUrl + 'BrandStyle/BrandStyles';
    return this.http.get<BaseResponse<any>>(apiURL);
  }

  updateBrandStyleSortOrder(brandStyleId, sortOrder): Observable<BaseResponse<any>> {
    const apiURL = this.appConstant.APIUrl + 'BrandStyle/ChangeSortOrder/' + brandStyleId + '/' + sortOrder;
    return this.http.post<BaseResponse<any>>(apiURL, { withCredentials: true });
  }

  // User Report api calls
  getUserReportData(): Observable<BaseResponse<any>> {
    const apiURL = this.appConstant.APIUrl + 'UserReport/GetUserReport';
    return this.http.get<BaseResponse<any>>(apiURL);
  }

  getChildUserReportData(roleId, entityId): Observable<BaseResponse<any>> {
    const apiURL = this.appConstant.APIUrl + 'UserReport/GetUserReportByRoleId/' + roleId + '/' + entityId;
    return this.http.get<BaseResponse<any>>(apiURL, { withCredentials: true });
  }

  getUserReportHierarchyData(): Observable<BaseResponse<UserReportHierarchyNode[]>> {
    const apiURL = this.appConstant.APIUrl + 'UserReport/GetUserReportHierarchy';
    return this.http.get<BaseResponse<UserReportHierarchyNode[]>>(apiURL, { withCredentials: true });
  }

  ChangeCustomerDetails(request) {
    const apiURL = this.appConstant.APIUrl + 'CustomerReassignment/ChangeCustomerDetails';
    return this.http.post(apiURL, request, { withCredentials: true });
  }

  DeleteCustomers(customerIds) {
    const apiURL = this.appConstant.APIUrl + 'CustomerReassignment/DeleteCustomers';
    return this.http.patch(apiURL, customerIds)
      .map(response => {
        return response;
      });
  }

  ActivateCustomers(customerIds) {
    const apiURL = this.appConstant.APIUrl + 'CustomerReassignment/ActivateCustomers';
    return this.http.patch(apiURL, customerIds)
      .map(response => {
        return response;
      });
  }

}
