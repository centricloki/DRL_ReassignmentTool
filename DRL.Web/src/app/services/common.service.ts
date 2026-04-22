import { Injectable } from '@angular/core';
import { MatDialogConfig, MatDialog } from '@angular/material';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppConstant } from '../app.constants';
import { HttpService } from './Httpservice';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { WarningDialogComponent } from '../warning-dialog/warning-dialog.component';
@Injectable({
    providedIn: 'root'
})
export class CommonService {

    constructor(private dialog: MatDialog,
        private ngbModal: NgbModal,
        private _appConstant: AppConstant,
        private http: HttpService) {

    }

    warningDialog(cnfText, callback) {
        const dialogConfig = new MatDialogConfig();

        dialogConfig.disableClose = true; // Disable close by clicking outside
        dialogConfig.autoFocus = true; // Auto focus on the dialog
        dialogConfig.width = '30%'; // Set dialog width (can be adjusted)
        dialogConfig.data = { message: cnfText };

        const dialogRef = this.dialog.open(WarningDialogComponent, dialogConfig);
        dialogRef.beforeClosed().subscribe(result => {
            callback(result);
        });
    }

    confirmDialog(cnfText, callback) {
        const dialogConfig = new MatDialogConfig();

        dialogConfig.disableClose = true;
        dialogConfig.autoFocus = true;
        dialogConfig.height = 'auto';
        dialogConfig.width = '30%';
        dialogConfig.data = { message: cnfText, showCommonPopup: false, showOnlyConfirmButton: false };

        const dialogRef = this.dialog.open(ConfirmDialogComponent, dialogConfig);

        dialogRef.afterClosed().subscribe(result => {
            callback(result);
        });
    }

    customConfirm(cnfText, callback) {
        const dialogConfig = new MatDialogConfig();

        dialogConfig.disableClose = true;
        dialogConfig.autoFocus = true;
        dialogConfig.height = 'auto';
        dialogConfig.width = '520px';
        dialogConfig.data = { message: cnfText, showCommonPopup: true, showOnlyConfirmButton: false };

        const dialogRef = this.dialog.open(ConfirmDialogComponent, dialogConfig);

        dialogRef.afterClosed().subscribe(result => {
            callback(result);
        });
    }
    parseData(responseData) {
        if (responseData.ok) {
            var Data = JSON.stringify(responseData._body);
            var Obj = JSON.parse(Data);
            return JSON.parse(Obj);
        }
        else {
            return "";
        }
    }
    GetActiveRoles() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/GetRoles';
        return this.http.get(apiURL);
    }

    GetAllTerritoryList() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/GetTerritories';
        return this.http.get(apiURL);
    }

    GetAllZoneList() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/GetZones';
        return this.http.get(apiURL);
    }
    GetZones(userId) {
        const apiURL = this._appConstant.APIUrl + 'Lookup/Zones/' + userId;
        return this.http.get(apiURL);
    }

    GetAllRegionList() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/GetRegions';
        return this.http.get(apiURL);
    }
    GetTerritories(userId) {
        const apiURL = this._appConstant.APIUrl + 'Lookup/GetTerritories/' + userId;
        return this.http.get(apiURL);
    }

    GetCustomerReassignmentRoles() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/CustomerReassignment/Roles';
        return this.http.get(apiURL);
    }
    GetCities(stateId) {
        const apiURL = this._appConstant.APIUrl + 'Lookup/Cities/' + stateId;
        return this.http.get(apiURL);
    }
    GetStates() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/States';
        return this.http.get(apiURL);
    }
    GetAllUsers() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/GetUsers';
        return this.http.get(apiURL);
    }
    //for add team & remove team dropdown
    GetCustReassignTerritoriesByRoleIds(roleIds) {
        const apiURL = this._appConstant.APIUrl + 'Lookup/CustomerReassignment/Territories/' + roleIds;
        return this.http.get(apiURL);
    }
    //for update territory dropdown
    GetCustReassignTeamByRoleIds(roleIds) {
        const apiURL = this._appConstant.APIUrl + 'Lookup/CustomerReassignment/Team/' + roleIds;
        return this.http.get(apiURL);
    }
    GetAllAVPs() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/AVPs';
        return this.http.get(apiURL);
    }
    GetAllBDs() {
        const apiURL = this._appConstant.APIUrl + 'Lookup/BDs';
        return this.http.get(apiURL);
    }
}
