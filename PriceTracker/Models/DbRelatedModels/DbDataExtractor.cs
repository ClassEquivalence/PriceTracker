using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections;
using System.Runtime.CompilerServices;

namespace PriceTracker.Models.DbRelatedModels
{
    //Этот класс можно было бы помимо оптимизации, ещё и переработать качественно.
    //Это то место которое потенциально надо оптимизировать. Много оптимизировать, использовать потом лучше Lazy Loading по идее.
    //Класс работает как надо только в случае, если существует не более одного хоста СУБД и одного хоста самого сервера.
    public class DbDataExtractor<TEntity>: ICollection<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> Entities { get; set; }
        public int Count
        {
            get
            {
                SaveChanges();
                return Entities.Count();
            }
        }
        public bool IsReadOnly => false;
        protected DbContext Context;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="InvalidOperationException">
        /// Исключение срабатывает, если к указанному обобщенному типу относятся разделяемые(общие) сущности,
        /// или если таких сущностей на один тип больше чем одна.
        /// </exception>
        public DbDataExtractor(DbContext context)
        {
            var entityTypes = context.Model.FindEntityTypes(typeof(TEntity));
            string exceptionStr = "";
            if (entityTypes.Count() >= 2)
                exceptionStr = $"Сущность класса {nameof(TEntity)} не должна быть общего(разделяемого, " +
                    "shared-type entity) типа. Таких сущностей должно быть не больше на 1 CLR тип." +
                    $"Контекст ошибки: {nameof(DbDataExtractor<TEntity>)}";
            else if (entityTypes.Count() != 1)
                exceptionStr = $"Не найдена сущность для построения {nameof(DbDataExtractor<TEntity>)}.";
            if(exceptionStr != "") 
                throw new InvalidOperationException(exceptionStr);

            Entities = context.Set<TEntity>();
            Context = context;
        }


        public void Add(TEntity entity)
        {
            Entities.Add(entity);
            SaveChanges();
        }

        public void Clear()
        {
            SaveChanges();
            Entities.RemoveRange(Entities.ToArray());
            SaveChanges();
        }

        public bool Contains(TEntity entity)
        {
            // По хорошему вызов метода правого операнда должен быть после вызова левого.
            // только тогда можно говорить о том, что метод Contains работает как задумано.
            return Entities.Contains(entity);
        }
        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            var arr = Entities.ToArray();
            arr.CopyTo(array, arrayIndex);
        }
        public bool Remove(TEntity entity)
        {
            if (Contains(entity))
            {
                Entities.Remove(entity);
                SaveChanges();
                return true;
            }
            else
                return false;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Entities.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() 
        { 
            return GetEnumerator(); 
        }

        protected void SaveChanges()
        {
            if(Context.ChangeTracker.HasChanges())
                Context.SaveChanges();
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveChangesInterval"> Период, за который происходит обращение к БД в секундах</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected void ProcessSavingChanges(int saveChangesInterval)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(saveChangesInterval,
                $"В методе {nameof(ProcessSavingChanges)} нельзя указывать нулевой" +
                    $"либо отрицательный интервал сохранения изменений в БД");
            Task.Run(() =>
            {
                while (true)
                {
                    //в секунде 1000 милисекунд.
                    Task.Delay(saveChangesInterval * 1000);
                    SaveChanges();
                }
            });

        }
        */


        public void OnExit()
        {
            SaveChanges();
        }
    }
}
