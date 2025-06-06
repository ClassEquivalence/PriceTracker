using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.Extensions
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
