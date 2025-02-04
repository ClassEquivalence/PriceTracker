using PriceTracker.Models.BaseModels;
using PriceTracker.Models.TestModels.ShopNamedTfarg;

public class TfargShop: IShop
{
    public int Id { get; set; } = 1;
    public string Name { get; set; } = "Tfarg";
    public List<TestTfargMerch> MerchList { get; set; }

    public TfargShop(string name, List<TestTfargMerch>? shopMerches)
    {
        Name = name;
        MerchList = shopMerches==null?new():shopMerches;
    }
    public IEnumerable<IShopMerch> GetAllMerches()
    {
        return MerchList;
    }
    public IShopMerch GetMerch(int id)
    {
        try
        {
            return MerchList.Single(merch => merch.Id == id);
        }
        catch (InvalidOperationException)
        {

        }
        return MerchList.Single(merch => merch.Id == id);
    }
}