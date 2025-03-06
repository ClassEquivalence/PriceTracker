using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Collections;
using PriceTracker.Models.BaseModels.TemplateMethods;

namespace PriceTracker.Models.BaseModels
{
    public abstract class AbstractShop
    {
        public int Id { get; set; }

        string name;
        public string Name
        {
            get { return name; }
            set 
            {
                if (isNameChangeAllowed == null)
                    name = value;
                else if (isNameChangeAllowed(value))
                    name = value;
                else
                {
                    throw new InvalidOperationException($"Попытка установить уже занятое название магазину {Name}");
                }
            }
        }
        public Func<string, bool>? isNameChangeAllowed;
        protected ILogger Logger { get; set; }
        public abstract ICollection<IShopMerch> GetAllMerches();
        public AbstractShop(string name, ILogger logger, int id = default) 
        { 
            Name = name;
            Id = id;
            Logger = logger;
        }
        public IShopMerch? GetMerch(int id)
        {
            var merches = GetAllMerches();
            return CollectionSingleObjectController.TryGetSingle(merches, merch => merch.Id == id,
                $"Не удалось однозначно найти товар магазина {Name} с id={id}");
        }

        public override string ToString()
        {
            return $"id: {Id}\n" +
                $"name: {Name}\n";
        }



    }
}
