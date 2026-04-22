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
    constructor(
        backend: XHRBackend,
        defaultOptions: RequestOptions
    ) {
        super(backend, defaultOptions);
    }

    get(url: string, options?: RequestOptionsArgs): Observable<any> {

        this.showLoader();

        return super.get(url, this.requestOptions(options))
            .catch(this.onCatch)
            .do((res: Response) => {
                this.onSuccess(res);
            }, (error: any) => {
                this.onError(error);
            })
            .finally(() => {
                this.onEnd();
            });

    }

    post(url: string, body: any, options?: RequestOptionsArgs): Observable<any> {
        this.showLoader();

        return super.post(url, body, this.requestOptions(options))
            .catch(this.onCatch)
            .do((res: Response) => {
                this.onSuccess(res);
            }, (error: any) => {
                this.onError(error);
            })
            .finally(() => {
                this.onEnd();
            });

    }

    patch(url: string, body: any, options?: RequestOptionsArgs): Observable<any> {

        this.showLoader();

        return super.patch(url, body, this.requestOptions(options))
            .catch(this.onCatch)
            .do((res: Response) => {
                this.onSuccess(res);
            }, (error: any) => {
                this.onError(error);
            })
            .finally(() => {
                this.onEnd();
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
        $('.ajax-loading').hide();
        if (error.statusText == "Unauthorized") {
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
        $('.ajax-loading').hide();
        //console.log('Error, status code: ' + res.status);
    }

    private onEnd(): void {
        this.hideLoader();
    }

    private showLoader(): void {
        $('.ajax-loading').show();
    }

    private hideLoader(): void {
        $('.ajax-loading').hide();
    }
}