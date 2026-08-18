using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using System.Net;
using System.Text;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node
{
    public class CitilinkScrapeMerchJsonTarget : CitilinkScrapeTarget
    {
        private const string _citilinkApiRoute = "https://www.citilink.ru/graphql/";
        private const string queryInBody = @"query GetSubcategoryProductsFilter($subcategoryProductsFilterInput:CatalogFilter_ProductsFilterInput!,$categoryFilterInput:Catalog_CategoryFilterInput!,$categoryCompilationFilterInput:Catalog_CategoryCompilationFilterInput!,$isTerminal:Boolean!){productsFilter(filter:$subcategoryProductsFilterInput){record{...SubcategoryProductsFilter},error{... on CatalogFilter_ProductsFilterInternalError{__typename,message},... on CatalogFilter_ProductsFilterIncorrectArgumentsError{__typename,message}}},category(filter:$categoryFilterInput){...SubcategoryCategoryInfo}}fragment SubcategoryProductsFilter on CatalogFilter_ProductsFilter{__typename,products{...ProductSnippetFilterBase,...ProductSnippetFilterExtra},...SubcategoryFilterAdditional}fragment ProductSnippetFilterBase on Catalog_Product{...ProductSnippetBase,labels{...ProductLabel},yandexPay{withYandexSplit},rating,counters{opinions,reviews},propertiesShort{...ProductProperty}}fragment ProductSnippetBase on Catalog_Product{id,name,shortName,slug,isAvailable,images{citilink{...Image}},price{...ProductPrice},category{id,name},brand{name},multiplicity,quantityInPackageFromSupplier,recommendations{hasAnalogProducts},accessMode}fragment Image on Image{sources{url,size}}fragment ProductPrice on Catalog_ProductPrice{current,old,club,clubPriceViewType,discount{percent},bonusPoints,sbpBonus}fragment ProductLabel on Catalog_Label{id,type,title,description,target{...Target},textColor,backgroundColor,expirationTime}fragment Target on Catalog_Target{action{...TargetAction},url,inNewWindow}fragment TargetAction on Catalog_TargetAction{id}fragment ProductProperty on Catalog_Property{name,value}fragment ProductSnippetFilterExtra on Catalog_Product{id,delivery@skip(if:$isTerminal){...ProductDelivery},stock@skip(if:$isTerminal){...ProductStock},accessMode}fragment ProductDelivery on Catalog_ProductDelivery{__typename,self{...ProductSelfDelivery}}fragment ProductSelfDelivery on Catalog_ProductSelfDelivery{availabilityByDays{__typename,deliveryTime,storeCount},availableInFavoriteStores{store{id,shortName},productsCount}}fragment ProductStock on Catalog_Stock{__typename,countInStores,maxCountInStock}fragment SubcategoryFilterAdditional on CatalogFilter_ProductsFilter{sortings{...ProductsFilterSorting},groups{...SubcategoryProductsFilterGroup},compilations{popular{...SubcategoryProductCompilationInfo},brands{...SubcategoryProductCompilationInfo},carousel{...SubcategoryProductCompilationInfo}},pageInfo{...Pagination},partialPageInfo{limit,offset},searchStrategy}fragment ProductsFilterSorting on CatalogFilter_Sorting{id,name,slug,directions{id,isSelected,name,slug,isDefault}}fragment SubcategoryProductsFilterGroup on CatalogFilter_FilterGroup{id,isCollapsed,isDisabled,name,showInShortList,isGlobal,description,filter{... on CatalogFilter_ListFilter{__typename,isSearchable,logic,filters{id,isDisabled,isInShortList,isInTagList,isSelected,name,total,childGroups{id,isCollapsed,isDisabled,name,filter{... on CatalogFilter_ListFilter{__typename,isSearchable,logic,filters{id,isDisabled,isInShortList,isInTagList,name,isSelected,total}},... on CatalogFilter_RangeFilter{__typename,fromValue,isInTagList,maxValue,minValue,serifValues,scaleStep,toValue,unit}}}}},... on CatalogFilter_RangeFilter{__typename,fromValue,isInTagList,maxValue,minValue,serifValues,scaleStep,toValue,unit}}}fragment SubcategoryProductCompilationInfo on CatalogFilter_CompilationInfo{__typename,compilation{...SubcategoryProductCompilation},isSelected}fragment SubcategoryProductCompilation on Catalog_ProductCompilation{__typename,id,type,name,slug,parentSlug,seo{h1,title,text,description}}fragment Pagination on PageInfo{hasNextPage,hasPreviousPage,perPage,page,totalItems,totalPages}fragment SubcategoryCategoryInfo on Catalog_CategoryResult{... on Catalog_Category{...Category,seo{h1,title,text,description},compilation(filter:$categoryCompilationFilterInput){... on Catalog_CategoryCompilation{__typename,id,name,seo{h1,title,description,text}},... on Catalog_CategoryCompilationIncorrectArgumentError{__typename,message},... on Catalog_CategoryCompilationNotFoundError{__typename,message}},defaultSnippetType},... on Catalog_CategoryIncorrectArgumentError{__typename,message},... on Catalog_CategoryNotFoundError{__typename,message}}fragment Category on Catalog_Category{__typename,id,name,slug}";

        private readonly string _categorySlug;
        public readonly int Page;
        public readonly int PerPage;

        public CitilinkScrapeMerchJsonTarget(string categorySlug, int page,
            bool isProcessed = false, int perPage = 48) :
            base(_citilinkApiRoute, CitilinkScrapeTargetPurpose.MerchesJson, isProcessed)
        {
            _categorySlug = categorySlug;
            Page = page;
            PerPage = perPage;
        }

        /*
                (string categorySlug, int page, int perPage = 1000,
            string? cookie = default)
        */

        public override CitilinkScrapeEpisodeDescriptor CreateDescriptor()
        {
            List<HttpStatusCode> statusCodes = new List<HttpStatusCode>();
            HttpMethod httpMethod;

            httpMethod = HttpMethod.Post;
            statusCodes.AddRange([HttpStatusCode.OK]);

            string requestBody = $@"
{{
    ""query"": ""{queryInBody}"",
    ""variables"": {{
        ""subcategoryProductsFilterInput"": {{
            ""categorySlug"": ""{_categorySlug}"",
            ""compilationPath"": [],
            ""pagination"": {{
                ""page"": {Page},
                ""perPage"": {PerPage}
            }},
            ""partialPagination"": {{
                ""limit"": {PerPage},
                ""offset"": 0
            }},
            ""conditions"": [],
            ""sorting"": {{
                ""id"": """",
                ""direction"": ""SORT_DIRECTION_DESC""
            }},
            ""popularitySegmentId"": ""THREE""
        }},
        ""categoryFilterInput"": {{
            ""slug"": ""{_categorySlug}""
        }},
        ""categoryCompilationFilterInput"": {{
            ""slug"": """"
        }},
        ""isTerminal"": false
    }}
}}";
            var descriptor = new CitilinkScrapeEpisodeDescriptor(this.Url, httpMethod, statusCodes,
                requestBody: requestBody, requestBodyEncoding: Encoding.UTF8, requestBodyMediaType:
                "application/json");
            return descriptor;
        }

    }
}
