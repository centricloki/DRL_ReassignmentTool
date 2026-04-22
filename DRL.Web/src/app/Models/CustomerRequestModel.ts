export class CustomerRequestModel {
    page: number;
    pageSize: number;
    userId: number;
    territoryId: number;
    address: string;
    customerName: string;
    customerMatch: boolean;
    accountType: number;
    includeDeleted: boolean;
    sortExpression: string;
    city: string;
    state: string;
    zipCode: string;
    parentNumber:string;
    teamId: number;
    constructor() {
        this.page = 0;
        this.pageSize = 0;
        this.userId = 0;
        this.territoryId = 0;
        this.address = '';
        this.customerName = '';
        this.customerMatch = false;
        this.accountType = 0;
        this.includeDeleted = false;
        this.sortExpression = '';
        this.city = '';
        this.state = '';
        this.zipCode = '';
        this.parentNumber = undefined;
        this.teamId = 0;
    }
}