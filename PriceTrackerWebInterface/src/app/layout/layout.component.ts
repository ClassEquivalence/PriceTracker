import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';

import { HeaderComponent } from '../header/header.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
  imports: [RouterOutlet, HeaderComponent, FooterComponent], 
})
export class LayoutComponent {


}
