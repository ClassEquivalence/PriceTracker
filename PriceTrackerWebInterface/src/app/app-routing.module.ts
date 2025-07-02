import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CitilinkPriceTrackerPageComponent } from
  './citilink-price-tracker-page/citilink-price-tracker-page.component';
import { MerchPriceHistoryPageComponent } from
  './merch-price-history-page/merch-price-history-page.component';

const routes: Routes = [
  { path: "citilinktracker", component: CitilinkPriceTrackerPageComponent },
  { path: "productcitilink/:id", component: MerchPriceHistoryPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
