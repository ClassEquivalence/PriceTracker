import { TimestampedPriceDto } from './timestamped-price-dto';

export interface MerchPriceHistoryDto {
  id: number;
  PreviousTimestampedPricesList: TimestampedPriceDto[];
  CurrentPrice: TimestampedPriceDto;
  MerchId: number;
}

