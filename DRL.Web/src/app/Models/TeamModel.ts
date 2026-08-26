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
    bdid: string; // Reverted back to bdid to maintain compatibility
    teamStatusId: string;

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
        this.bdid = ''; // Initialize the bdid property
        this.teamStatusId = '';
    }
}