import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppConstant } from 'src/app/app.constants';
declare var $: any;

@Component({
  selector: 'app-sitelayout',
  templateUrl: './sitelayout.component.html',
  styleUrls: ['./sitelayout.component.css']
})
export class SitelayoutComponent implements OnInit {
  navItems = [
    { label: 'Tool', code: 'DASHBOARD', route: '/dashboard', icon: 'icon-01', id: 'dashboardLink', action: 'default' },
    { label: 'Users', code: 'USERS', route: '/users', icon: 'icon-08', id: 'lnkUser', action: 'user' },
    { label: 'Roles', code: 'ROLES', route: '/roles', icon: 'icon-05', id: 'lnkRole', action: 'role' },
    { label: 'Teams', code: 'TEAMS', route: '/teams', icon: 'icon-09', id: 'lnkTeam', action: 'team' },
    { label: 'Regions', code: 'REGIONS', route: '/regions', icon: 'icon-05', id: 'lnkRegion', action: 'region' },
    { label: 'Zones', code: 'ZONES', route: '/zones', icon: 'icon-05', id: 'lnkZone', action: 'zone' },
    { label: 'User Mapping', code: 'USER_REASSIGNMENT', route: '/customers/user-reassignment', icon: 'icon-06', id: 'lnkuserreassignment', action: 'userreassignment' },
    { label: 'Customers', code: 'CUSTOMERS', route: '/customers', icon: 'icon-09', id: 'lnkCustomer', action: 'customerreassignment' },
    { label: 'Action History', code: 'ACTION_HISTORY', route: '/customers/action-history', icon: 'icon-12 icon-custom', id: 'lnkactionhistory', action: 'actionhistory' },
    { label: 'Customer Import', code: 'CUSTOMER_IMPORT', route: '/customers/customer-import', icon: 'icon-11 icon-custom', id: 'lnkCustomerImport', action: 'customerimport' },
    { label: 'Brand Style', code: 'BRAND_STYLE', route: '/customers/brand-style', icon: 'icon-10 icon-custom', id: 'lnkBrandStyle', action: 'brandstyle' },
    { label: 'User Report', code: 'USER_REPORT', route: '/customers/user-report', icon: 'icon-07', id: 'lnkUserReport', action: 'userreport' },
  ];
  navLinks: any[] = [];

  constructor(public _appConstant: AppConstant, private route: Router) { }
  ngOnInit() {
    this._appConstant.userPermissions$.subscribe(permissions => {
      this.visibleNavItems();
    });
  }

  // Getter to filter visible items and mark the 2nd one
  visibleNavItems() {
    const visible = this.navItems.filter(item =>
      this._appConstant.hasLinkAccess(item.code)
    );
    this.navLinks = visible.map((item, index) => {
      // debugger

      let activeURL = false;
      if (this._appConstant.groupValue.toLowerCase() == 'rpb sales admin') {
        if (item.code == 'CUSTOMERS') {
          activeURL = true;
        }
      }
      else if (this._appConstant.groupValue.toLowerCase() == 'drl it') {
        if (item.code == 'USERS') {
          activeURL = true;
        }
      }
      else {
        if (item.code == 'DASHBOARD') {
          activeURL = true;
        }
      }
      return ({
        ...item,
        isActive: activeURL //this.route.url.replace('/', '').includes(item.route.replace('/', ''))
      });
    });
    console.log("navLinks", this.navLinks);
  }

  clear(type) {
    console.log('type', type);
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
        $('#dashboardLink').addClass('active');
        break;
    }
  }
}
