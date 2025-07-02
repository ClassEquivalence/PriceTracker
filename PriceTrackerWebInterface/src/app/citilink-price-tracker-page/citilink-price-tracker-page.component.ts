import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ApiClientService } from '../api/api-client.service';
import { Router } from '@angular/router';
import { AppRoutes } from '../routes'
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-citilink-price-tracker-page',
  templateUrl: './citilink-price-tracker-page.component.html',
  styleUrl: './citilink-price-tracker-page.component.css'
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

  title = "Трекер цен магазина Ситилинк";
  
}
