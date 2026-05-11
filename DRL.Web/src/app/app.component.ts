import { ChangeDetectionStrategy, Component, HostListener, } from '@angular/core';
import { Router } from '@angular/router';
import { ToasterService } from 'angular2-toaster';
import { AuthenticationService } from './services/authentication.service';
import { UserModel } from './Models/UserModel';
import { CommonService } from './services/common.service';
import { AppConstant } from './app.constants';
import { SessionTimeoutService } from './services/SessionTimeoutService';
import { take } from 'rxjs/operators';
import { ResponseParser } from './helpers/response-parser.helper';

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

  // @HostListener("window:onbeforeunload", ["$event"])
  // clearLocalStorage(event) {
  //   localStorage.clear();
  // }

  ngOnInit() {
    const name = localStorage.getItem('userName');
    const perms = localStorage.getItem('userPermissions');
    if (name && perms) {
      this._appConstant.userDisplayName = name;
      this._appConstant.userPermissions = JSON.parse(perms);
      this._appConstant.isAuthenticate = true;
    }

    // fetch Windows identity once
    this.authenticationService.Authentication(this.SugarCRMUser).pipe(take(1)).subscribe({
      next: body => {
        const parsedData = ResponseParser.parseLegacyResponse(body);
        if (parsedData && !parsedData.isSuccess) return;
        const u = parsedData.data; // no JSON.parse(JSON.stringify())
        this._appConstant.isAuthenticate = true;
        this._appConstant.userDisplayName = u.userName;
        this._appConstant.groupValue = u.userGroup;
        this._appConstant.landingPage = u.landingPage;
        if (u.linkPermissions) {
          this._appConstant.userPermissions = u.linkPermissions;
        }
        else {
          this._appConstant.userPermissions = [];
        }
        localStorage.setItem('userName', u.userName);
        this.lastVisitedRoute();
        localStorage.setItem('userPermissions', JSON.stringify(this._appConstant.userPermissions));
        this._router.navigate([this._appConstant.landingPage]);
      }
    });
  }

  lastVisitedRoute() {
    const lastUrl = sessionStorage.getItem('lastUrl');
    console.log('lastVisitedRoute>>lastUrl', lastUrl);
    if (lastUrl) {
      this._appConstant.landingPage = lastUrl;
      sessionStorage.removeItem('lastUrl');
    }
  }
}