import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { AppConstant } from '../app.constants';

declare var $: any;

@Injectable({
  providedIn: 'root'
})
export class GeolocationService {

  constructor(private http: HttpClient,
    private _appConstant: AppConstant ) { }

  //opencage api keys
  private openCageApiKey: string = '95a00fe4478349d2913d2a8c8ca5d353';

  private geoCodeApiKey = 'AIzaSyBIbPL5doJug3ZyG0zQ4Wzz3eHXqpskVzY';
  private geoCodeApiUrl = 'https://maps.googleapis.com/maps/api/geocode/json';


  getCoordinates(address: string): Observable<[string, string]> {
    const geoCodeEnabled =  this._appConstant.geoCodeURLEnabled;
    if (!geoCodeEnabled) {
      return this.getOpenCageCoordinates(address);
    }
    else if (geoCodeEnabled) {
      return this.getGeoCodeCoordinates(address);
    } else {
      // If no API key is available, return an error
      return throwError(() => new Error('No API key available for geocoding.'));
    }
  }

  getOpenCageCoordinates(address: string): Observable<[string, string]> {
    const url = `https://api.opencagedata.com/geocode/v1/json?q=${encodeURIComponent(address)}&key=${this.openCageApiKey}`;
    // Show loader before request starts
    this.showLoader();

    return this.http.get<any>(url).pipe(
      map(response => {
        if (response && response.results && response.results.length > 0) {
          const lat = response.results[0].geometry.lat;
          const lon = response.results[0].geometry.lng;
          return [lat, lon] as [string, string];
        } else {
          return ['', ''] as [string, string]; // Return empty values if no result
        }
      }),
      tap(() => this.hideLoader()),
      catchError((error) => {
        this.hideLoader();
        console.error('Error fetching coordinates:', error);
        return throwError(() => new Error('Failed to fetch coordinates.'));
      })
    );
  }

  getGeoCodeCoordinates(address: string): Observable<[string, string]>{
    const url = `${this.geoCodeApiUrl}?sensor=true&address=${encodeURIComponent(address)}&key=${this.geoCodeApiKey}`;

    return this.http.get<any>(url).pipe(
      map(response => {
        if (response && response.results && response.results.length > 0) {
          const location = response.results[0].geometry.location;
          const lat = location.lat;
          const lon = location.lng;
          return [lat, lon] as [string, string];
        } else {
          return ['', ''] as [string, string]; // Return empty values if no result
        }
      }),
      tap(() => this.hideLoader()),
      catchError((error) => {
        this.hideLoader();
        console.error('Error fetching coordinates:', error);
        return throwError(() => new Error('Failed to fetch coordinates.'));
      })
    );
  }


  private showLoader(): void {
    $('.ajax-loading').show();
  }

  private hideLoader(): void {
    $('.ajax-loading').hide();
  }
}
