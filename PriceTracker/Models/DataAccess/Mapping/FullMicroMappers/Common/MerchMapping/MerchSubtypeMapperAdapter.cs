using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common.MerchMapping
{
    public class MerchSubtypeMapperAdapter<TDomain, TEntity>: IMerchSubtypeMapper
        where TDomain: MerchModel where TEntity : MerchEntity
    {
        public (Type domain, Type entity) HandledTypes { get; init; }
        private readonly IMerchSubtypeMapper<TDomain, TEntity> _actualMapper;
        public MerchSubtypeMapperAdapter(IMerchSubtypeMapper<TDomain, TEntity> actualMapper)
        {
            HandledTypes = (typeof(TDomain), typeof(TEntity));
            _actualMapper = actualMapper;
        }

        public MerchModel Map(MerchEntity entity)
        {
            if (entity is TEntity)
            {
                return _actualMapper.Map(entity as TEntity);
            }
            else
                throw new ArgumentException($"{nameof(Map)}: Аргумент {entity} должен" +
                    $"соответствовать типу {typeof(TEntity)}. Его фактический тип - " +
                    $"{entity.GetType()}");
        }

        public MerchEntity Map(MerchModel model)
        {
            if (model is TDomain)
            {
                return _actualMapper.Map(model as TDomain);
            }
            else
                throw new ArgumentException($"{nameof(Map)}: Аргумент {model} должен" +
                    $"соответствовать типу {typeof(TDomain)}. Его фактический тип - " +
                    $"{model.GetType()}");
        }

    }
}
