export interface TimestampedPriceDto {
  id: number;
  price: number;
  dateTime: string;
  merchPriceHistoryId: number;
}

/*(int Id, decimal Price, DateTime DateTime,
        int MerchPriceHistoryId) */
