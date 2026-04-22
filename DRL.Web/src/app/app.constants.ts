import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';

@Injectable()
export class AppConstant {

  //API Level
  public APIUrl = environment.APIUrl + 'api/';
  public Interval = environment.interval;
  public userId = '';
  public roleId = '';
  public teamId = '';
  public regionId = '';
  public pageSize = 100;
  public skip = 0;
  public noRecordsMessage = "There are no items to display.";
  public listItems: Array<Item> = [
    { text: "(All)", value: 2 },
    { text: "Active", value: 1 },
    { text: "InActive", value: 0 }
  ];
  public selectedValue: number = 2;
  public teamName = '';
  public userDisplayName;
  public userGroupName;
  public isDRLIT = false;
  public isAuthenticate = false;
  public groupValue = '';
  public currentPage = 0;
  public geoCodeURLEnabled = true;
  userPermissions: LinkPermission[] = [];

  // Fallback helper during transition
  hasLinkAccess(linkCode: string, legacyFlag?: boolean): boolean {
    const perm = this.userPermissions.find(p => p.linkCode === linkCode);
    if (perm) return perm.isVisible;
    return legacyFlag || false; // Fallback to old logic
  }

  // Optional: Get all visible links for dynamic rendering
  getVisibleLinks(): LinkPermission[] {
    return this.userPermissions.filter(p => p.isVisible);
  }
}
interface Item {
  text: string,
  value: number
}

export class LinkPermission {
  linkCode: string;
  isVisible: boolean;
  isEnabled: boolean;
}