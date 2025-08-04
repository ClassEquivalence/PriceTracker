import { Component } from '@angular/core';

import { AppRoutes } from '../routes';



@Component({
  selector: 'app-main-page',
  imports: [],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.css'
})
export class MainPageComponent {
  citilinkTrackerUrl: string = AppRoutes.citilinkTrackerPage;
}
