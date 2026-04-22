import { Component, OnInit, OnDestroy } from '@angular/core';
import { CustomerimportService } from '../customerimport.service';
import { ToasterService } from 'angular2-toaster';
import { AppConstant } from 'src/app/app.constants';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-brand-style',
  templateUrl: './brand-style.component.html',
  styleUrls: ['./brand-style.component.css']
})
export class BrandStyleComponent implements OnInit, OnDestroy {
  brandStyles: any[] = [];
  filteredBrandStyles: any[] = [];
  sortOrders: number[] = [];
  //editState: boolean[] = [];
  public editState: { [key: number]: boolean } = {};
  public originalSortOrder: { [key: number]: number } = {};
  private unsubscribe$ = new Subject<void>();
  constructor(private _customerimportService: CustomerimportService
    , private _toasterService: ToasterService
    , public _appConstant: AppConstant) { }

  ngOnInit(): void {
    this.loadBrandStyleMaster();
  }
  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  onOptionChange(option: string) {
    this.filterBrandStyles(option);
  }

  filterBrandStyles(option: string) {
    if (option === 'brand') {
      this.filteredBrandStyles = this.brandStyles.filter(style => style.parentID === 0);
      const itemsCount = this.getMaxSortOrder();
      this.sortOrders = this.generateOrderList(itemsCount);

    } else if (option === 'rack') {
      this.filteredBrandStyles = this.brandStyles.filter(style => style.parentID !== 0);
      const itemsCount = this.getMaxSortOrder();
      this.sortOrders = this.generateOrderList(itemsCount);
    }
  }

  generateOrderList(count: number): number[] {
    return Array.from({ length: count }, (_, i) => i + 1);
  }

  getMaxSortOrder(): number {
    if (!this.filteredBrandStyles || this.filteredBrandStyles.length === 0) {
      return 0;
    }
    return this.filteredBrandStyles.reduce((max, style) => {
      const sortOrder = style.sortOrder || 0;
      return sortOrder > max ? sortOrder : max;
    }, 0);
  }

  loadBrandStyleMaster() {
    this._customerimportService.getBrandStyleMaster()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(res => {
        this.brandStyles = res.data.map(item => {
          if (item.imageFilePath && !item.imageFilePath.endsWith('.pdf')) {
            item.imageFilePath = item.imageFilePath.replace("https://honeyrel.drl-ent.com/Content", "/assets/images");
          }
          else if (item.imageFilePath && item.imageFilePath.endsWith('.pdf')) {
            item.imageFilePath = "/assets/images/BrandStyle/pdf.png";
          }
          else {
            item.imageFilePath = "/assets/images/BrandStyle/DefaultPic.png";
          }

          this.editState[item.brandIStyleID] = false
          return item;
        });
        this.filteredBrandStyles = this.brandStyles.filter(style => style.parentID === 0);
        // this.sortOrders = Array.from(new Set(this.brandStyles.map(item => item.sortOrder))).sort((a, b) => a - b);
        const itemsCount = this.getMaxSortOrder();
        this.sortOrders = this.generateOrderList(itemsCount);
      });
  }

  updateRowIndex() {
    this.filteredBrandStyles.forEach((_, index) => {
      this.editState[index] = false;
    });
  }

  onEdit(brandIStyleID: number, currentSortOrder: number): void {
    this.originalSortOrder[brandIStyleID] = currentSortOrder;
    this.editState[brandIStyleID] = true;
  }

  onCancel(brandIStyleID: number): void {
    const rowIndex = this.filteredBrandStyles.findIndex(item => item.brandIStyleID === brandIStyleID);
    if (rowIndex !== -1) {
      this.filteredBrandStyles[rowIndex].sortOrder = this.originalSortOrder[brandIStyleID];
    }

    this.editState[brandIStyleID] = false;
  }

  onSave(brandStyleID: number, newSortOrder: number, brandIStyleID: number): void {
    this._customerimportService.updateBrandStyleSortOrder(brandStyleID, newSortOrder)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(
        response => {
          if (response.isSuccess) {
            this._toasterService.pop('success', 'Success', response.message);
            this.filteredBrandStyles.sort((a, b) => a.sortOrder - b.sortOrder);
            this.editState[brandIStyleID] = false;
          }
          else {
            this._toasterService.pop('error', 'Error', response.message);
            this.editState[brandIStyleID] = false;
          }
        },
        error => {
          this._toasterService.pop('error', 'Error', error);
          this.editState[brandIStyleID] = false;
        }
      );
  }
}
