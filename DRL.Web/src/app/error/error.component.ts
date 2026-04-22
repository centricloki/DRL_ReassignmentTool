import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
declare var $: any;

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {

  constructor(private _router: Router) {
  }

  ngOnInit() {
    $('#divfooter').hide();
    $('#divheader').hide();
    $('#navList').hide();
  }
  redirecttoHome() {
    $('#divfooter').show();
    $('#divheader').show();
    $('#navList').show();
    this._router.navigate(['/dashboard']);
  }
}
