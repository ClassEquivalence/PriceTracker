import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MerchPriceHistoryPageComponent } from './merch-price-history-page.component';

describe('MerchPriceHistoryPageComponent', () => {
  let component: MerchPriceHistoryPageComponent;
  let fixture: ComponentFixture<MerchPriceHistoryPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MerchPriceHistoryPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MerchPriceHistoryPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
