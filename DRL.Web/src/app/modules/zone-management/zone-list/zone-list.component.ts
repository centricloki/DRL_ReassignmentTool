import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ZoneService } from '../zone.service';
import { ToastrService } from 'ngx-toastr';
import { ConfirmDialogComponent } from 'src/app/confirm-dialog/confirm-dialog.component';
import { WarningDialogComponent } from 'src/app/warning-dialog/warning-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-zone-list',
  templateUrl: './zone-list.component.html',
  styleUrls: ['./zone-list.component.css']
})
export class ZoneListComponent implements OnInit {

  zones: any[] = [];
  displayedColumns: string[] = ['zoneName', 'avpName', 'createdBy', 'createdDate', 'isActive', 'actions'];
  isLoading = false;

  constructor(
    private zoneService: ZoneService,
    private router: Router,
    private toastr: ToastrService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadZones();
  }

  loadZones() {
    this.isLoading = true;
    this.zoneService.GetAllZones().subscribe(
      (response: any) => {
        if (response.isSuccess) {
          this.zones = response.data || [];
        } else {
          this.toastr.error(response.message || 'Failed to load zones');
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.toastr.error('Failed to load zones');
      }
    );
  }

  editZone(zoneId: number) {
    this.router.navigate(['/zone-management/create'], { queryParams: { id: zoneId } });
  }

  deleteZone(zone: any) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Zone',
        message: `Are you sure you want to delete zone "${zone.zoneName}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const deleteRequest = {
          id: zone.zoneId,
          status: true
        };
        this.zoneService.DeleteZone(deleteRequest).subscribe(
          (response: any) => {
            if (response.isSuccess) {
              this.toastr.success('Zone deleted successfully');
              this.loadZones();
            } else {
              this.toastr.error(response.message || 'Failed to delete zone');
            }
          },
          error => {
            this.toastr.error('Failed to delete zone');
          }
        );
      }
    });
  }

  toggleZoneStatus(zone: any) {
    const newStatus = !zone.isActive;
    const statusText = newStatus ? 'activate' : 'deactivate';
    
    const dialogRef = this.dialog.open(WarningDialogComponent, {
      width: '400px',
      data: {
        title: `${statusText.charAt(0).toUpperCase() + statusText.slice(1)} Zone`,
        message: `Are you sure you want to ${statusText} zone "${zone.zoneName}"?`,
        confirmText: 'Yes',
        cancelText: 'No'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const statusRequest = {
          id: zone.zoneId,
          status: newStatus
        };
        this.zoneService.ManageZoneStatus(statusRequest).subscribe(
          (response: any) => {
            if (response.isSuccess) {
              this.toastr.success(`Zone ${statusText}d successfully`);
              this.loadZones();
            } else {
              this.toastr.error(response.message || `Failed to ${statusText} zone`);
            }
          },
          error => {
            this.toastr.error(`Failed to ${statusText} zone`);
          }
        );
      }
    });
  }

  addNewZone() {
    this.router.navigate(['/zone-management/create']);
  }
}