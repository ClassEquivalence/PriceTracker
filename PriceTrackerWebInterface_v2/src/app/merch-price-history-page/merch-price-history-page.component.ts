import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { firstValueFrom } from 'rxjs';

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
    private client: ApiClientService) { }

  async ngOnInit() {
    this.titleService.setTitle(this.title);
    var idAsNumber : number = 0; // TODO: Костыльное присваивание переменной.
    if (this.id != null) {
      idAsNumber = Number.parseInt(this.id);
    }
    var productObservable = this.client.getProductById(idAsNumber);
    this.product = await firstValueFrom(productObservable);
  }

  private id: string | null = this.route.snapshot.paramMap.get('id');
  title = `Обзор истории цен товара someMerch ${this.id}`;

  product: DetailedMerchDto | null = null;


}
