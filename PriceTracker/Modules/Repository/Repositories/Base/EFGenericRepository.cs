using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models;
using PriceTracker.Modules.Repository.Entities;
using PriceTracker.Modules.Repository.Mapping;



namespace PriceTracker.Modules.Repository.Repositories.Base
{

    /*
     TODO: [Оптимизация] Здесь, и в маппере, имеется огромный простор для оптимизации.
    Проверить, строится ли при одинаковой проекции и загруженных в контекст элементах,
    необходимых для проекции, каждый раз новый запрос к БД (вместо использования Local кеша).
     */
    public abstract class EFGenericRepository<TCoreDto, TEntity, SpecificDbContext> :
        IRepository<TCoreDto>
        where TEntity : BaseEntity where TCoreDto : BaseDto
        where SpecificDbContext : DbContext
    {

        protected readonly ICoreToEntityMapper<TCoreDto, TEntity> Mapper;
        protected readonly IEntityRepository<TEntity> EntityRepository;

        protected readonly ILogger? _logger;

        public EFGenericRepository(IEntityRepository<TEntity> entityRepository,
            ICoreToEntityMapper<TCoreDto, TEntity> mapper, ILogger? logger = null)
        {
            EntityRepository = entityRepository;
            Mapper = mapper;

            _logger = logger;
        }
        public List<TCoreDto> Where(Func<TCoreDto, bool> predicate)
        {
            return EntityRepository.Where(e => predicate(Mapper.Map(e))).
                Select(Mapper.Map).ToList();
        }

        public List<TCoreDto> GetAll()
        {
            return EntityRepository.Where().Select(Mapper.Map).ToList();
        }

        public TCoreDto? SingleOrDefault(Func<TCoreDto, bool> predicate)
        {
            var dtos = Where(predicate);
            if (dtos.Count == 1)
                return dtos.SingleOrDefault();
            else if (dtos.Count == 0)
                return default;
            else
                throw new InvalidOperationException("В последовательности" +
                    "элементов больше 1-го.");
        }


        public bool Any(Func<TCoreDto, bool> predicate)
        {
            return EntityRepository.Any(e => predicate(Mapper.Map(e)));
        }

        public void Create(TCoreDto model)
        {
            _logger?.LogTrace($"{this.GetType().Name}, {Mapper.GetType().Name}:" +
                $" попытка отмаппить и сохранить данные в entityRepository.");
            var entity = ModelToEntity(model);
            _logger?.LogTrace($"{this.GetType().Name}, {Mapper.GetType().Name}:" +
                $" отмапплена {entity.GetType().Name}, её содержимое:\n" +
                $"{entity}");
            EntityRepository.Create(entity);
        }
        public virtual bool Update(TCoreDto model)
        {
            return EntityRepository.Update(ModelToEntity(model));
        }
        public bool Delete(int id)
        {
            return EntityRepository.Delete(id);
        }

        /// <summary>
        /// Маппинг entity в model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected TCoreDto EntityToModel(TEntity entity)
        {
            return Mapper.Map(entity);
        }

        /// <summary>
        /// Маппинг model в entity.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected TEntity ModelToEntity(TCoreDto model)
        {
            return Mapper.Map(model);
        }


    }
}
