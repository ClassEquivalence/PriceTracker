using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Collections;
using PriceTracker.Models.BaseAppModels.TemplateMethods;

namespace PriceTracker.Models.BaseAppModels
{
    public class Shop
    {
        public int Id { get; set; }

        string name;
        public string Name
        {
            get { return name; }
            set 
            {
                if (ValidateNameAvailability == null)
                    name = value;
                else if (ValidateNameAvailability(value))
                    name = value;
                else
                {
                    throw new InvalidOperationException($"Попытка установить уже занятое название магазину {Name}");
                }
            }
        }
        public Func<string, bool>? ValidateNameAvailability;
        protected ILogger Logger { get; set; }
        public virtual ICollection<ShopMerch> Merches { get; set; }
        public Shop(string name, ILogger logger, ICollection<ShopMerch> shopMerches, int id = default) 
        { 
            this.name = name;
            Id = id;
            Logger = logger;
            Merches = shopMerches;
        }
        public ShopMerch? GetMerch(int id)
        {
            return CollectionSingleObjectController.TryGetSingle(Merches, merch => merch.Id == id,
                $"Не удалось однозначно найти товар магазина {Name} с id={id}");
        }

        public bool AddMerch(ShopMerch merch)
        {
            if (ValidateMerchUniqueness(merch))
            {
                if (merch.Id == default || !IsIdUnique(merch.Id))
                    merch.Id = getFreeId();
                Merches.Add(merch);
                merch.ValidateNameAvailability = ValidateMerchNameUniqueness;
                return true;
            }
            else
            {
                Logger.LogError($"Не получилось добавить {merch.Name} из-за повторяющихся с другими товарами параметров.");
                return false;
            }
        }

        public bool ChangeMerchName(int merchId, string newName)
        {
            var merch = GetMerch(merchId);
            if (merch != null && ValidateMerchNameUniqueness(newName))
            {
                merch.Name = newName;
                Logger.LogDebug($"Имя {newName} установлено.");
                return true;
            }
            else
                return false;
        }

        public bool RemoveMerch(int merchId)
        {
            var merch = GetMerch(merchId);
            if (merch != null)
                return Merches.Remove(merch);
            else
                return false;
        }

        protected virtual bool ValidateMerchUniqueness(ShopMerch merch)
        {
            return ValidateMerchNameUniqueness(merch.Name) && IsIdUnique(merch.Id);
        }


        protected bool ValidateMerchNameUniqueness(string name)
        {
            var count = Merches.Where(merch=>merch.Name == name).Count();
            if (count > 0)
                return false;
            else
                return true;
        }

        protected bool IsIdUnique(int id)
        {
            return Merches.Where(m => m.Id == id).Count() > 0 ? false : true;
        }

        protected int AvailableId = 1;
        int getFreeId()
        {
            while (Merches.Any(merch => merch.Id == AvailableId))
            {
                AvailableId++;
            }
            return AvailableId;
        }

        public override string ToString()
        {
            return $"id: {Id}\n" +
                $"name: {Name}\n";
        }


    }
}
