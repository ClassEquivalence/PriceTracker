using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain.MerchMapping
{

    /// <summary>
    /// Эта версия подмаппера должна работать с конкретными типами-наследниками,
    /// при этом она возвращает их апкастнутую версию.
    /// В метод Map для неполучения исключения надо передавать типы строго соответствующие
    /// HandledTypes.
    /// </summary>
    public interface IMerchSubtypeMapper : IBidirectionalDomainEntityMapper<MerchModel, MerchEntity>
    {
        (Type domain, Type entity) HandledTypes { get; }
    }
    public interface IMerchSubtypeMapper<TDomain, TEntity> : IBidirectionalDomainEntityMapper<TDomain, TEntity>
        where TDomain : MerchModel where TEntity : MerchEntity
    {

    }
}
