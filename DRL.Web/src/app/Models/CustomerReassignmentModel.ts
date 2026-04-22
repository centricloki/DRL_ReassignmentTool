export class CustomerReassignmentModel {

    stateId: string;
    accountTypeId: number;
    cityId: string;
    customerId: string;
    exactMatch: boolean;
    partialMatch: boolean;
    includeDeleted: boolean;
    zipCode: string;
    address: string;
    customerName: string;
    customerNumber: string;
    cityName: string;
    stateName: string;
    defaultTeam: string;
    defaultTeams: string;
    isDeleted: boolean;
    accountType: string;
    keyAccount:string;
    insideSales:string;
    broker:string;
    parentNumber:string;

    constructor() {
        this.stateId = '';
        this.accountTypeId = 0;
        this.cityId = '';
        this.customerId = '';
        this.exactMatch = true;
        this.partialMatch = false;
        this.includeDeleted = false;
        this.zipCode = '';
        this.address = '';
        this.customerName = '';
        this.customerNumber = '';
        this.cityName = '';
        this.stateName = '';
        this.defaultTeam = '';
        this.defaultTeams = '';
        this.isDeleted = false;
        this.accountType = '';
        this.keyAccount='';
        this.insideSales='';
        this.broker='';
        this.parentNumber = undefined;
    }
}