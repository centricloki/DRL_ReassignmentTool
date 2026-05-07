import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ZoneService } from '../zone.service';
import { ZoneModel } from 'src/app/Models/ZoneModel';
import { ToastHelper } from 'src/app/helpers/toast.helper';
import { AppConstant } from 'src/app/app.constants';
import { ResponseParser } from 'src/app/helpers/response-parser.helper';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'app-manage-zone',
  templateUrl: './manage-zone.component.html',
  styleUrls: ['./manage-zone.component.css']
})
export class ManageZoneComponent implements OnInit {
  zoneForm: FormGroup;
  isEditMode = false;
  zoneId: number | null = null;
  pageTitle = 'Add New Zone';
  avpList: any[] = [];

  constructor(
    private fb: FormBuilder,
    private zoneService: ZoneService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastHelper,
    private _appConstantService: AppConstant,
    private _commonLookupData: CommonService

  ) {
    this.zoneForm = this.fb.group({
      zoneId: [0],
      zoneName: ['', [Validators.required, Validators.maxLength(100)]],
      avpid: [null],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.GetAllAVPs();
    this.zoneId = this._appConstantService.zoneId ? Number.parseInt(this._appConstantService.zoneId) : 0;
    if (this.zoneId === 0) {
      this.isEditMode = false;
      this.pageTitle = 'Create Zone';
    }
    else {
      this.isEditMode = true;
      this.pageTitle = 'Edit Zone';
      this.loadZoneDetails(this.zoneId);
    }
  }

  loadZoneDetails(zoneId: number) {
    this.zoneService.GetZoneDetails(zoneId).subscribe(
      (response: any) => {
        const data = ResponseParser.parseLegacyResponse(response);
        if (data && data.isSuccess) {
          const zoneData = data.data;
          this.zoneForm.patchValue({
            zoneId: zoneData.zoneId,
            zoneName: zoneData.zoneName,
            avpid: zoneData.avpid,
            isActive: zoneData.isActive
          });
        } else {
          this.toastr.error('Error', 'Failed to load zone details');
          this.goBack();
        }
      },
      error => {
        this.toastr.error('Error', 'Failed to load zone details');
        this.goBack();
      }
    );
  }

  onSubmit() {
    if (this.zoneForm.valid) {
      const zoneModel: ZoneModel = this.zoneForm.value;
      // Set default values
      zoneModel.sugarZoneId = '';
      zoneModel.importedFrom = 0;
      zoneModel.isDeleted = false;
      zoneModel.updateDate = new Date();

      this.zoneService.manageZone(zoneModel).subscribe(
        (response: any) => {
          const data = ResponseParser.parseLegacyResponse(response);
          if (data && data.isSuccess) {
            const message = this.isEditMode ? 'Zone updated successfully' : 'Zone added successfully';
            this.toastr.success('Success', message);
            this.goBack();
          } else {
            this.toastr.error('Error', data.message || 'Failed to save zone');
          }
        },
        error => {
          this.toastr.error('Error', 'Failed to save zone');
        }
      );
    } else {
      this.toastr.error('Error', 'Please fill in all required fields');
    }
  }

  GetAllAVPs() {
    this._commonLookupData.GetAllAVPs().subscribe(response => {
      var data = this._commonLookupData.parseData(response);
      this.avpList = data.data;
    });
  }

  goBack() {
    this.router.navigate(['/zones']);
  }

  get f() {
    return this.zoneForm.controls;
  }
}
