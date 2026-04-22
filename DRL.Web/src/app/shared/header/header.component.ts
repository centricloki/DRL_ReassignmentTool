import { Component, OnInit } from '@angular/core';
import { AppConstant } from 'src/app/app.constants';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(public _appConstant: AppConstant, private _router: Router) {
  }

  ngOnInit() {
    this._appConstant.userDisplayName = localStorage["displayName"];
    // this._appConstant.userGroupName = localStorage["userGroup"];
    // if (this._appConstant.userGroupName != '' && this._appConstant.userGroupName != null) {
    //   if (this._appConstant.userGroupName.length > 10) {
    //     this._appConstant.userGroupName = this._appConstant.userGroupName.substr(0, 10) + '...';
    //   }
    // }
  }


  logout() {
    localStorage.clear();
    sessionStorage.clear();
    localStorage.setItem('logout', 'true');
    this._router.navigate(['/']);
  }
}
