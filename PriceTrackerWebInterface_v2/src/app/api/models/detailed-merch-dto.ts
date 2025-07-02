import { MerchPriceHistoryDto } from './price-history-dto';

export interface DetailedMerchDto {
  priceHistory: MerchPriceHistoryDto;
  name: string;
  shopId: number;
  merchId: number;
}
