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

    pairs = pairs.sort((a, b) => this.substractDateStrings(a.name, b.name));

    pairs = this.mapMultipleIsoPairsToLocale(pairs);

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

  mapTimestampedPriceToNameValuePair(t: TimestampedPriceDto)
    : { name: string, value: number } {
    const ms = Date.parse(t.dateTime);
    const iso = new Date(ms).toISOString();
    return { name: iso, value: t.price };
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

  mapIsoPairsToLocale(pair: { name: string; value: number }):
    { name: string; value: number } {
    const { name, value } = pair;
    return { name: this.formatIsoToLocaleDate(name), value };
  }

  mapMultipleIsoPairsToLocale(pairs: { name: string; value: number }[]):
    { name: string; value: number }[] {
    return pairs.map(pair => this.mapIsoPairsToLocale(pair));
  }

  substractDateStrings(a: string, b: string): number {
    return Date.parse(a) - Date.parse(b);
  }


  formatIsoToLocaleDate(isoString: string): string {
    const date = new Date(isoString);

    const datePart = date.toLocaleDateString();

    return `${datePart}`;
  }

}
