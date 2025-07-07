import { Component, Input, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';

import { ApiRoutes } from '../api/api-routes';
import { ApiClientService } from '../api/api-client.service';
import { DetailedMerchDto } from '../api/models/detailed-merch-dto';
import { PriceHistoryChartComponent } from './price-history-chart/price-history-chart.component';
import { TimestampedPriceDto } from '../api/models/timestamped-price-dto';


@Component({
  selector: 'app-merch-price-history-page',
  templateUrl: './merch-price-history-page.component.html',
  styleUrl: './merch-price-history-page.component.css',
  imports: [PriceHistoryChartComponent],
  standalone: true,
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

    if (this.product == null) {

      var productObservable = this.client.getProductById(this.id);
      //this.product = await firstValueFrom(productObservable);

      productObservable.subscribe({
        next: this.assignProductAndTimestampedPricesValues,
        error: err => {
          console.error('Error loading product', err);
        }
      });

    }
    else {
      this.assignProductAndTimestampedPricesValues(this.product);
    }

  }


  assignProductAndTimestampedPricesValues(productData: DetailedMerchDto) {
    console.log('Product data:', productData);
    this.product = productData;
    var priceHistory = this.product.merchPriceHistory;

    var previousPrices = priceHistory.previousTimestampedPricesList;
    if (previousPrices != null && previousPrices != undefined) {

      this.timestampedPrices =
        previousPrices.concat(priceHistory.currentPrice);
    }
    else {

      this.timestampedPrices = [priceHistory.currentPrice];
    }
  }


  id: number;
  title: string;

  // По идее, первоначальное присваивание должно перебиваться присваиванием через Input
  // от родителя.
  @Input() product: DetailedMerchDto | null = null;

  timestampedPrices: TimestampedPriceDto[] | undefined = undefined;

  rteee: string;

}
