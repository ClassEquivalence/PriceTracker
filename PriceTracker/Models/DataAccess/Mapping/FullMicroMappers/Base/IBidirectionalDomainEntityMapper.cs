using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DomainModels;
using System.Runtime.CompilerServices;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base
{

    // TODO: [Важно] Важно гарантировать единичность всех инстансов TDomain'а.
    public interface IBidirectionalDomainEntityMapper<TDomain, TEntity> 
        : IEntityToModelMapper<TEntity, TDomain>,
        IModelToEntityMapper<TDomain, TEntity>
        where TDomain : BaseDomain where TEntity : BaseEntity
    {


    }
}
