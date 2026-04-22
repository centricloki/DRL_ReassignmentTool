import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './Httpservice';
import { AppConstant } from '../app.constants';
import { UserModel } from '../Models/UserModel';
import {RequestOptions,Headers} from '@angular/http';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private http: HttpService, private _appConstant: AppConstant) { }

  Authentication(data: UserModel): Observable<any> {
    let headers = new Headers();
    headers.append('Access-Control-Allow-Origin', '*');

    // const apiURL = this._appConstant.APIUrl + 'Account/Authenticate';
    const apiURL = this._appConstant.APIUrl + 'Account';
    return this.http.get(apiURL,{withCredentials: true,headers});
  }
}