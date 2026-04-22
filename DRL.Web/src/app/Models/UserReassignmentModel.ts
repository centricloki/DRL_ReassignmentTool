export class UserReassignmentModel {

    userId: number;
    userName: string;
    name: string;
    role: string;
    defaultTeams: string;
    teams: string;
    checked: boolean;
    defaultTeamId: string;
    territoryId: number;
    keyAccount: string;
    insideSales: string;
    broker: string;
    teamId: number;

    constructor() {
        this.userId = 0;
        this.userName = '';
        this.name = '';
        this.role = '';
        this.defaultTeams = '';
        this.teams = '';
        this.checked = false;
        this.defaultTeamId = '';
        this.territoryId = 0;
        this.keyAccount = '';
        this.insideSales = '';
        this.broker = '';
        this.teamId = 0;
    }
}
export class TeamReassignmentModel {

    TeamId: string;
    AddTeamId: number;
    RemoveTeamId: number;
    UpdateTeamId: number;

    constructor() {
        this.TeamId = '';
        this.AddTeamId = 0;
        this.RemoveTeamId = 0
        this.UpdateTeamId = 0;
    }
}

export class LookUpModel {
    recordId: string;
    value: string;

    constructor() {
        this.recordId = '';
        this.value = '';
    }

}