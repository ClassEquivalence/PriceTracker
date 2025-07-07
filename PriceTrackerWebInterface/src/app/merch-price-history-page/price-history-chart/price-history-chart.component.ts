import { Component, Input } from '@angular/core';

import { NgxChartsModule } from '@swimlane/ngx-charts';
import { ScaleType } from '@swimlane/ngx-charts';

import { TimestampedPriceDto } from '../../api/models/timestamped-price-dto';

@Component({
  selector: 'app-price-history-chart',
  standalone: true,
  templateUrl: './price-history-chart.component.html',
  styleUrl: './price-history-chart.component.css',
  imports: [NgxChartsModule],
})
export class PriceHistoryChartComponent {



  @Input({ required: true }) timestampedPricesArray!: TimestampedPriceDto[];

  ngOnInit() {

    console.log("TimestampedPricesArray is ", this.timestampedPricesArray);

    var pairs = this.mapMultipleTimestampedPriceToNameValuePair(
      this.timestampedPricesArray);

    console.log("Series is ", pairs);
       
    this.multi = [
      {
        name: 'Цена',
        series: pairs,
      }
    ];
    console.log("График построен.");
    console.log("Данные для графика: ", this.multi.at(0)?.series);
  }

  // Присваивается в ngOnInit.
  multi!: [{ name: string, series: { name: string, value: number }[] }];
  view: [number, number] = [700, 300];

  // options
  legend: boolean = true;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = true;
  xAxisLabel: string = 'День';
  yAxisLabel: string = 'Цена';
  timeline: boolean = true;



  colorScheme =
    {
    domain: ['#0C5DA5'],
    name: 'customScheme',
    selectable: true,
    group: ScaleType.Ordinal // нужно импортировать ScaleType
  };

  mapTimestampedPriceToNameValuePair(timestampedPrice: TimestampedPriceDto)
    : { name: string, value: number }
  {
    var msDifference = Date.parse(timestampedPrice.dateTime);

    var date = new Date(msDifference).toLocaleDateString();
    var price = timestampedPrice.price;
    return { name: date, value: price };
  }

  mapMultipleTimestampedPriceToNameValuePair(
    timestampedPrices: TimestampedPriceDto[]
  ): { name: string, value: number }[] {

    var pairs: { name: string, value: number }[] = [];

    timestampedPrices.forEach(tpd => {
      var pair = this.mapTimestampedPriceToNameValuePair(tpd);
      pairs.push(pair);

    });

    return pairs;
  }

}
