using PriceTracker.Models.BaseModels;
using PriceTracker.Models.TestModels.ShopNamedTfarg;

public class TfargShop: AbstractShop
{
    public List<TestTfargMerch> MerchList { get; set; }

    public TfargShop(string name, ILogger logger, int id = default, List<TestTfargMerch>? shopMerches = null): base (name, logger, id)
    {
        MerchList = shopMerches==null?new():shopMerches;
    }
    public override ICollection<IShopMerch> GetAllMerches()
    {
        try
        {
            var listToReturn = MerchList.ToList<IShopMerch>();
        }
        catch(ArgumentNullException)
        {
            Logger.LogCritical("Коллекция товаров не инициализирована!");
        }
        return MerchList.ToList<IShopMerch>();
    }
}

