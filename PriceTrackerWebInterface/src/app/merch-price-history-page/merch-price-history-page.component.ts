import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ApiRoutes } from '../api/api-routes';

import { ApiClientService } from '../api/api-client.service';
import { DetailedMerchDto } from '../api/models/detailed-merch-dto';

@Component({
  selector: 'app-merch-price-history-page',
  templateUrl: './merch-price-history-page.component.html',
  styleUrl: './merch-price-history-page.component.css'
})
export class MerchPriceHistoryPageComponent implements OnInit {
  constructor(private titleService: Title,
    private route: ActivatedRoute,
    private client: ApiClientService) {


    var idAsString = this.route.snapshot.paramMap.get('id');

    if (idAsString != null) {

      this.id = Number.parseInt(idAsString);
    }
    else {
      this.id = 0;
    }
    this.title = `Обзор истории цен товара someMerch ${this.id}`;

    console.log('Product ID:', this.id);

    this.rteee = ApiRoutes.productById(this.id);

    }

  async ngOnInit() {
    this.titleService.setTitle(this.title);

    var productObservable = this.client.getProductById(this.id);
    //this.product = await firstValueFrom(productObservable);

    productObservable.subscribe({
      next: data => {
        console.log('Product data:', data);
        this.product = data;
      },
      error: err => {
        console.error('Error loading product', err);
      }
    });

  }

  id: number;
  title: string;

  product: DetailedMerchDto | null = null;

  rteee: string;

}
