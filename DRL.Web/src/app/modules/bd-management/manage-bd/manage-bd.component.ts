import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BdService } from '../bd.service';
import { BDModel } from 'src/app/Models/BDModel';
import { ToastHelper } from 'src/app/helpers/toast.helper';
import { AppConstant } from 'src/app/app.constants';
import { ResponseParser } from 'src/app/helpers/response-parser.helper';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'app-manage-bd',
  templateUrl: './manage-bd.component.html',
  styleUrls: ['./manage-bd.component.css']
})
export class ManageBdComponent implements OnInit {
  bdForm: FormGroup;
  isEditMode = false;
  bdId: number | null = null;
  pageTitle = 'Add New Business Division';

  constructor(
    private fb: FormBuilder,
    private bdService: BdService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastHelper,
    private _appConstantService: AppConstant,
    private _commonLookupData: CommonService
  ) {
    this.bdForm = this.fb.group({
      bdid: [0],
      bdName: ['', [Validators.required, Validators.maxLength(250)]],
      approver: [false],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.bdId = this._appConstantService.bdId ? Number.parseInt(this._appConstantService.bdId) : 0;
    if (this.bdId === 0 || isNaN(this.bdId)) {
      this.isEditMode = false;
      this.pageTitle = 'Create Business Division';
    }
    else {
      this.isEditMode = true;
      this.pageTitle = 'Edit Business Division';
      this.loadBdDetails(this.bdId);
    }
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

  onSubmit() {
    if (this.bdForm.valid) {
      const bdModel: BDModel = this.bdForm.value;
      
      this.bdService.manageBD(bdModel).subscribe(
        (response: any) => {
          const data = ResponseParser.parseLegacyResponse(response);
          if (data && data.isSuccess) {
            const message = this.isEditMode ? 'Business Division updated successfully' : 'Business Division added successfully';
            this.toastr.success('Success', message);
            this.goBack();
          } else {
            this.toastr.error('Error', data.message || 'Failed to save Business Division');
          }
        },
        error => {
          this.toastr.error('Error', 'Failed to save Business Division');
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
