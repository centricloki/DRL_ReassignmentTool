export class RoleModel {

    roleId: string;
    roleName: string;
    description: string;
    isActive: boolean;
    createdDate: Date;
    createdBy: string;
    updatedDate: Date;
    updatedBy: string;

    constructor() {
        this.roleId = '';
        this.roleName = '';
        this.description = '';
        this.isActive = true;
        this.createdDate = new Date();
        this.createdBy = '';
        this.updatedBy = '';
        this.updatedDate = new Date();
       
    }
}    