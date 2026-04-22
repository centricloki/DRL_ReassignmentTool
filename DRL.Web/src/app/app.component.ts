import { Component, HostListener, } from '@angular/core';
import { Router } from '@angular/router';
import { ToasterService } from 'angular2-toaster';
import { AuthenticationService } from './services/authentication.service';
import { UserModel } from './Models/UserModel';
import { CommonService } from './services/common.service';
import { AppConstant } from './app.constants';
import { SessionTimeoutService } from './services/SessionTimeoutService';
declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isLoggedIn: boolean = false;
  SugarCRMUser = new UserModel();
  title = 'SugarCRM';
  constructor(private _router: Router, private _toasterService: ToasterService
    , private authenticationService: AuthenticationService
    , private _commonLookupData: CommonService
    , private _appConstant: AppConstant
    , private sessionTimeoutService: SessionTimeoutService
  ) {
  }

  @HostListener("window:onbeforeunload", ["$event"])
  clearLocalStorage(event) {
    localStorage.clear();
  }

  ngOnInit() {
    // Restore permissions from localStorage if available
    const savedUserName = localStorage.getItem('userName');
    const savedPermissions = localStorage.getItem('userPermissions');
    
    if (savedUserName && savedPermissions) {
      try {
        this._appConstant.userDisplayName = savedUserName;
        this._appConstant.userPermissions = JSON.parse(savedPermissions);
        this._appConstant.isAuthenticate = true;
        // Group value might be empty, so we still need to check or authenticate
      } catch (e) {
        // Clear invalid data
        localStorage.removeItem('userName');
        localStorage.removeItem('userPermissions');
      }
    }

    if (this._appConstant.groupValue == '') {
      this.authenticationService.Authentication(this.SugarCRMUser).subscribe(data => {
        if (data != null) {
          let body = this._commonLookupData.parseData(data);
          if (body.isSuccess && body.data != null) {
            var userData = JSON.parse(JSON.stringify(body.data));
            if (userData.userName != null) {
              this._appConstant.isAuthenticate = true;
              localStorage["userName"] = userData.userName;
              this._appConstant.groupValue = userData.userGroup;
              this._appConstant.userDisplayName = userData.userName;
              this._appConstant.userPermissions = userData.linkPermissions || [];
              // Persist permissions in localStorage
              localStorage["userPermissions"] = JSON.stringify(userData.linkPermissions || []);
              this.checkUserGroup();
            }
          }
        }
      },
        (err: any) => {
          if (err.status == 401) {
            //this._router.navigate(['/error']);
          }
          else {
            if (err._body != undefined && err._body != "" && typeof err._body === 'string') {
              var errMsg = JSON.parse(err._body)
              this._toasterService.pop('error', 'Error', errMsg.message);
              // this._router.navigate(['/error']);
            }
            else {
              // this._router.navigate(['/error']);
            }
          }
        }
      );
    }
    else {
      this.checkUserGroup();
    }
  }
  checkUserGroup() {
    const lastUrl = sessionStorage.getItem('lastUrl');

    if (this._appConstant.groupValue.toLowerCase() == 'rpb sales admin') {
      this._appConstant.isDRLIT = false;

      if (lastUrl) {
        sessionStorage.removeItem('lastUrl');
        this._router.navigate([lastUrl]);
      } else {
        this._router.navigate(['/customers']);
      }
    }
    else if (this._appConstant.groupValue.toLowerCase() == 'drl it') {
      this._appConstant.isDRLIT = true;
      console.log('lastUrl', lastUrl)
      if (lastUrl) {
        sessionStorage.removeItem('lastUrl');
        this._router.navigate([lastUrl]);
      } else {
        console.log('no lastUrl')
        this._router.navigate(['/users']);
      }
    }
    else {
      this._appConstant.isDRLIT = false;

      if (lastUrl) {
        sessionStorage.removeItem('lastUrl');
        this._router.navigate([lastUrl]);
      } else {
        this._router.navigate(['/dashboard']);
      }
    }
  }
}