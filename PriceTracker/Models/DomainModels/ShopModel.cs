using PriceTracker.Models.DataAccess.Entities;
using System.Xml.Linq;

namespace PriceTracker.Models.DomainModels
{
    public class ShopModel: BaseModel
    {
        public string Name { get; set; }
        public virtual List<MerchModel> Merches { get; set; }
        public ShopModel(string name, List<MerchModel> shopMerches, int id = default):
            base(id)
        { 
            Name = name;
            Id = id;
            Merches = shopMerches;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidOperationException">Указанный id имеется у нескольких элементов</exception>
        /// <returns></returns>
        public MerchModel? GetMerch(int id)
        {
            return Merches.SingleOrDefault(m => m.Id == id);
        }


        /// <summary>
        /// Добавляет товар, если его параметры уникальны
        /// </summary>
        /// <param name="merch"></param>
        /// <returns>true если товар успешно добавлен, false если товар не добавлен (если его параметры неуникальны)</returns>
        public bool QueueMerchForPersistense(MerchModel merch)
        {
            if (ValidateMerchUniqueness(merch))
            {
                Merches.Add(merch);
                return true;
            }
            else
                return false;
        }

        public bool QueueMerchForPersistense(string name, TimestampedPrice currentPrice, int id = default)
        {
            return QueueMerchForPersistense(new MerchModel(name, currentPrice, this, id));
        }

        /// <summary>
        /// Изменяет название товара.
        /// </summary>
        /// <param name="merchId"></param>
        /// <param name="newName"></param>
        /// <exception cref="InvalidOperationException">Выбрасывается, если товаров с указанным Id несколько.</exception>
        /// <returns>true - название товара успешно изменено, false - не изменено 
        /// (либо нету товара с указанным Id, либо название повторяется) </returns>
        public bool ChangeMerchName(int merchId, string newName)
        {
            var merch = GetMerch(merchId);
            if (merch != null && ValidateMerchNameUniqueness(newName))
            {
                merch.Name = newName;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Удаляет товар из магазина.
        /// </summary>
        /// <param name="merchId"></param>
        /// <exception cref="InvalidOperationException">Выбрасывается, если товаров с указанным Id несколько.</exception>
        /// <returns>true если товар был удален успешно, false - товар с укзанным id не найден.</returns>
        public bool RemoveMerch(int merchId)
        {
            var merch = GetMerch(merchId);
            if (merch != null)
                return Merches.Remove(merch);
            else
                return false;
        }

        protected virtual bool ValidateMerchUniqueness(MerchModel merch)
        {
            return ValidateMerchNameUniqueness(merch.Name); // && IsIdUnique(merch.Id);
        }


        protected bool ValidateMerchNameUniqueness(string name)
        {
            return !Merches.Any(merch => merch.Name == name);
        }

        public override string ToString()
        {
            return $"id: {Id}\n" +
                $"name: {Name}\n";
        }


    }
}
