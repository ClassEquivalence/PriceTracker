using PriceTracker.Models.DataAccess.Entities;

namespace PriceTracker.Models.DomainModels
{
    public class MerchPriceHistory: BaseDomain
    {
        public IReadOnlyList<TimestampedPrice> TimestampedPrices => TimestampedPricesList;
        protected List<TimestampedPrice> TimestampedPricesList { get; set; }

        protected TimestampedPrice currentPrice;


        public TimestampedPrice CurrentPrice 
        {
            get => currentPrice;
            set
            {
                currentPrice = value;
                if (!TimestampedPricesList.Contains(value))
                    TimestampedPricesList.Add(value);

            }
        }

        public MerchPriceHistory(TimestampedPrice currentPrice, int id=default)
            :base(id)
        {
            TimestampedPricesList = [currentPrice];
            this.currentPrice = currentPrice;
        }
        public MerchPriceHistory(TimestampedPrice currentPrice, List<TimestampedPrice> timestampedPrices, int id=default):
            base(id)
        {
            TimestampedPricesList = timestampedPrices;
            this.currentPrice = currentPrice;
            if (!TimestampedPricesList.Contains(currentPrice))
                TimestampedPricesList.Add(currentPrice);
        }


        public void AddHistoricalPrice(TimestampedPrice timedPrice)
        {
            TimestampedPricesList.Add(timedPrice);
        }

        /// <summary>
        /// Удаление успешно, если элемент содержится в списке и не является текущей ценой.
        /// </summary>
        /// <param name="timedPrice"></param>
        /// <exception cref="InvalidOperationException">Выбрасывается, если существует более 1 элемента с данным Id.</exception>
        /// <returns>true, если удаление прошло успешно, false, если нет.</returns>
        public bool RemovePrice(int priceId)
        {
            var removablePrice = TimestampedPricesList.SingleOrDefault(m => m.Id == priceId);
            if (removablePrice != null && priceId != CurrentPrice.Id)
            {
                return TimestampedPricesList.Remove(removablePrice);
            }
            else
                return false;
        }

        public void ClearHistoricalPrices()
        {
            TimestampedPricesList = [CurrentPrice];
        }
    }
}
