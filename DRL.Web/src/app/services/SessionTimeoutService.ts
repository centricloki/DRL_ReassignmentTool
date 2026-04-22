import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, fromEvent, merge, timer } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { CommonService } from './common.service';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SessionTimeoutService implements OnDestroy {
    private idleTimeOut = 15 * 60 * 1000; // 15 minutes of inactivity
    //private idleTimeOut = 1 * 20 * 1000; // 20 minutes of inactivity
    private userActivity$ = new Subject<void>();
    private destroy$ = new Subject<void>();

    constructor(private router: Router, private _commonLookupData: CommonService) {
        this.init();
    }

    private init() {
        // Detect user activity
        merge(
            fromEvent(document, 'mousemove'),
            fromEvent(document, 'keydown')
        )
            .pipe(takeUntil(this.destroy$), debounceTime(1000)) // Prevent excessive triggers
            .subscribe(() => this.resetTimer());

        // Start the inactivity timer
        this.startInactivityTimer();
    }

    private startInactivityTimer() {
        timer(this.idleTimeOut)
            .pipe(takeUntil(this.userActivity$))
            .subscribe(() => this.reloadPage());
    }

    private resetTimer() {
        this.userActivity$.next(); // Cancel any existing logout timer
        this.startInactivityTimer(); // Restart the inactivity timer
    }


    private reloadPage() {
        alert('Your session has expired due to inactivity. Click OK to continue.');
        // this._commonLookupData.warningDialog('Your session has expired due to inactivity. Click OK to continue.', (result: any) => {
        //     if (result) {
        // Clear all stored data
        sessionStorage.clear();
        localStorage.clear();
        const url = new URL(environment.APIUrl);
        document.cookie.split(";").forEach((cookie) => {
            document.cookie = cookie
                .replace(/^ +/, "")
                .replace(/=.*/, "=;expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/" + url.hostname);
        });

        sessionStorage.setItem('lastUrl', this.router.url);
        // Reload the page after deleting cookies
        window.location.href = '/';
        //}
        // });
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

}
