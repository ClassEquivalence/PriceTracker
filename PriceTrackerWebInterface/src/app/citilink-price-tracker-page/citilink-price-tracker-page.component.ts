import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ApiClientService } from '../api/api-client.service';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'
import { AppRoutes } from '../routes';
import { firstValueFrom } from 'rxjs';

import { MerchPriceHistoryPageComponent } from
  '../merch-price-history-page/merch-price-history-page.component';
import { DetailedMerchDto } from '../api/models/detailed-merch-dto';

@Component({
    selector: 'app-citilink-price-tracker-page',
    templateUrl: './citilink-price-tracker-page.component.html',
  styleUrl: './citilink-price-tracker-page.component.css',
  imports: [MerchPriceHistoryPageComponent, MatProgressSpinnerModule],
})
export class CitilinkPriceTrackerPageComponent implements OnInit {
  constructor(private titleService: Title,
    private apiClient: ApiClientService,
    private router: Router) { }

  ngOnInit() {
    this.titleService.setTitle(this.title);
  }

  async tryOpenProductPage(code: string) {
    var merch = await firstValueFrom(this.apiClient.getProductByCitilinkCode(code));
    

    this.router.navigateByUrl(AppRoutes.citilinkProduct(merch.id));

    // TODO: сделать вызов страницы с товаром по идентификатору товара,
    // а не по коду товара ситилинк.
    /*
    Ещё нормально соединить бекенд-апи с клиентом ангуляра с передачей товара.
    Ещё сделать так, чтобы при загрузке страницы по пути навигации отсюда,
    грузить заново товар из бекенда бы не пришлось. (чатгпт опрашивал)
    */

  }

  async tryLoadProductPage(code: string) {

    this.productComponentLoading = true;
    this.productComponentLoadFailed = false;
    console.log("Попытка загрузить продукт.");

    try {
      this.merch = await firstValueFrom(this.apiClient.getProductByCitilinkCode(code));
      if (this.merch == null || this.merch == undefined)
        throw new Error("Failed to load product");
      this.productComponentLoading = false;
      this.productComponentLoaded = true;
      console.log("Продукт загружен.");
    }
    catch {
      console.log("С загрузкой что то пошло не так.");
      this.productComponentLoadFailed = true;
      this.productComponentLoading = false;
      this.productComponentLoaded = false;
    }

  }

  merch: DetailedMerchDto | null = null;

  title = "Трекер цен магазина Ситилинк";

  productComponentLoading: boolean = false;
  productComponentLoaded: boolean = false;
  productComponentLoadFailed: boolean = false;

}
