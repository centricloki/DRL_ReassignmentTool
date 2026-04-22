import { Component, OnInit } from '@angular/core';
import { UserModel } from '../Models/UserModel';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { CommonService } from '../services/common.service';
import { AppConstant } from '../app.constants';
import { ToasterService } from 'angular2-toaster';
declare var $: any;

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  SugarCRMUser = new UserModel();

  constructor(private _router: Router) {
    $('.btnLi').removeClass('active');
    $('#dashboardLink').addClass('active');
  }

  ngOnInit() {
    this.showHiddenDivs();
  }

  showHiddenDivs() {
    setTimeout(function () {
      $('#divheader').show();
      $('#divfooter').show();
      $('#navList').show();
    }, 100);
  }
}
