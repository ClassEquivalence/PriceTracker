using Microsoft.Extensions.Logging;

namespace PriceTracker.Models.BaseAppModels.TemplateMethods
{
    //предполагается класс-синглтон.
    public static class CollectionSingleObjectController
    {
        public static ILogger? Logger { get; set; }

        ///<summary>
        /// Попытаться взять один элемент коллекции. Если взять не получится, будет возвращен null, и в логгер будет записана
        /// информация об ошибке.
        /// foundMultipleErrorText - текст, записывающийся в логгер при неоднозначности выбора объекта
        /// notFoundErrorText - текст, записывающийся в логгер при не найденном объекте.
        ///</summary>
        public static T? TryGetSingle<T>(ICollection<T> collection, Func<T, bool> getCondition, string notFoundErrorText,
            string foundMultipleErrorText)
        {
            try
            {
                var el = collection.SingleOrDefault(getCondition);
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
                Logger?.LogError($"TryGetSingle<{nameof(T)}>: Не передана коллекция для выбора объекта!");
                return default;
            }
        }
        ///<summary>
        /// Попытаться удалить один элемент коллекции. true - удаление успешно, false - провально.
        /// notFoundErrorText - текст, записывающийся в логгер при ненахождении объекта
        /// foundMultipleErrorText - текст, записывающийся в логгер при неоднозначности выбора объекта
        ///</summary>
        public static bool TryRemoveSingle<T>(ICollection<T> collection, Func<T, bool> removeCondition, string notFoundErrorText, 
            string foundMultipleErrorText)
        {
            try
            {
                var el = collection.SingleOrDefault(removeCondition);
                if (!Equals(el, default(T)))
                {
                    collection.Remove(el);
                    return true;
                }
                else
                {
                    Logger?.LogError(notFoundErrorText);
                }
            }
            catch (ArgumentNullException)
            {
                Logger?.LogError($"TryRemoveSingle<{nameof(T)}>: Не передана коллекция для выбора объекта!");
            }
            catch (InvalidOperationException)
            {
                Logger?.LogError(foundMultipleErrorText);
            }
            catch (NotSupportedException)
            {
                Logger?.LogError($"TryRemoveSingle<{nameof(T)}>: Коллекция не допускает удаления объекта!");
            }
            return false;
        }

        ///<summary>
        /// Попытаться взять один элемент коллекции. Если взять не получится, будет возвращен null, и в логгер будет записана
        /// информация об ошибке.
        /// errorText - текст, записывающийся в логгер при какой либо ошибке при выполнении метода.
        ///</summary>
        public static T? TryGetSingle<T>(ICollection<T> collection, Func<T, bool> getCondition, string errorText)
        {
            return TryGetSingle(collection, getCondition, errorText, errorText);
        }

        ///<summary>
        /// Попытаться удалить один элемент коллекции. true - удаление успешно, false - провально.
        /// errorText - текст, записывающийся в логгер при какой либо ошибке при выполнении метода.
        ///</summary>
        public static bool TryRemoveSingle<T>(ICollection<T> collection, Func<T, bool> removeCondition, string errorText)
        {
            return TryRemoveSingle(collection, removeCondition, errorText, errorText);
        }



    }
}
