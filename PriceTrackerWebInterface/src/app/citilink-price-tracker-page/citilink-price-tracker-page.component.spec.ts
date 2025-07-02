import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CitilinkPriceTrackerPageComponent } from './citilink-price-tracker-page.component';

describe('CitilinkPriceTrackerPageComponent', () => {
  let component: CitilinkPriceTrackerPageComponent;
  let fixture: ComponentFixture<CitilinkPriceTrackerPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CitilinkPriceTrackerPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CitilinkPriceTrackerPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
