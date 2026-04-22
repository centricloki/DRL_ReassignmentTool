import { LookupItemModel } from './LookupItemModel';
import { RoleModel } from './RoleModel';
import { TeamModel } from './TeamModel';
import { ZoneModel } from './ZoneModel';

export class UserModel {

    userId: string;
    userName: string;
    firstName: string;
    lastName: string;
    pin: string;
    managerId: string;
    roleId: string;
    teamID: string;
    isActive: boolean;
    createdDate: Date;
    createdBy: string;
    updatedDate: Date;
    updatedBy: string;
    userRole: string;
    name: string;
    password: string;
    domain: string;
    defaultTeamId: string;
    Role: RoleModel[];
    zoneId: string;
    regionId: string;
    team: TeamModel;
    teams: TeamModel[];
    statusId: string;
    territoryId:string;
    email:string;
    bdid:string;
    avpid:string;
    zones:ZoneModel[];

    constructor() {
        this.userId = '';
        this.userName = '';
        this.firstName = '';
        this.lastName = '';
        this.pin = '';
        this.managerId = '';
        this.roleId = '';
        this.teamID = '';
        this.isActive = true;
        this.createdDate = new Date();
        this.createdBy = '';
        this.updatedBy = '';
        this.userRole = '';
        this.updatedDate = new Date();
        this.name = '';
        this.password = '';
        this.domain = '';
        this.defaultTeamId = '';
        this.regionId = '';
        this.zoneId = '';
        this.statusId = '';
        this.teams = [];
        this.territoryId='';
        this.email='';
        this.zones = [];
    }
}

export class ENTRequestModel {
    id: number;
    status: boolean;

    constructor() {
        this.id = 0;
        this.status = false;
    }
}