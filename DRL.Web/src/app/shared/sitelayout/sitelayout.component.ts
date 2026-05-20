import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AppConstant } from 'src/app/app.constants';
@Component({
  selector: 'app-sitelayout',
  templateUrl: './sitelayout.component.html',
  styleUrls: ['./sitelayout.component.css'],
})
export class SitelayoutComponent implements OnInit, OnDestroy {
  navItems = [
    { label: 'Tool', code: 'DASHBOARD', route: '/dashboard', icon: 'icon-01', id: 'dashboardLink', action: 'default' },
    { label: 'Users', code: 'USERS', route: '/users', icon: 'icon-08', id: 'lnkUser', action: 'user' },
    { label: 'Roles', code: 'ROLES', route: '/roles', icon: 'icon-05', id: 'lnkRole', action: 'role' },
    { label: 'Teams', code: 'TEAMS', route: '/teams', icon: 'icon-09', id: 'lnkTeam', action: 'team' },
    { label: 'Regions', code: 'REGIONS', route: '/regions', icon: 'icon-05', id: 'lnkRegion', action: 'region' },
    { label: 'Zones', code: 'ZONES', route: '/zones', icon: 'icon-05', id: 'lnkZone', action: 'zone' },
    { label: 'BDS', code: 'BDS', route: '/bd', icon: 'icon-05', id: 'lnkBD', action: 'bd' },
    { label: 'User Mapping', code: 'USER_REASSIGNMENT', route: '/customers/user-reassignment', icon: 'icon-06', id: 'lnkuserreassignment', action: 'userreassignment' },
    { label: 'Customers', code: 'CUSTOMERS', route: '/customers', icon: 'icon-09', id: 'lnkCustomer', action: 'customerreassignment' },
    { label: 'Action History', code: 'ACTION_HISTORY', route: '/customers/action-history', icon: 'icon-12 icon-custom', id: 'lnkactionhistory', action: 'actionhistory' },
    { label: 'Customer Import', code: 'CUSTOMER_IMPORT', route: '/customers/customer-import', icon: 'icon-11 icon-custom', id: 'lnkCustomerImport', action: 'customerimport' },
    { label: 'Brand Style', code: 'BRAND_STYLE', route: '/customers/brand-style', icon: 'icon-10 icon-custom', id: 'lnkBrandStyle', action: 'brandstyle' },
    { label: 'User Report', code: 'USER_REPORT', route: '/customers/user-report', icon: 'icon-07', id: 'lnkUserReport', action: 'userreport' },
  ];
  navLinks: any[] = [];
  private unsubscribe$ = new Subject<void>();
  constructor(public _appConstant: AppConstant, private _router: Router) { }
  ngOnInit() {
    this._appConstant.userPermissions$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(permissions => {
        this.visibleNavItems();
      });
  }

  // Getter to filter visible items and mark the 2nd one
  visibleNavItems() {
    const visible = this.navItems.filter(item =>
      this._appConstant.hasLinkAccess(item.code)
    );

    const isSameLastSegment = function (a: string, b: string): boolean {
      const getLast = function (path: string): string {
        const parts = path.replace(/\/+$/, '').split('/').filter(Boolean);
        return parts.length > 0 ? parts[parts.length - 1] : '';
      };

      return getLast(a) === getLast(b);
    };

    //   let isActiveURL = false;
    //   this.navLinks = visible.map((item, index) => ({
    //     ...item,
    //     isActive: (() => {
    //       if (this._appConstant.landingPage == '' || this._appConstant.landingPage == '/') {
    //         isActiveURL = item.route == '/dashboard';
    //         return isActiveURL;
    //       }
    //       else if (isSameLastSegment(item.route, this._appConstant.landingPage) && !isActiveURL) {
    //         this._appConstant.landingPage = item.route;
    //         isActiveURL = true;
    //         return isActiveURL;
    //       }
    //       return false;
    //     })()
    //   }));
    //   console.log("navLinks", this.navLinks);
    // }

    this.navLinks = visible.map((item, index) => ({
      ...item,
      isActive: (() => {
        if (this._appConstant.landingPage == '' || this._appConstant.landingPage == '/') {
          return item.route == '/dashboard';
        }
        else if (isSameLastSegment(item.route, this._appConstant.landingPage)) {
          this._appConstant.landingPage = item.route;
          return true;
        }
        return false;
      })()
    }));
    console.log("navLinks", this.navLinks);
  }




  clear(type: string) {
    this.navLinks.forEach(item => {
      if (item.code === type) {
        item.isActive = true;
      } else {
        item.isActive = false;
      }
    });
    this._appConstant.userId = '';
    this._appConstant.roleId = '';
    this._appConstant.teamId = '';
    this._appConstant.regionId = '';
    this._appConstant.teamName = '';
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
