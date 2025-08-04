import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CitilinkPriceTrackerPageComponent } from
  './citilink-price-tracker-page/citilink-price-tracker-page.component';
import { MerchPriceHistoryPageComponent } from
  './merch-price-history-page/merch-price-history-page.component';
import { MainPageComponent } from
  './main-page/main-page.component';
import { AboutPageComponent } from './about-page/about-page.component';

export const routes: Routes = [
  { path: '', redirectTo: 'main', pathMatch: 'full' },
  { path: "citilinktracker", component: CitilinkPriceTrackerPageComponent },
  { path: "productcitilink/:id", component: MerchPriceHistoryPageComponent },
  { path: "main", component: MainPageComponent },
  { path: "about", component: AboutPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
