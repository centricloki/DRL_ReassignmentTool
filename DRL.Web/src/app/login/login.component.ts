import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { UserModel } from '../Models/UserModel';
import { CommonService } from '../services/common.service';
import { ToasterService } from 'angular2-toaster';
import { AppConstant } from '../app.constants';
declare var $: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  SugarCRMUser = new UserModel();
  icon: boolean = false;
  public year;

  constructor(private authenticationService: AuthenticationService, private router: Router, private _commonLookupData: CommonService, private _toasterService: ToasterService, public _appConstant: AppConstant) {
  }

  ngOnInit() {
    // this.year = new Date().getFullYear();
    // $('#divfooter').hide();
    // $('#divheader').hide();
    // $('#navList').hide();


    // if (localStorage.getItem('tokenexpiredmessage') != null && localStorage.getItem('tokenexpiredmessage') != "null") {
    //   this._toasterService.pop('error', 'Error', localStorage.getItem('tokenexpiredmessage'));
    //   localStorage.setItem('tokenexpiredmessage', null);
    // }
  }
  // click() {
  //   this.icon = !this.icon;
  // }

  Login() {
    //   this.authenticationService.Authentication(this.SugarCRMUser).subscribe(data => {
    //     if (data != null) {
    //       let body = this._commonLookupData.parseData(data);
    //       if (body.isSuccess && body.data != null) {
    //         var userData = JSON.parse(JSON.stringify(body.data));

    //         localStorage.setItem('logged-in-token', userData.token);
    //         sessionStorage.setItem('logged-in-token', userData.token);

    //         sessionStorage.setItem('CurrentUserData', JSON.stringify(body.data));
    //         if (userData.token != null) {
    //           localStorage["displayName"] = userData.displayName;
    //           localStorage["userGroup"] = userData.userGroup;
    //           this._appConstant.userDisplayName = userData.displayName;
    //           this._appConstant.userGroupName = userData.userGroup;
    //           this.showHiddenDivs();
    //           this.router.navigate(['/dashboard']);
    //         }
    //         else {
    //           // this._toasterService.pop('error', 'Error', body.message + " Invalid credentials");
    //           this.router.navigate(['/error']);
    //         }
    //       }
    //       else {
    //         // this._toasterService.pop('error', 'Error', body.message + " Invalid credentials");
    //         this.router.navigate(['/error']);
    //       }
    //     }
    //   },
    //     (err: any) => {
    //       if (err.status == 401) {
    //         this.router.navigate(['error']);
    //       }
    //       else {          
    //         if (err._body != undefined && err._body != "" && typeof err._body === 'string') {
    //           var errMsg = JSON.parse(err._body)
    //           this._toasterService.pop('error', 'Error', errMsg.message);
    //         }
    //         else {
    //           this.router.navigate(['error']);
    //         }
    //       }
    //     }
    //   );
    // }
    // showHiddenDivs() {
    //   setTimeout(function () {
    //     $('#divheader').show();
    //     $('#divfooter').show();
    //     $('#navList').show();
    //   }, 100);
  }
}