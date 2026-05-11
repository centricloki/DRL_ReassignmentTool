import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppConstant } from 'src/app/app.constants';
declare var $: any;

@Component({
  selector: 'app-sitelayout',
  templateUrl: './sitelayout.component.html',
  styleUrls: ['./sitelayout.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SitelayoutComponent implements OnInit {
  activeItemCode = 'USERS';
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
      let activeURL = false;
      if (this._appConstant.groupValue.toLowerCase() == 'rpb sales admin') {
        if (item.code == 'CUSTOMERS') {
          this.activeItemCode = 'CUSTOMERS';
          activeURL = true;
        }
      }
      else if (this._appConstant.groupValue.toLowerCase() == 'drl it') {
        if (item.code == 'USERS') {
          this.activeItemCode = 'USERS';
          activeURL = true;
        }
      }
      else {
        if (item.code == 'DASHBOARD') {
          this.activeItemCode = 'DASHBOARD';
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

  clear(type: string) {
    this.activeItemCode = type;
    switch (type) {
      case 'ROLES': this._appConstant.roleId = ''; break;
      case 'USERS': this._appConstant.userId = ''; break;
      case 'TEAMS': this._appConstant.teamId = ''; break;
      case 'REGIONS': this._appConstant.regionId = ''; break;
      case 'ZONES': this._appConstant.zoneId = ''; break;
      case 'DASHBOARD': {
        this._appConstant.roleId = '';
        this._appConstant.userId = '';
        this._appConstant.teamId = '';
        this._appConstant.regionId = '';
        this._appConstant.zoneId = '';
        break;
      }
    }
  }
}
