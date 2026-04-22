import { Component, OnInit } from '@angular/core';
import { AppConstant } from 'src/app/app.constants';
declare var $: any;

@Component({
  selector: 'app-sitelayout',
  templateUrl: './sitelayout.component.html',
  styleUrls: ['./sitelayout.component.css']
})
export class SitelayoutComponent implements OnInit {

  constructor(public _appConstant: AppConstant) { }

  ngOnInit() {

  }


  clear(type) {
    $('a.btnLi').removeClass('active');
   
    switch (type) {
      case 'role':
        this._appConstant.roleId = '';
        $('#lnkRole').addClass('active');
        break;
      case 'user':
        this._appConstant.userId = '';
        $('#lnkUser').addClass('active');
        break;
      case 'team':
        this._appConstant.teamId = '';
        $('#lnkTeam').addClass('active');
        break;
      case 'region':
        this._appConstant.regionId = '';
        $('#lnkRegion').addClass('active');
        break;
      case 'userreassignment':
        $('#lnkuserreassignment').addClass('active');
        break;
      case 'customerreassignment':
        $('#lnkCustomer').addClass('active');
        break;
      case 'actionhistory':
        $('#lnkactionhistory').addClass('active');
        break;
      case 'customerimport':
        $('#lnkCustomerImport').addClass('active');
        break;
      case 'brandstyle':
        $('#lnkBrandStyle').addClass('active');
        break;
      case 'userreport':
        $('#lnkUserReport').addClass('active');
        break;
      case 'default':
        break;
    }
  }
}
