import { TimestampedPriceDto } from './timestamped-price-dto';

export interface MerchPriceHistoryDto {
  id: number;
  previousTimestampedPricesList: TimestampedPriceDto[];
  currentPrice: TimestampedPriceDto;
  merchId: number;
}

