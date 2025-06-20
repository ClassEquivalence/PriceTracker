using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.Services.Mapping;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Mapping;
using System.Runtime.CompilerServices;



namespace PriceTracker.Modules.Repository.Repositories.Base
{

    /*
     TODO: [Оптимизация] Здесь, и в маппере, имеется огромный простор для оптимизации.
    Проверить, строится ли при одинаковой проекции и загруженных в контекст элементах,
    необходимых для проекции, каждый раз новый запрос к БД (вместо использования Local кеша).
     */
    public abstract class EFGenericRepository<TCoreDto, TEntity> : IRepository<TCoreDto>
        where TEntity : BaseEntity where TCoreDto : BaseDto
    {

        private readonly ICoreToEntityMapper<TCoreDto, TEntity> _mapper;
        private readonly EFGenericEntityRepository<TEntity> _entityRepository;



        public EFGenericRepository(EFGenericEntityRepository<TEntity> entityRepository,
            ICoreToEntityMapper<TCoreDto, TEntity> mapper)
        {
            _entityRepository = entityRepository;
            _mapper = mapper;
        }
        public List<TCoreDto> Where(Func<TCoreDto, bool> predicate)
        {
            return _entityRepository.Where(e => predicate(_mapper.Map(e))).
                Select(_mapper.Map).ToList();
        }

        public List<TCoreDto> GetAll()
        {
            return _entityRepository.Where().Select(_mapper.Map).ToList();
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
            return _entityRepository.Any(e => predicate(_mapper.Map(e)));
        }

        public void Create(TCoreDto model)
        {
            _entityRepository.Create(ModelToEntity(model));
        }
        public bool Update(TCoreDto model)
        {
            return _entityRepository.Update(ModelToEntity(model));
        }
        public bool Delete(int id)
        {
            return _entityRepository.Delete(id);
        }
        public void SaveChanges()
        {
            _entityRepository.SaveChanges();
        }

        /// <summary>
        /// Маппинг entity в model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected TCoreDto EntityToModel(TEntity entity)
        {
            return _mapper.Map(entity);
        }

        /// <summary>
        /// Маппинг model в entity.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected TEntity ModelToEntity(TCoreDto model)
        {
            return _mapper.Map(model);
        }


    }
}
