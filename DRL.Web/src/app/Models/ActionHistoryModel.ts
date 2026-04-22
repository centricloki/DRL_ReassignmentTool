export class ActionHistoryModel
{
    module:string;
    operation:string;
    accountName:string;
    customerNumber:string;
    team:string;
    updatedBy:string;
    updatedDate:Date;
    updateDatestring:string;

    constructor() {
        this.module='';
        this.operation='';
        this.accountName='';
        this.customerNumber='';
        this.team='';
        this.updatedBy='';
        this.updatedDate=new Date();
        this.updateDatestring='';
    }
}