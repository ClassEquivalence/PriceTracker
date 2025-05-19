using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common.MerchMapping
{

    /// <summary>
    /// Эта версия подмаппера должна работать с конкретными типами-наследниками,
    /// при этом она возвращает их апкастнутую версию.
    /// В метод Map для неполучения исключения надо передавать типы строго соответствующие
    /// HandledTypes.
    /// </summary>
    public interface IMerchSubtypeMapper: IBidirectionalDomainEntityMapper<MerchModel, MerchEntity>
    {
        (Type domain, Type entity) HandledTypes { get; }
    }
    public interface IMerchSubtypeMapper<TDomain, TEntity> : IBidirectionalDomainEntityMapper<TDomain, TEntity>
        where TDomain : MerchModel where TEntity : MerchEntity
    {

    }
}
