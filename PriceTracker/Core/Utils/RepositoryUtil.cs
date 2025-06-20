using Microsoft.Extensions.Logging;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Core.Utils
{
    //предполагается класс-синглтон.
    public static class RepositoryUtil
    {
        public static ILogger? Logger { get; set; }

        ///<summary>
        /// Попытаться взять один элемент репозитория. Если взять не получится, будет возвращен null, и в логгер будет записана
        /// информация об ошибке.
        /// foundMultipleErrorText - текст, записывающийся в логгер при неоднозначности выбора объекта
        /// notFoundErrorText - текст, записывающийся в логгер при не найденном объекте.
        ///</summary>
        public static T? TryGetSingle<T>(IDomainRepository<T> repository, Func<T, bool> getCondition, string notFoundErrorText,
            string foundMultipleErrorText) where T : BaseDomain
        {
            try
            {
                var el = repository.SingleOrDefault(getCondition);
                if (!Equals(el, default(T)))
                {
                    return el;
                }
                else
                {
                    Logger?.LogError(notFoundErrorText);
                    return default;
                }
            }
            catch (InvalidOperationException)
            {
                Logger?.LogError(foundMultipleErrorText);
                return default;
            }
            catch (ArgumentNullException)
            {
                Logger?.LogError($"TryGetSingle<{nameof(T)}>: Не передан репозиторий для выбора объекта!");
                return default;
            }
        }
        ///<summary>
        /// Попытаться удалить один элемент в репозитории. true - удаление успешно, false - провально.
        /// notFoundErrorText - текст, записывающийся в логгер при ненахождении объекта
        /// foundMultipleErrorText - текст, записывающийся в логгер при неоднозначности выбора объекта
        ///</summary>
        public static bool TryRemoveSingle<T>(IDomainRepository<T> repository, Func<T, bool> removeCondition, string notFoundErrorText,
            string foundMultipleErrorText) where T : BaseDomain
        {
            try
            {
                var el = repository.SingleOrDefault(removeCondition);
                if (!Equals(el, default(T)))
                {
                    repository.Delete(el.Id);
                    return true;
                }
                else
                {
                    Logger?.LogError(notFoundErrorText);
                }
            }
            catch (InvalidOperationException)
            {
                Logger?.LogError(foundMultipleErrorText);
            }
            return false;
        }

        ///<summary>
        /// Попытаться взять один элемент репозитория. Если взять не получится, будет возвращен null, и в логгер будет записана
        /// информация об ошибке.
        /// errorText - текст, записывающийся в логгер при какой либо ошибке при выполнении метода.
        ///</summary>
        public static T? TryGetSingle<T>(IDomainRepository<T> repository, Func<T, bool> getCondition, string errorText) where T : BaseDomain
        {
            return TryGetSingle(repository, getCondition, errorText, errorText);
        }

        ///<summary>
        /// Попытаться удалить один элемент репозитория. true - удаление успешно, false - провально.
        /// errorText - текст, записывающийся в логгер при какой либо ошибке при выполнении метода.
        ///</summary>
        public static bool TryRemoveSingle<T>(IDomainRepository<T> repository, Func<T, bool> removeCondition, string errorText) where T : BaseDomain
        {
            return TryRemoveSingle(repository, removeCondition, errorText, errorText);
        }


    }
}
