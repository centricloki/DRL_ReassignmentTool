export class RegionModel {

    regionId: string;
    regioname: string;
    zoneId: string;
    zoneName: string;
    isActive: boolean;
    createdDate: Date;
    createdBy: string;
    updatedDate: Date;
    updatedBy: string;

    constructor() {
        this.regionId = '';
        this.regioname = '';
        this.zoneId = '';
        this.zoneName = '';
        this.isActive = true;
        this.createdDate = new Date();
        this.createdBy = '';
        this.updatedBy = '';
        this.updatedDate = new Date();
    }
}