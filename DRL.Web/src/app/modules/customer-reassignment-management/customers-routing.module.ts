import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { CustomerReassignmentComponent } from './customer-reassignment/customer-reassignment.component';
import { AuthGuardService } from 'src/app/services/auth-guard.service';
import { UserReassignmentComponent } from './user-reassignment/user-reassignment.component';
import { ActionHistoryComponent } from './action-history/action-history.component';
import { CustomerImportComponent } from './customer-import/customer-import.component';
import { BrandStyleComponent } from './brand-style/brand-style.component';
import { UserReportComponent } from './user-report/user-report.component';

export const CustomerRoutes: Routes = [
  {
    path: '',
    component: CustomerReassignmentComponent,
    data: {
      pageTitle: 'Customer Reassignment Management',
      linkCode: 'CUSTOMERS'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'user-reassignment',
    component: UserReassignmentComponent,
    data: {
      pageTitle: 'User Reassignment Management',
      linkCode: 'USER_REASSIGNMENT'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'action-history',
    component: ActionHistoryComponent,
    data: {
      pageTitle: 'Action History',
      linkCode: 'ACTION_HISTORY'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'customer-import',
    component: CustomerImportComponent,
    data: {
      pageTitle: 'Customer Import',
      linkCode: 'CUSTOMER_IMPORT'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }, {
    path: 'brand-style',
    component: BrandStyleComponent,
    data: {
      pageTitle: 'Brand Style',
      linkCode: 'BRAND_STYLE'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }
  , {
    path: 'user-report',
    component: UserReportComponent,
    data: {
      pageTitle: 'User Report',
      linkCode: 'USER_REPORT'  // ❌ Sales Group: RESTRICTED
    },
    canActivate: [AuthGuardService]
  }];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(CustomerRoutes)
  ]
})
export class CustomersRoutingModule { }
