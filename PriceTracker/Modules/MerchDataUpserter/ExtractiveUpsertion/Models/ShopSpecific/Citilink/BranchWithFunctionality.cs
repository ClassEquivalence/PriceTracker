using HtmlAgilityPack;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink
{



    public record BranchWithFunctionality
    {

        

        public int Id;
        public string Url;
        public List<BranchWithFunctionality> Children;

        /// <summary>
        /// Ветвь обработана = из неё и её возможных наследников выкачаны все
        /// возможные товары.
        /// </summary>
        public bool IsProcessed;

        public PageFunctionality? functionality;

        public BranchWithFunctionality(int id, string url, List<BranchWithFunctionality> children, 
            bool isProcessed = false)
        {
            Id = id;
            Url = url;
            Children = children;
            IsProcessed = isProcessed;
        }

        public string GetCategoryString()
        {
            return Url.Replace("https://www.citilink.ru/catalog/", "")
                .Split('/')[0];
        }

        public string GetCategorySlug()
        {
            return GetCategoryString().Split("--")[0];
        }

        public bool HasFilter()
        {
            return GetCategoryString().Split("--").Count()>1;
        }

        public void RemoveFilter()
        {
            var categoryString = GetCategoryString();
            if (string.IsNullOrEmpty(categoryString))
                return;
            Url = Url.Replace(GetCategoryString(), GetCategorySlug());
        }

        public override string ToString()
        {
            return $"Branch({Id}): {Url}";
        }

    }

    public enum PageFunctionality
    {
        MerchCatalog, MainCatalog, SubCatalog, UnknownCatalogOfCatalogs, ServerTired, Unknown
    }

}
