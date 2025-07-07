import { MerchPriceHistoryDto } from './price-history-dto';

export interface DetailedMerchDto {
  merchPriceHistory: MerchPriceHistoryDto;
  name: string;
  shopId: number;
  id: number;
}
