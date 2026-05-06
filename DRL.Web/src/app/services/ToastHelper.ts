import { Injectable } from '@angular/core';
import { ToasterService } from 'angular2-toaster';

@Injectable({ providedIn: 'root' })
export class ToastHelper {
  constructor(private toasterService: ToasterService) { }

  success(title: string, message: string) {
    this.toasterService.pop('success', title, message);
  }

  error(title: string, message: string) {
    this.toasterService.pop('error', title, message);
  }

  warning(title: string, message: string) {
    this.toasterService.pop('warning', title, message);
  }

  info(title: string, message: string) {
    this.toasterService.pop('info', title, message);
  }
}
