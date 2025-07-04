import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CitilinkPriceTrackerPageComponent } from './citilink-price-tracker-page/citilink-price-tracker-page.component';
import { MerchPriceHistoryPageComponent } from './merch-price-history-page/merch-price-history-page.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { LayoutComponent } from './layout/layout.component';

@NgModule({ declarations: [
        AppComponent,
        CitilinkPriceTrackerPageComponent,
        MerchPriceHistoryPageComponent,
        HeaderComponent,
        FooterComponent,
        LayoutComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule], providers: [provideHttpClient(withInterceptorsFromDi())] })
export class AppModule { }
