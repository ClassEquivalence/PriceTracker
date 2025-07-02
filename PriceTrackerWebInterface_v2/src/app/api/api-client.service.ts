import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiRoutes } from './api-routes';
import { Observable } from 'rxjs';
import { DetailedMerchDto } from './models/detailed-merch-dto';

@Injectable({
  providedIn: 'root'
})

export class ApiClientService {
  constructor(private http: HttpClient) { }

  getProductByCitilinkCode(code: string): Observable<DetailedMerchDto> {
    const url = ApiRoutes.productByCitilinkCode(code);
    return this.http.get <DetailedMerchDto>(url);
  }

  getProductById(id: number): Observable<DetailedMerchDto> {
    const url = ApiRoutes.productById(id);
    return this.http.get<DetailedMerchDto>(url);
  }
}
