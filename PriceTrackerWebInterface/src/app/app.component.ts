import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

import { LayoutComponent } from './layout/layout.component';

@Component({
    selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  styleUrl: './app.component.css',
  imports: [LayoutComponent],
})
export class AppComponent implements OnInit {

  constructor(private http: HttpClient) {}

  ngOnInit() {

  }


  title = 'PriceTrackerWebInterface';
}
