import { ChangeDetectionStrategy, Component, HostListener, OnInit } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { ToasterService } from 'angular2-toaster';
import { AuthenticationService } from './services/authentication.service';
import { UserModel } from './Models/UserModel';
import { CommonService } from './services/common.service';
import { AppConstant } from './app.constants';
import { SessionTimeoutService } from './services/SessionTimeoutService';
import { HttpService } from './services/Httpservice';
import { take } from 'rxjs/operators';
import { ResponseParser } from './helpers/response-parser.helper';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  isLoggedIn: boolean = false;
  isAuthenticating: boolean = true;
  SugarCRMUser = new UserModel();
  title = 'SugarCRM';
  constructor(private _router: Router, private _toasterService: ToasterService
    , private authenticationService: AuthenticationService
    , private _commonLookupData: CommonService
    , public _appConstant: AppConstant
    , private sessionTimeoutService: SessionTimeoutService
  ) {
  }

  // @HostListener("window:onbeforeunload", ["$event"])
  // clearLocalStorage(event) {
  //   localStorage.clear();
  // }

  ngOnInit() {
    // Show loader immediately on initial load while authenticating
    HttpService.showLoader();

    // Reset loader on route navigation so in-flight requests from a previous page never freeze the new page,
    // but do not hide the loader while initial authentication is in progress.
    this._router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        if (!this.isAuthenticating) {
          HttpService.resetLoader();
        }
      }
    });

    const name = localStorage.getItem('userName');
    const perms = localStorage.getItem('userPermissions');
    const group = localStorage.getItem('groupValue');
    if (name && perms) {
      try {
        this._appConstant.userDisplayName = name;
        this._appConstant.userPermissions = JSON.parse(perms);
        this._appConstant.groupValue = group || '';
        this._appConstant.isAuthenticate = true;
      } catch (e) {
        localStorage.removeItem('userName');
        localStorage.removeItem('userPermissions');
        localStorage.removeItem('groupValue');
      }
    }

    // fetch Windows identity once
    this.authenticationService.Authentication(this.SugarCRMUser).pipe(take(1)).subscribe({
      next: body => {
        const parsedData = ResponseParser.parseLegacyResponse(body);
        if (!parsedData || !parsedData.isSuccess || !parsedData.data) {
          this.handleAuthFailure();
          return;
        }
        const u = parsedData.data;
        const userName = u.userName || u.UserName;
        if (!userName) {
          this.handleAuthFailure();
          return;
        }
        this._appConstant.isAuthenticate = true;
        this._appConstant.userDisplayName = userName;
        this._appConstant.groupValue = u.userGroup || u.UserGroup || '';
        this._appConstant.landingPage = u.landingPage || u.LandingPage || '/dashboard';
        this.lastVisitedRoute();
        if (u.linkPermissions || u.LinkPermissions) {
          this._appConstant.userPermissions = u.linkPermissions || u.LinkPermissions;
        }
        else {
          this._appConstant.userPermissions = [];
        }
        localStorage.setItem('userName', userName);
        localStorage.setItem('groupValue', this._appConstant.groupValue);
        localStorage.setItem('userPermissions', JSON.stringify(this._appConstant.userPermissions));

        this.isAuthenticating = false;
        HttpService.resetLoader();
        this._router.navigate([this._appConstant.landingPage]);
      },
      error: () => {
        this.handleAuthFailure();
      }
    });
  }

  private handleAuthFailure() {
    this.isAuthenticating = false;
    this._appConstant.isAuthenticate = false;
    this._appConstant.groupValue = '';
    this._appConstant.userDisplayName = '';
    this._appConstant.userPermissions = [];
    localStorage.removeItem('userName');
    localStorage.removeItem('groupValue');
    localStorage.removeItem('userPermissions');
    HttpService.resetLoader();
    this._router.navigate(['/error']);
  }

  lastVisitedRoute() {
    const lastUrl = sessionStorage.getItem('lastUrl');
    console.log('lastVisitedRoute>>lastUrl', lastUrl);
    if (lastUrl && lastUrl.trim() !== '' && lastUrl !== 'dashboard' && lastUrl !== '/dashboard' && lastUrl !== 'error' && lastUrl !== '/error') {
      this._appConstant.landingPage = lastUrl.startsWith('/') ? lastUrl : '/' + lastUrl;
      sessionStorage.removeItem('lastUrl');
    } else {
      sessionStorage.removeItem('lastUrl');
    }
  }
}