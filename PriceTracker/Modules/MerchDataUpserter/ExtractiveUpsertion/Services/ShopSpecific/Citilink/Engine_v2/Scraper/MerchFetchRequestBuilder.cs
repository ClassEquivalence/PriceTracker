using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper
{
    public class MerchFetchRequestBuilder
    {

        private string _citilinkApiRoute;

        private string query = @"query GetSubcategoryProductsFilter($subcategoryProductsFilterInput:CatalogFilter_ProductsFilterInput!,$categoryFilterInput:Catalog_CategoryFilterInput!,$categoryCompilationFilterInput:Catalog_CategoryCompilationFilterInput!,$isTerminal:Boolean!){productsFilter(filter:$subcategoryProductsFilterInput){record{...SubcategoryProductsFilter},error{... on CatalogFilter_ProductsFilterInternalError{__typename,message},... on CatalogFilter_ProductsFilterIncorrectArgumentsError{__typename,message}}},category(filter:$categoryFilterInput){...SubcategoryCategoryInfo}}fragment SubcategoryProductsFilter on CatalogFilter_ProductsFilter{__typename,products{...ProductSnippetFilterBase,...ProductSnippetFilterExtra},...SubcategoryFilterAdditional}fragment ProductSnippetFilterBase on Catalog_Product{...ProductSnippetBase,labels{...ProductLabel},yandexPay{withYandexSplit},rating,counters{opinions,reviews},propertiesShort{...ProductProperty}}fragment ProductSnippetBase on Catalog_Product{id,name,shortName,slug,isAvailable,images{citilink{...Image}},price{...ProductPrice},category{id,name},brand{name},multiplicity,quantityInPackageFromSupplier,recommendations{hasAnalogProducts},accessMode}fragment Image on Image{sources{url,size}}fragment ProductPrice on Catalog_ProductPrice{current,old,club,clubPriceViewType,discount{percent},bonusPoints,sbpBonus}fragment ProductLabel on Catalog_Label{id,type,title,description,target{...Target},textColor,backgroundColor,expirationTime}fragment Target on Catalog_Target{action{...TargetAction},url,inNewWindow}fragment TargetAction on Catalog_TargetAction{id}fragment ProductProperty on Catalog_Property{name,value}fragment ProductSnippetFilterExtra on Catalog_Product{id,delivery@skip(if:$isTerminal){...ProductDelivery},stock@skip(if:$isTerminal){...ProductStock},accessMode}fragment ProductDelivery on Catalog_ProductDelivery{__typename,self{...ProductSelfDelivery}}fragment ProductSelfDelivery on Catalog_ProductSelfDelivery{availabilityByDays{__typename,deliveryTime,storeCount},availableInFavoriteStores{store{id,shortName},productsCount}}fragment ProductStock on Catalog_Stock{__typename,countInStores,maxCountInStock}fragment SubcategoryFilterAdditional on CatalogFilter_ProductsFilter{sortings{...ProductsFilterSorting},groups{...SubcategoryProductsFilterGroup},compilations{popular{...SubcategoryProductCompilationInfo},brands{...SubcategoryProductCompilationInfo},carousel{...SubcategoryProductCompilationInfo}},pageInfo{...Pagination},partialPageInfo{limit,offset},searchStrategy}fragment ProductsFilterSorting on CatalogFilter_Sorting{id,name,slug,directions{id,isSelected,name,slug,isDefault}}fragment SubcategoryProductsFilterGroup on CatalogFilter_FilterGroup{id,isCollapsed,isDisabled,name,showInShortList,isGlobal,description,filter{... on CatalogFilter_ListFilter{__typename,isSearchable,logic,filters{id,isDisabled,isInShortList,isInTagList,isSelected,name,total,childGroups{id,isCollapsed,isDisabled,name,filter{... on CatalogFilter_ListFilter{__typename,isSearchable,logic,filters{id,isDisabled,isInShortList,isInTagList,name,isSelected,total}},... on CatalogFilter_RangeFilter{__typename,fromValue,isInTagList,maxValue,minValue,serifValues,scaleStep,toValue,unit}}}}},... on CatalogFilter_RangeFilter{__typename,fromValue,isInTagList,maxValue,minValue,serifValues,scaleStep,toValue,unit}}}fragment SubcategoryProductCompilationInfo on CatalogFilter_CompilationInfo{__typename,compilation{...SubcategoryProductCompilation},isSelected}fragment SubcategoryProductCompilation on Catalog_ProductCompilation{__typename,id,type,name,slug,parentSlug,seo{h1,title,text,description}}fragment Pagination on PageInfo{hasNextPage,hasPreviousPage,perPage,page,totalItems,totalPages}fragment SubcategoryCategoryInfo on Catalog_CategoryResult{... on Catalog_Category{...Category,seo{h1,title,text,description},compilation(filter:$categoryCompilationFilterInput){... on Catalog_CategoryCompilation{__typename,id,name,seo{h1,title,description,text}},... on Catalog_CategoryCompilationIncorrectArgumentError{__typename,message},... on Catalog_CategoryCompilationNotFoundError{__typename,message}},defaultSnippetType},... on Catalog_CategoryIncorrectArgumentError{__typename,message},... on Catalog_CategoryNotFoundError{__typename,message}}fragment Category on Catalog_Category{__typename,id,name,slug}";

        public MerchFetchRequestBuilder(string citilinkApiRoute)
        {
            _citilinkApiRoute = citilinkApiRoute;
        }

        /// <summary>
        /// page начинается с единицы.
        /// categorySlug - название категории(каталога товаров) на латинице.
        /// </summary>
        /// <param name="categorySlug"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public HttpRequestMessage Build(string categorySlug, int page, int perPage = 1000,
            string? cookie = default)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);

            string requestBody = $@"
{{
    ""query"": ""{query}"",
    ""variables"": {{
        ""subcategoryProductsFilterInput"": {{
            ""categorySlug"": ""{categorySlug}"",
            ""compilationPath"": [],
            ""pagination"": {{
                ""page"": {page},
                ""perPage"": {perPage}
            }},
            ""partialPagination"": {{
                ""limit"": {perPage},
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
            ""slug"": ""noutbuki""
        }},
        ""categoryCompilationFilterInput"": {{
            ""slug"": """"
        }},
        ""isTerminal"": false
    }}
}}";

            var request = new HttpRequestMessage(HttpMethod.Post, _citilinkApiRoute);

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Content.Headers.ContentLength = requestBody.Length;

            request.Headers.Add("Accept", "*/*");

            if (cookie == default)
            {
                cookie = "_city_guessed=1; _space=chlb_cl; _tuid=afe6c93ebb00a653fa10981930de680623afd184; ab_test_segment=56 ";
            }
            
            request.Headers.Add("Cookie", cookie);

            return request;
        }

    }
}
