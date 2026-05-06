import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ZoneService } from '../zone.service';
import { ZoneModel } from 'src/app/Models/ZoneModel';
import { ToastrService } from 'ngx-toastr';

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

  constructor(
    private fb: FormBuilder,
    private zoneService: ZoneService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.zoneForm = this.fb.group({
      zoneId: [0],
      zoneName: ['', [Validators.required, Validators.maxLength(100)]],
      avpid: [null],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['id']) {
        this.zoneId = +params['id'];
        this.isEditMode = true;
        this.pageTitle = 'Edit Zone';
        this.loadZoneDetails(this.zoneId);
      }
    });
  }

  loadZoneDetails(zoneId: number) {
    this.zoneService.GetZoneDetails(zoneId).subscribe(
      (response: any) => {
        if (response.isSuccess && response.data) {
          const zoneData = response.data;
          this.zoneForm.patchValue({
            zoneId: zoneData.zoneId,
            zoneName: zoneData.zoneName,
            avpid: zoneData.avpid,
            isActive: zoneData.isActive
          });
        } else {
          this.toastr.error('Failed to load zone details');
          this.goBack();
        }
      },
      error => {
        this.toastr.error('Failed to load zone details');
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
          if (response.isSuccess) {
            const message = this.isEditMode ? 'Zone updated successfully' : 'Zone added successfully';
            this.toastr.success(message);
            this.goBack();
          } else {
            this.toastr.error(response.message || 'Failed to save zone');
          }
        },
        error => {
          this.toastr.error('Failed to save zone');
        }
      );
    } else {
      this.toastr.error('Please fill in all required fields');
    }
  }

  goBack() {
    this.router.navigate(['/zone-management']);
  }

  get f() {
    return this.zoneForm.controls;
  }
}