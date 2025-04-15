using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DomainModels;
using System.Runtime.CompilerServices;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base
{

    // TODO: [Важно] Важно гарантировать единичность всех инстансов TDomain'а.
    public interface IDomainToEntityMapper<TDomain, TEntity> where TDomain : BaseModel where TEntity : BaseEntity
    {
        public TEntity Map(TDomain domainModel);
        public TDomain Map(TEntity entity);


    }
}
