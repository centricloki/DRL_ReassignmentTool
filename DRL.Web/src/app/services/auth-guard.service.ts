import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AppConstant } from '../app.constants';
import { AuthenticationService } from './authentication.service';
import { Observable, of } from 'rxjs';
import { catchError, map, switchMap, timeout } from 'rxjs/operators';
import { ResponseParser } from '../helpers/response-parser.helper';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(
    private _appConstant: AppConstant,
    private router: Router,
    private authenticationService: AuthenticationService
  ) { }

  canActivate(route: ActivatedRouteSnapshot): boolean | Observable<boolean> {
    // Store the attempted URL for redirect later
    sessionStorage.setItem('lastUrl', route.url.join('/'));

    // Check if user is authenticated
    if (!this._appConstant.isAuthenticate || !this._appConstant.groupValue) {
      // Try to restore from localStorage if available
      const savedUserName = localStorage.getItem('userName');
      const savedPermissions = localStorage.getItem('userPermissions');
      const savedGroupValue = localStorage.getItem('groupValue');

      if (savedUserName && savedPermissions) {
        try {
          this._appConstant.userDisplayName = savedUserName;
          this._appConstant.userPermissions = JSON.parse(savedPermissions);
          this._appConstant.groupValue = savedGroupValue || '';
          this._appConstant.isAuthenticate = true;
        } catch (e) {
          // Clear invalid data and force re-authentication
          localStorage.removeItem('userName');
          localStorage.removeItem('userPermissions');
          localStorage.removeItem('groupValue');
          return this.handleAuthentication();
        }
      } else {
        return this.handleAuthentication();
      }
    }

    let requiredLinkCode: string = '';

    // Check current route first
    if (route.data && route.data['linkCode']) {
      requiredLinkCode = route.data['linkCode'];
    }

    // Traverse child routes if linkCode not found
    if (!requiredLinkCode) {
      let currentRoute: ActivatedRouteSnapshot | null = route.firstChild;
      while (currentRoute) {
        if (currentRoute.data && currentRoute.data['linkCode']) {
          requiredLinkCode = currentRoute.data['linkCode'];
          break;
        }
        currentRoute = currentRoute.firstChild;
      }
    }

    // If route requires no specific link code (e.g. dashboard), allow access
    if (!requiredLinkCode) {
      return true;
    }

    const hasAccess = this._appConstant.hasLinkAccess(requiredLinkCode);

    if (!hasAccess) {
      sessionStorage.removeItem('lastUrl');
      // Redirect to error if access denied
      this.router.navigate(['/error']);
      return false;
    }

    return true;
  }

  private handleAuthentication(): Observable<boolean> {
    // Clear any existing session data
    this.clearSessionData();

    // Attempt to authenticate silently
    return this.authenticationService.Authentication({} as any).pipe(
      timeout(3 * 60 * 1000), // 3 minutes timeout to prevent hanging
      map(data => {
        if (data != null) {
          const body = ResponseParser.parseLegacyResponse(data);
          if (body && body.isSuccess && body.data) {
            const userData = body.data;
            const userName = userData.userName || userData.UserName;
            if (userName) {
              this._appConstant.isAuthenticate = true;
              localStorage["userName"] = userName;
              localStorage["groupValue"] = userData.userGroup || userData.UserGroup || '';
              this._appConstant.groupValue = userData.userGroup || userData.UserGroup || '';
              this._appConstant.userDisplayName = userName;
              this._appConstant.userPermissions = userData.linkPermissions || userData.LinkPermissions || [];
              localStorage["userPermissions"] = JSON.stringify(this._appConstant.userPermissions);

              // Restore last URL and navigate if not dashboard/error
              const lastUrl = sessionStorage.getItem('lastUrl');
              if (lastUrl && lastUrl.trim() !== '' && lastUrl !== 'dashboard' && lastUrl !== '/dashboard' && lastUrl !== 'error' && lastUrl !== '/error') {
                sessionStorage.removeItem('lastUrl');
                this.router.navigate([lastUrl.startsWith('/') ? lastUrl : '/' + lastUrl]);
              }
              return true;
            }
          }
        }
        this.router.navigate(['/error']);
        return false;
      }),
      catchError((error) => {
        console.error('Authentication failed', error);
        this.router.navigate(['/error']);
        return of(false);
      })
    );
  }

  private parseAuthResponse(data: any): any {
    // Handle different response formats
    if (typeof data === 'string') {
      return JSON.parse(data);
    }
    return data;
  }

  private clearSessionData(): void {
    this._appConstant.isAuthenticate = false;
    this._appConstant.groupValue = '';
    this._appConstant.userDisplayName = '';
    this._appConstant.userPermissions = [];
  }
}