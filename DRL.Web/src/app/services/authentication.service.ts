import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './Httpservice';
import { AppConstant } from '../app.constants';
import { UserModel } from '../Models/UserModel';
import { RequestOptions, Headers } from '@angular/http';
import { shareReplay, finalize } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private inFlightAuth$: Observable<any> | null = null;

  constructor(private http: HttpService, private _appConstant: AppConstant) { }

  Authentication(data?: UserModel): Observable<any> {
    if (this.inFlightAuth$) {
      return this.inFlightAuth$;
    }

    let headers = new Headers();
    headers.append('Access-Control-Allow-Origin', '*');

    const apiURL = this._appConstant.APIUrl + 'Account';

    this.inFlightAuth$ = this.http.get(apiURL, { withCredentials: true, headers }).pipe(
      shareReplay(1),
      finalize(() => {
        this.inFlightAuth$ = null;
      })
    );

    return this.inFlightAuth$;
  }
}