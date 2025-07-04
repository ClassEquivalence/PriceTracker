import { Component } from '@angular/core';

import { AppRoutes } from '../routes';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrl: './header.component.css',
    standalone: false
})
export class HeaderComponent {

  mainPageUrl: string = AppRoutes.mainPage;
}
