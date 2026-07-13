using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionInvocationLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.ScrapingLayer
{
    public interface IScraper<in ExecutionConstraints, MetaData>
        where ExecutionConstraints : LayersExecutionPolicies
        where MetaData : LayersExecutionMetaData
    {
        /// <summary>
        /// Принимает на вход url запроса. Возвращает тело 
        /// ответа(может быть null в случае неуспешности)
        /// и флаг успешности эпизода скрапинга (true/false).
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<FunctionResult<string?, MetaData>> UrlToNodeAsync(string url,
            ExecutionConstraints constraints);
    }
}
