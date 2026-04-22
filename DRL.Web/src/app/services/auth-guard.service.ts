import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AppConstant } from '../app.constants';
import { AuthenticationService } from './authentication.service';
import { Observable, of } from 'rxjs';
import { catchError, map, switchMap, timeout } from 'rxjs/operators';

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
      
      if (savedUserName && savedPermissions) {
        try {
          this._appConstant.userDisplayName = savedUserName;
          this._appConstant.userPermissions = JSON.parse(savedPermissions);
          this._appConstant.isAuthenticate = true;
          // Group value might need to be inferred or we need to re-authenticate
          // For now, proceed with authentication check
        } catch (e) {
          // Clear invalid data and force re-authentication
          localStorage.removeItem('userName');
          localStorage.removeItem('userPermissions');
          return this.handleAuthentication();
        }
      } else {
        return this.handleAuthentication();
      }
    }

    // ✅ FIXED: Traditional null-safe access (no optional chaining)
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

    if (!requiredLinkCode) {
      // No permission check required
      if (this._appConstant.groupValue.toLowerCase() == "drl it") {
        return true;
      }
      else if (this._appConstant.groupValue.toLowerCase() == "rpb sales admin") {
        return false;
      }
    }

    const hasAccess = this._appConstant.hasLinkAccess(requiredLinkCode);

    if (!hasAccess) {
      sessionStorage.removeItem('lastUrl');
      // Redirect to dashboard if access denied
      this.router.navigate(['/dashboard']);
      return false;
    }

    return true;
  }

  private handleAuthentication(): Observable<boolean> {
    // Clear any existing session data
    this.clearSessionData();
    
    // Attempt to authenticate silently
    return this.authenticationService.Authentication({} as any).pipe(
      timeout(5000), // 5 second timeout to prevent hanging
      map(data => {
        if (data != null) {
          // Parse authentication response
          let body: any;
          try {
            body = this.parseAuthResponse(data);
          } catch (e) {
            console.error('Failed to parse auth response', e);
            this.router.navigate(['/login']);
            return false;
          }
          
          if (body.isSuccess && body.data != null) {
            var userData = JSON.parse(JSON.stringify(body.data));
            if (userData.userName != null) {
              this._appConstant.isAuthenticate = true;
              localStorage["userName"] = userData.userName;
              this._appConstant.groupValue = userData.userGroup;
              this._appConstant.userDisplayName = userData.userName;
              this._appConstant.userPermissions = userData.linkPermissions || [];
              localStorage["userPermissions"] = JSON.stringify(userData.linkPermissions || []);
              
              // Restore last URL and navigate
              const lastUrl = sessionStorage.getItem('lastUrl');
              if (lastUrl) {
                sessionStorage.removeItem('lastUrl');
                this.router.navigate([lastUrl]);
              }
              return true;
            }
          }
        }
        this.router.navigate(['/login']);
        return false;
      }),
      catchError((error) => {
        console.error('Authentication failed', error);
        this.router.navigate(['/login']);
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