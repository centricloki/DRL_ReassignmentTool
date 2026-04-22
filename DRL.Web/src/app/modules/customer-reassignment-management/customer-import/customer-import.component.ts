import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { GeolocationService } from 'src/app/services/geolocation.service';
import { CustomerimportService, CustomerMaster } from '../customerimport.service';
import { ToasterService, ToasterConfig } from 'angular2-toaster';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { AppConstant } from 'src/app/app.constants';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { Observable, Subject } from 'rxjs';
import { takeUntil, finalize, catchError } from 'rxjs/operators';

declare var $: any;

@Component({
  selector: 'app-customer-import',
  templateUrl: './customer-import.component.html',
  styleUrls: ['./customer-import.component.css']
})
export class CustomerImportComponent implements OnDestroy {
  private unsubscribe$ = new Subject<void>();

  constructor(private _geolocationService: GeolocationService
    , private _customerimportService: CustomerimportService
    , private _toasterService: ToasterService
    , public _appConstant: AppConstant
  ) { }

  // public toasterconfig: ToasterConfig =
  //   new ToasterConfig({
  //     timeout: 5000,
  //     tapToDismiss: true,
  //     showCloseButton: true
  //   });

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  @ViewChild(TooltipDirective) public tooltipDir: TooltipDirective;
  public gridData: any[] = [];
  public gridViewData: any[] = [];
  data: any[] = [];
  isGetLatLongDisabled = true;
  isUploadDisabled = true;
  //Template verification
  requiredColumns: string[] = ['Store Number', 'Store Manager', 'Store Phone', 'Store Email Address', 'Store Name', 'Address', 'City', 'ST', 'Zip', 'Territory Number', 'Is Parent', 'Parent Number', 'County', 'Account Type', 'Account Classification'];
  public accountClassificationMap = {
    '1': 'Full Service Distributor',
    '2': 'Cash & Carry',
    '3': 'C-Store',
    '7': 'Tobacco Outlet',
    '8': 'Other',
    '22': 'C-Store – Chain HQ',
    '23': 'C-Store – Chain Location',
    '24': 'C-Store – Independent',
    '25': 'Tobacco Outlet – Chain HQ',
    '26': 'Tobacco Outlet – Chain Location',
    '27': 'Tobacco Outlet – Independent',
    '28': 'Smoke Shop',
    '29': 'Dispensary Store',
    '30': 'S-D-M – Chain HQ',
    '31': 'S-D-M – Chain Location',
    '32': 'S-D-M – Independent',
    '33': 'Liquor Store – Chain HQ',
    '34': 'Liquor Store – Chain Location',
    '35': 'Liquor Store – Independent',
    '36': 'Sub jobber Wholesale',
    '37': 'Tribal Accounts',
    '38': 'Out of business',
    '39': 'Grocery Warehouse',
    '40': 'Retail Oper. w/ own Distribution Center',
    '41': 'Truck Jobber',
    '42': 'Master Distributor',
    '43': 'Prisons',
    '44': 'Smoke Shop - Chain HQ',
    '45': 'Smoke Shop - Chain Location',
    '46': 'DM Location',
    '47': 'MSAi List A'
  };

  isNumber(value: any): value is number {
    return typeof value === 'number' && !isNaN(value);
  }

  isString(value: any): value is string {
    return typeof value === 'string';
  }

  isValidAccountClassification = (accountClassification) => {
    const accountClassificationValue = String(accountClassification).trim();
    return (
      Object.keys(this.accountClassificationMap).includes(accountClassificationValue) ||
      Object.values(this.accountClassificationMap).map(value => value.toLowerCase()).includes(accountClassificationValue.toLowerCase())
    );
  };

  gridView: GridDataResult = {
    data: this.gridData.slice(0, this._appConstant.pageSize),
    total: this.gridData.length
  };

  state: State = {
    skip: 0,
    take: this._appConstant.pageSize
  };

  loadItems(): void {
    this.gridView = process(this.gridData, this.state);
  }

  dataStateChange(state): void {
    this.state = state;
    this.loadItems();
  }

  onFileChange(event: any) {
    const target: DataTransfer = <DataTransfer>(event.target);

    if (target.files.length !== 1) throw new Error('Cannot use multiple files');
    this.gridData = [];
    this.isGetLatLongDisabled = true;
    this.isUploadDisabled = true;

    const reader: FileReader = new FileReader();
    reader.onload = (e: any) => {
      const binaryStr: string = e.target.result;
      const workbook: XLSX.WorkBook = XLSX.read(binaryStr, { type: 'binary' });
      const sheetName: string = workbook.SheetNames[0];
      const worksheet: XLSX.WorkSheet = workbook.Sheets[sheetName];

      const excelData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });

      const headers = excelData[0] as string[];
      this.data = excelData.slice(1);

      //Template Verification
      if (headers && headers.length > 0) {
        const validateTemplate = this.validateRequiredColumns(headers)
        if (!validateTemplate.isTemplateValid) {
          this.isGetLatLongDisabled = true;
          this.isUploadDisabled = true;
          this._toasterService.pop({
            type: 'error',
            title: 'Error',
            body: validateTemplate.errorMessage,
            timeout: 0,
            showCloseButton: true
          });
          return;
        } else {
          this.isGetLatLongDisabled = false;
        }
      }

      // Map Excel data to grid format
      this.gridData = this.data.map((row: any[]) => ({
        storeNumber: row[0],
        storeManager: row[1],
        storePhone: row[2],
        storeEmail: row[3],
        storeName: row[4],
        address: row[5],
        city: row[6],
        state: row[7],
        zip: row[8],
        territoryNumber: row[9],
        isParent: row[10],
        parentNumber: row[11],
        county: row[12],
        accountType: row[13],
        accountClassification: row[14],
        latitude: '',  // Initially blank, will be updated later
        longitude: ''  // Initially blank, will be updated later
      }));

      if (this.data && this.data.length > 0) {
        const validationResult = this.validateExcelData(this.data);
        if (!validationResult.isValid) {
          this.isGetLatLongDisabled = true;
          this.isUploadDisabled = true;
          this._toasterService.pop({
            type: 'warning',
            title: 'Warning',
            body: validationResult.errorMessage,
            timeout: 0,
            showCloseButton: true
          });
          console.log(validationResult.errorMessage)
        } else {
          this.isGetLatLongDisabled = false;
          this.loadItems();
        }
      } else {
        this.isGetLatLongDisabled = true;
      }
    };
    reader.readAsArrayBuffer(target.files[0]);
    // event.target.value = '';
  }

  validateRequiredColumns(headers: string[]): { isTemplateValid: boolean, errorMessage: string } {
    const missingColumns: string[] = [];

    this.requiredColumns.forEach((column) => {
      if (!headers.includes(column)) {
        missingColumns.push(column);
      }
    });

    if (missingColumns.length > 0) {
      return { isTemplateValid: false, errorMessage: `The following required columns are missing: ${missingColumns.join(', ')}` };
    }

    return { isTemplateValid: true, errorMessage: '' };
  }

  validateExcelData(data: any[]): { isValid: boolean, errorMessage: string } {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const numericRegex = /^\d+$/;
    let errorMessages: string[] = [];

    for (let i = 0; i < data.length; i++) {
      const row = data[i];
      let rowErrors: string[] = [];

      const customerName = row[4];
      const email = row[3];
      const zipCode = row[8];
      const address = row[5];
      const city = row[6];
      const state = row[7];
      const territory = row[9];
      const isParent = row[10];
      //const county = row[12];
      const accountType = row[13];
      const accountClassification = row[14];

      // Validate Customer Name
      if (!customerName || customerName.trim() === '') {
        rowErrors.push(`Customer Name cannot be blank.`);
      }

      // Validate Email
      if (!email || email.trim() === '') {
        rowErrors.push(`Email cannot be blank.`);
      }
      if (email && !emailRegex.test(email.trim())) {
        rowErrors.push(`Invalid Email format.`);
      }

      if (!zipCode || zipCode.length < 0) {
        rowErrors.push(`Zip Code cannot be blank.`);
      }

      // Validate Zip Code (must be numeric)
      if (zipCode && !numericRegex.test(zipCode)) {
        rowErrors.push(`Zip Code must be numeric.`);
      }

      if (!address || address.trim() === '') {
        rowErrors.push(`Address cannot be blank.`);
      }

      if (!city || city.trim() === '') {
        rowErrors.push(`City cannot be blank.`);
      }

      if (!state || state.trim() === '') {
        rowErrors.push(`State cannot be blank.`);
      }

      if (territory) {
        if (this.isNumber(territory))
          rowErrors.push(`Territory must be a valid name.`);
        else if (this.isString(territory) && territory.trim() === '')
          rowErrors.push(`Territory cannot be blank.`);
      } else {
        rowErrors.push(`Territory cannot be blank.`);
      }

      if (isParent === null || isParent === undefined || (isParent !== 0 && isParent !== 1)) {
        rowErrors.push('Is Parent must be either 0 or 1.');
      }

      // if (!county || county.trim() === '') {
      //   rowErrors.push(`County cannot be blank.`);
      // }

      if (!accountType || !['1', '2', 'direct', 'indirect'].includes(String(accountType).toLowerCase())) {
        rowErrors.push(`Account Type must be either 1, 2, Direct or Indirect.`);
      }

      if (!accountClassification || String(accountClassification).trim() === '') {
        rowErrors.push(`Account Classification cannot be blank.`);
      } else if (!this.isValidAccountClassification(accountClassification)) {
        rowErrors.push(`Account Classification must be a valid number or name.`);
      }

      // Joining all errors for that row
      if (rowErrors.length > 0) {
        errorMessages.push(`Row ${i + 2}: ${rowErrors.join('\n')}`);
      }
    }
    // Return if there are any errors and joining errors for all rows
    if (errorMessages.length > 0) {
      return { isValid: false, errorMessage: errorMessages.join('\n') };
    }
    return { isValid: true, errorMessage: '' };
  }

  async getLatLong() {
    this.showLoader();

    // Loop through each item in the grid data
    for (let i = 0; i < this.gridData.length; i++) {
      const row = this.gridData[i];
      const address = `${row.storeNumber}, ${row.address}, ${row.city}, ${row.state}, ${row.zip}, ${row.county}`;

      // Await the latitude and longitude fetching
      await this._geolocationService.getCoordinates(address)
        .pipe(takeUntil(this.unsubscribe$)
          , finalize(() => this.hideLoader())
          , catchError(err => {
            console.error(err);
            return Observable.of([]); // Return empty array on error
          }))
        .subscribe(response => {
          row.latitude = response[0];
          row.longitude = response[1];

          this.gridData[i] = row;
        }, (err) => console.error(err));

      this.isUploadDisabled = false; // Enable upload button after data is fetched
    }
  }


  downloadExcel() {
    this.showLoader();

    // Define headers
    const headers = ['Store Number', 'Store Manager', 'Store Phone', 'Store Email', 'Store Name', 'Address', 'City', 'State', 'Zip', 'Territory Number', 'Is Parent', 'Parent Number', 'County', 'Account Type', 'Account Classification', 'Latitude', 'Longitude'];

    // Map gridData into a 2D array format, converting each object into an array of values
    const sheetData = this.gridData.map(item => [
      item.storeNumber,
      item.storeManager,
      item.storePhone,
      item.storeEmail,
      item.storeName,
      item.address,
      item.city,
      item.state,
      item.zip,
      item.territoryNumber,
      item.isParent,
      item.parentNumber,
      item.county,
      item.accountType,
      item.accountClassification,
      item.latitude,
      item.longitude
    ]);

    // Combine headers and data rows
    const excelSheetData: any[][] = [headers].concat(sheetData);

    // Create the worksheet and workbook
    const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(excelSheetData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Customer Import');

    // Write the workbook to a binary format
    const workbookOutput = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });

    // Save the file
    saveAs(new Blob([workbookOutput], { type: 'application/octet-stream' }), 'GeoLocation_data.xlsx');

    this.hideLoader();
  }


  saveDataToDatabase() {
    this.showLoader();
    // Map the gridData to match the CustomerMaster structure
    const customersToAdd: CustomerMaster[] = this.gridData.map((row: any) => ({
      CustomerName: row.storeName,
      EmailId: row.storeEmail,
      Address: row.address,
      AddressCity: row.city,
      AddressState: row.state,
      AddressZipCode: row.zip.toString(),
      TerritoryCode: row.territoryNumber.toString(),
      IsParent: row.isParent.toString() === '1' ? true : false,
      Parent: !row.parentNumber || row.parentNumber === 'unidentified' ? '' : row.parentNumber,
      AccountType: row.accountType,
      AccountClassification: row.accountClassification,
      Latitude: row.latitude,
      Longitude: row.longitude
    }));

    // Send the mapped data to the customer import service
    this._customerimportService.addCustomerMaster(customersToAdd)
      .pipe(takeUntil(this.unsubscribe$), finalize(() => this.hideLoader()))
      .subscribe(
        (res) => {
          if (res.isSuccess) {
            this._toasterService.pop('success', 'Success', res.message);
          }
          else {
            this._toasterService.pop({
              type: 'warning',
              title: 'Warning',
              body: res.message,
              timeout: 0,
              showCloseButton: true
            });
          }
        }, (error: any) => {
          console.log(error.message);
          this._toasterService.pop('error', 'Error', error.message);
        });
  }

  downloadTemplate() {
    // Path to your Excel template file inside the assets folder
    const filePath = 'assets/Template/ClientImportTemplate.xlsx';

    // Create a temporary anchor element
    const link = document.createElement('a');
    link.href = filePath;
    link.download = 'Client_Import_Template.xlsx';  // Name the file for download

    // Trigger the download
    link.click();
  }

  public showTooltip(e: MouseEvent): void {
    const element = e.target as HTMLElement;
    if ((element.nodeName === 'TD' || element.nodeName === 'TH') && element.offsetWidth < element.scrollWidth) {
      this.tooltipDir.toggle(element);
    } else {
      this.tooltipDir.hide();
    }
  }

  private showLoader(): void {
    $('.ajax-loading').show();
  }

  private hideLoader(): void {
    $('.ajax-loading').hide();
  }
}
