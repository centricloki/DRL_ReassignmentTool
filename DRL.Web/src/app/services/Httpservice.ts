import { Injectable } from '@angular/core';
import 'rxjs/Rx';
import { Observable, of } from 'rxjs';

import {
    Http,
    RequestOptions,
    RequestOptionsArgs,
    Response,
    Headers,
    XHRBackend
} from '@angular/http';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

declare var $: any;

@Injectable()
export class HttpService extends Http {

    // Tracks active in-flight request IDs across ALL services to prevent count mismatches
    private static activeRequestIds: Set<number> = new Set<number>();
    private static requestCounter: number = 0;
    private static watchdogTimer: any = null;
    private static readonly WATCHDOG_TIMEOUT_MS: number = 3 * 60 * 1000; // 3 mins auto-recovery safeguard

    constructor(
        backend: XHRBackend,
        defaultOptions: RequestOptions
    ) {
        super(backend, defaultOptions);
    }

    /**
     * Resets the loader and invalidates all currently tracked requests.
     * Useful on route navigation or emergency recovery to ensure the loader never stays stuck.
     */
    public static resetLoader(): void {
        HttpService.activeRequestIds.clear();
        HttpService.clearWatchdog();
        if (typeof $ !== 'undefined') {
            $('.ajax-loading').hide();
        }
    }

    get(url: string, options?: RequestOptionsArgs): Observable<any> {
        const reqId = ++HttpService.requestCounter;
        this.showLoader(reqId);

        return super.get(url, this.requestOptions(options))
            .catch(this.onCatch.bind(this))
            .do((res: Response) => {
                this.onSuccess(res);
            }, (error: any) => {
                this.onError(error);
            })
            .finally(() => {
                this.onEnd(reqId);
            });

    }

    post(url: string, body: any, options?: RequestOptionsArgs): Observable<any> {
        const reqId = ++HttpService.requestCounter;
        this.showLoader(reqId);

        return super.post(url, body, this.requestOptions(options))
            .catch(this.onCatch.bind(this))
            .do((res: Response) => {
                this.onSuccess(res);
            }, (error: any) => {
                this.onError(error);
            })
            .finally(() => {
                this.onEnd(reqId);
            });

    }

    patch(url: string, body: any, options?: RequestOptionsArgs): Observable<any> {
        const reqId = ++HttpService.requestCounter;
        this.showLoader(reqId);

        return super.patch(url, body, this.requestOptions(options))
            .catch(this.onCatch.bind(this))
            .do((res: Response) => {
                this.onSuccess(res);
            }, (error: any) => {
                this.onError(error);
            })
            .finally(() => {
                this.onEnd(reqId);
            });

    }

    private requestOptions(options?: RequestOptionsArgs): RequestOptionsArgs {
        var userName = localStorage["userName"];
        if (options == null) {
            options = new RequestOptions({ withCredentials: true });
        }
        if (options.headers == null) {
            options.headers = new Headers();
            if (userName != null) {
                //  options.headers = new Headers({'Content-Type': 'application/json'
                //                                 ,'user-id':'test'
                //                                 // , 'Access-Control-Allow-Origin' : '*'
                //                                 , 'Access-Control-Allow-Headers' : '*'});
            }
        }

        return options;
    }

    private onCatch(error: any, caught: Observable<any>): Observable<any> {
        if (error.statusText == "Unauthorized") {
            HttpService.resetLoader();
            localStorage.clear();
            sessionStorage.clear();
            console.clear();
            window.location.href = "/";
        }
        return Observable.of("error");
    }

    private onSuccess(res: Response): void {
        //console.log('Request successful');
    }

    private onError(res: Response): void {
        // onEnd() will also be called via finally — no direct hide here
        //console.log('Error, status code: ' + res.status);
    }

    private onEnd(reqId: number): void {
        this.hideLoader(reqId);
    }

    private showLoader(reqId: number): void {
        HttpService.activeRequestIds.add(reqId);
        // Only touch DOM on first request (0→1 transition)
        if (HttpService.activeRequestIds.size === 1 && typeof $ !== 'undefined') {
            $('.ajax-loading').show();
        }
        HttpService.startWatchdog();
    }

    private hideLoader(reqId: number): void {
        // If this request ID was already cleared (e.g. by route navigation or watchdog reset), ignore it
        if (!HttpService.activeRequestIds.has(reqId)) {
            return;
        }

        HttpService.activeRequestIds.delete(reqId);

        // Only hide when truly all active requests across the app are finished
        if (HttpService.activeRequestIds.size === 0) {
            HttpService.clearWatchdog();
            if (typeof $ !== 'undefined') {
                $('.ajax-loading').hide();
            }
        }
    }

    private static startWatchdog(): void {
        if (!HttpService.watchdogTimer) {
            HttpService.watchdogTimer = setTimeout(() => {
                if (HttpService.activeRequestIds.size > 0) {
                    console.warn(`[HttpService] Watchdog auto-reset: ${HttpService.activeRequestIds.size} request(s) exceeded ${HttpService.WATCHDOG_TIMEOUT_MS / 1000}s limit.`);
                    HttpService.resetLoader();
                }
            }, HttpService.WATCHDOG_TIMEOUT_MS);
        }
    }

    private static clearWatchdog(): void {
        if (HttpService.watchdogTimer) {
            clearTimeout(HttpService.watchdogTimer);
            HttpService.watchdogTimer = null;
        }
    }
}
