import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';
import { RouterModule, Routes, PreloadAllModules } from '@angular/router';
import { AuthGuardService } from './services/auth-guard.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SitelayoutComponent } from './shared/sitelayout/sitelayout.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { WarningDialogComponent } from './warning-dialog/warning-dialog.component';
import { MatDialogModule, MatInputModule, MatExpansionModule, MatIconModule } from '@angular/material';
import { ErrorComponent } from './error/error.component';
import { AuthenticationService } from './services/authentication.service';
import { HttpService } from './services/Httpservice';
import { HttpModule } from '@angular/http';
import { AppConstant } from './app.constants';
import { GridModule } from '@progress/kendo-angular-grid';
import { ToasterConfig, ToasterModule, ToasterService } from 'angular2-toaster/angular2-toaster';
import { RolesService } from './modules/role-management/roles.service';
import { LocationStrategy, PathLocationStrategy, CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CommonService } from './services/common.service';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { LoginComponent } from './login/login.component';
import { HttpClientModule } from '@angular/common/http';
import { SessionTimeoutService } from './services/SessionTimeoutService';

const routes: Routes = [
  {
    path: 'dashboard', component: DashboardComponent, data: { pageTitle: 'User Landing' }, pathMatch: 'full', redirectTo: ''
    , canActivate: [AuthGuardService]
  },
  { path: 'error', component: ErrorComponent, data: { pageTitle: 'Error' } },
  {
    path: 'users',
    loadChildren: 'src/app/modules/user-management/users.module#UsersModule',
    data: { pageTitle: 'User Management' }
    , canActivate: [AuthGuardService]
  },
  {
    path: 'teams',
    loadChildren: 'src/app/modules/team-management/teams.module#TeamsModule',
    data: { pageTitle: 'Team Management' }
    , canActivate: [AuthGuardService]
  },
  {
    path: 'roles',
    loadChildren: 'src/app/modules/role-management/roles.module#RolesModule',
    data: { pageTitle: 'Role Management' }
    , canActivate: [AuthGuardService]
  },
  {
    path: 'regions',
    loadChildren: 'src/app/modules/region-management/regions.module#RegionsModule',
    data: { pageTitle: 'Region Management' }
    , canActivate: [AuthGuardService]
  },
  {
    path: 'customers',
    loadChildren: 'src/app/modules/customer-reassignment-management/customers.module#CustomersModule',
    data: { pageTitle: 'Customer Reassignment Management' }
    , canActivate: [AuthGuardService]
  },
  {
    path: 'error',
    component: ErrorComponent
    // , canActivate: [AuthGuardService]
  }
];
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HeaderComponent,
    FooterComponent,
    SitelayoutComponent,
    SitelayoutComponent,
    ConfirmDialogComponent,
    ErrorComponent,
    DashboardComponent,
    WarningDialogComponent
  ],
  imports: [
    BrowserModule,
    HttpModule,
    FormsModule, ReactiveFormsModule,
    RouterModule.forRoot(routes, { 
      preloadingStrategy: PreloadAllModules,
      onSameUrlNavigation: 'reload'
    }),
    BrowserAnimationsModule,
    MatDialogModule,
    GridModule,
    MatDialogModule,
    MatInputModule,
    MatExpansionModule,
    MatIconModule,
    ToasterModule.forRoot(),
    AngularFontAwesomeModule,
    HttpClientModule
  ],
  providers: [AuthGuardService, AuthenticationService
    , CommonService, HttpService, AppConstant, ToasterService
    , RolesService, { provide: LocationStrategy, useClass: PathLocationStrategy }
    , SessionTimeoutService],
  bootstrap: [AppComponent],
  entryComponents: [ConfirmDialogComponent, WarningDialogComponent]
})
export class AppModule {
  public toasterConfig: ToasterConfig = new ToasterConfig({
    timeout: 8000,  // Increase timeout duration (default is 5000ms, adjust as needed)
    newestOnTop: true,   // Display newest toasts on top
    tapToDismiss: true,  // Allow dismissing on click
    showCloseButton: true, // Enable close button
  });

}
