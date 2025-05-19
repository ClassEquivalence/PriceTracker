using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.Extensions
{
    public static class DomainToEntityMapperExtension
    {
        public static List<TDomain> MapMany<TDomain, TEntity>(this IBidirectionalDomainEntityMapper<TDomain, TEntity> mapper, List<TEntity> entityModels)
                where TEntity : BaseEntity where TDomain : BaseDomain
        {
            return entityModels.Select(mapper.Map).ToList();
        }
        public static List<TEntity> MapMany<TDomain, TEntity>(this IBidirectionalDomainEntityMapper<TDomain, TEntity> mapper, List<TDomain> domainModels)
            where TEntity : BaseEntity where TDomain : BaseDomain
        {
            return domainModels.Select(mapper.Map).ToList();
        }
    }
}
