using HtmlAgilityPack;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink
{



    public record BranchWithHtml
    {

        

        public int Id;
        public string Url;
        public List<BranchWithHtml> Children;

        /// <summary>
        /// Ветвь обработана = из неё и её возможных наследников выкачаны все
        /// возможные товары.
        /// </summary>
        public bool IsProcessed;

        public HtmlNode? Node;
        public PageFunctionality? functionality;

        public BranchWithHtml(int id, string url, List<BranchWithHtml> children, 
            bool isProcessed = false, HtmlNode? node = null)
        {
            Id = id;
            Url = url;
            Children = children;
            IsProcessed = isProcessed;
            Node = node;
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
