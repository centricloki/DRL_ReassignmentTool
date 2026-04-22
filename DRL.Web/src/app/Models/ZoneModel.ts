export class ZoneModel {

    zoneId: number;
    zoneName: string;
    sugarZoneId: string;
    importedFrom: number;
    avpid: number;
    updateDate: Date;
    isActive: boolean;
    isDeleted: boolean;

    constructor() {
        this.zoneId = 0;
        this.zoneName = '';
        this.sugarZoneId = '';
        this.importedFrom = 0;
        this.isActive = true;
        this.isDeleted = true;
        this.updateDate = new Date();
    }
}    