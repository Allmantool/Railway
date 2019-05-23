import { Component } from '@angular/core';

import { products } from './domains/products';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  public gridData: any[] = products;
  public title = 'Railway project';
}
