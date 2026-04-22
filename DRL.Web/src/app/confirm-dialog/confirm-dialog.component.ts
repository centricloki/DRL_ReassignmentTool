import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit {
  confirmText: string;
  showCommonPopup: boolean;
  showOnlyConfirmButton: boolean;
  constructor(private dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) _data) {
    this.showCommonPopup = _data.showCommonPopup
    this.showOnlyConfirmButton = _data.showOnlyConfirmButton;
    this.confirmText = _data.message;
  };
  
  ngOnInit() {
  };

  close() {
    this.dialogRef.close();
  };

}
