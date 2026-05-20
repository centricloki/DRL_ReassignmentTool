import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BdService } from '../bd.service';
import { BDModel } from 'src/app/Models/BDModel';
import { ToastHelper } from 'src/app/helpers/toast.helper';
import { AppConstant } from 'src/app/app.constants';
import { ResponseParser } from 'src/app/helpers/response-parser.helper';
import { CommonService } from 'src/app/services/common.service';
import { TeamModel } from 'src/app/Models/TeamModel';
import { UsersService } from '../../user-management/users.service';
import { Observable, Subject } from 'rxjs';
import { map, startWith, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-manage-bd',
  templateUrl: './manage-bd.component.html',
  styleUrls: ['./manage-bd.component.css']
})
export class ManageBdComponent implements OnInit, OnDestroy {
  bdForm: FormGroup;
  isEditMode = false;
  bdId: number | null = null;
  pageTitle = 'Add New BD';

  TeamList: Array<TeamModel> = [];
  myItems: TeamModel[] = [];
  teamModel = new TeamModel();
  teamSearchControl = new FormControl('');
  filteredTeamList: Observable<any[]>;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private bdService: BdService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastHelper,
    private _appConstantService: AppConstant,
    private _commonLookupData: CommonService,
    private usersService: UsersService
  ) {
    this.bdForm = this.fb.group({
      bdid: [0],
      bdName: ['', [Validators.required, Validators.maxLength(250)]],
      approver: [false],
      isActive: [true]
    });
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit(): void {
    this.bdId = this._appConstantService.bdId ? Number.parseInt(this._appConstantService.bdId) : 0;

    this.getAllTerritories();

    if (this.bdId === 0 || isNaN(this.bdId)) {
      this.isEditMode = false;
      this.pageTitle = 'Create BDM';
    }
    else {
      this.isEditMode = true;
      this.pageTitle = 'Edit BDM';
      this.loadBdDetails(this.bdId);
    }

    this.filteredTeamList = this.teamSearchControl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterTeams(value || ''))
    );
  }

  private filterTeams(value: string): any[] {
    if (!this.TeamList || !Array.isArray(this.TeamList)) {
      return [];
    }
    if (!value) {
      return this.TeamList;
    }
    const filterValue = value.toLowerCase();
    return this.TeamList.filter(team => {
      if (!team || !team.name) {
        return false;
      }
      return team.name.toLowerCase().includes(filterValue);
    });
  }

  getAllTerritories(): void {
    this.usersService.GetAllTerritories().pipe(takeUntil(this.unsubscribe$)).subscribe(res => {
      var data = this._commonLookupData.parseData(res);
      this.TeamList = (data.data || []) as TeamModel[];
      this.teamSearchControl.updateValueAndValidity();
    });
  }

  loadBdDetails(bdId: number) {
    this.bdService.GetBDDetails(bdId).subscribe(
      (response: any) => {
        const data = ResponseParser.parseLegacyResponse(response);
        if (data && data.isSuccess) {
          const bdData = data.data;
          this.bdForm.patchValue({
            bdid: bdData.bdid,
            bdName: bdData.bdName,
            approver: bdData.approver,
            isActive: bdData.isActive
          });
          this.myItems = bdData.teams || [];
        } else {
          this.toastr.error('Error', 'Failed to load BD details');
          this.goBack();
        }
      },
      error => {
        this.toastr.error('Error', 'Failed to load BD details');
        this.goBack();
      }
    );
  }

  addUserToTeam() {
    if (this.teamModel.teamId && this.teamModel.teamId != '') {
      if (this.myItems.find(x => x.teamId == this.teamModel.teamId)) {
        this.toastr.error('Error', "Team already exist");
      }
      else {
        const team = this.TeamList.find(x => x.teamId == this.teamModel.teamId);
        this.teamModel.createdBy = "0";
        this.teamModel.createdDate = new Date();
        this.teamModel.updatedBy = "0";
        this.teamModel.updateDate = new Date();
        this.teamModel.name = team.name;
        this.myItems.push(this.teamModel);
      }
      this.teamModel = new TeamModel();
    }
  }

  deleteTeamDetail(i) {
    this._commonLookupData.confirmDialog('Are you sure you want to delete this team?', (result: any) => {
      if (result) {
        this.myItems.splice(i, 1);
      }
      this.teamModel = new TeamModel();
    });
  }

  onSubmit() {
    if (this.bdForm.valid) {
      const bdModel: BDModel = this.bdForm.value;
      bdModel.teams = this.myItems;

      this.bdService.manageBD(bdModel).subscribe(
        (response: any) => {
          const data = ResponseParser.parseLegacyResponse(response);
          if (data && data.isSuccess) {
            const message = this.isEditMode ? 'BDM updated successfully' : 'BDM added successfully';
            this.toastr.success('Success', message);
            this.goBack();
          } else {
            this.toastr.error('Error', data.message || 'Failed to save BDM');
          }
        },
        error => {
          this.toastr.error('Error', 'Failed to save BD');
        }
      );
    } else {
      this.toastr.error('Error', 'Please fill in all required fields');
    }
  }

  goBack() {
    this.router.navigate(['/bd']);
  }

  get f() {
    return this.bdForm.controls;
  }
}
