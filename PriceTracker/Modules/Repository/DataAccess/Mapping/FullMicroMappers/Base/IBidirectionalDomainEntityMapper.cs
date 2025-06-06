using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using System.Runtime.CompilerServices;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base
{

    // TODO: [Важно] Важно гарантировать единичность всех инстансов TDomain'а.
    public interface IBidirectionalDomainEntityMapper<TDomain, TEntity>
        : IBidirectionalModelEntityMapper<TDomain, TEntity>
        where TDomain : BaseDomain where TEntity : BaseEntity
    {


    }
}
