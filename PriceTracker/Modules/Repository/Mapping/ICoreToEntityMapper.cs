using PriceTracker.Core.Models;
using PriceTracker.Modules.Repository.Entities;

namespace PriceTracker.Modules.Repository.Mapping
{
    /// <summary>
    /// ВАЖНО: Этот маппер только создаёт новые модели на базе имеющихся.
    /// Контроль за единичностью инстансов моделей отвечающих одной и той же сущности,
    /// может осуществляться другими компонентами.
    /// 
    /// Маппинг из Dto в Entity может влечь потерю информации.
    /// </summary>
    /// <typeparam name="TCoreModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICoreToEntityMapper<TCoreModel, TEntity>
        where TCoreModel : BaseDto where TEntity : BaseEntity
    {
        TCoreModel Map(TEntity entity);
        TEntity Map(TCoreModel model);
    }
}
