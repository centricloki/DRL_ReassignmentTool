export class TeamModel {

    teamId: string;
    name: string;
    description: string;
    isActive: boolean;
    createdDate: Date;
    createdBy: string;
    updateDate: Date;
    updatedBy: string;
    regionId: string;
    teamStatusId: string;
    bdid: number;

    constructor() {
        this.teamId = '';
        this.name = '';
        this.description = '';
        this.isActive = true;
        this.createdDate = new Date();
        this.createdBy = '';
        this.updatedBy = '';
        this.updateDate = new Date();
        this.regionId = '';
        this.teamStatusId = '';
    }
}    