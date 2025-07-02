export interface TimestampedPriceDto {
  id: number;
  price: number;
  dateTime: Date;
  merchPriceHistoryId: number;
}

/*(int Id, decimal Price, DateTime DateTime,
        int MerchPriceHistoryId) */
