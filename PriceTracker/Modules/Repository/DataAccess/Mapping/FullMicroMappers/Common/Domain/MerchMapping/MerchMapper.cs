using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain.MerchMapping
{
    public class MerchMapper : IBidirectionalDomainEntityMapper<MerchModel, MerchEntity>
    {
        private readonly Dictionary<Type, IMerchSubtypeMapper>
            _subMappersByDomain;
        private readonly Dictionary<Type, IMerchSubtypeMapper>
            _subMappersByEntity;
        public MerchMapper(List<IMerchSubtypeMapper> subMappers)
        {
            _subMappersByDomain = subMappers.ToDictionary(ks => ks.HandledTypes.domain);
            _subMappersByEntity = subMappers.ToDictionary(ks => ks.HandledTypes.entity);
        }

        public MerchModel Map(MerchEntity entity)
        {
            bool isMapperHere = _subMappersByEntity.TryGetValue(entity.GetType(), out var mapper);
            if (isMapperHere && mapper != null)
                return mapper.Map(entity);
            else
                throw new ArgumentException($"Не удалось найти нужный маппер, соответствующий типу " +
                    $"{entity.GetType()}");
        }

        public MerchEntity Map(MerchModel domain)
        {
            bool isMapperHere = _subMappersByDomain.TryGetValue(domain.GetType(), out var mapper);
            if (isMapperHere && mapper != null)
                return mapper.Map(domain);
            else
                throw new ArgumentException($"Не удалось найти нужный маппер, соответствующий типу " +
                    $"{domain.GetType()}");
        }

    }
}
