
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository
{

    public class MerchRepository : IDomainRepository<MerchDto>
    {
        public readonly List<IMerchSubtypeRepositoryAdapter> _repositoryAdapters;

        public MerchRepository(List<IMerchSubtypeRepositoryAdapter> repositoryAdapters)
        {
            _repositoryAdapters = repositoryAdapters;
        }




        private List<(MerchDto merch, IMerchSubtypeRepositoryAdapter repository)>
            Where_WithRepositories(Func<MerchDto, bool> predicate)
        {
            List<(MerchDto merch, IMerchSubtypeRepositoryAdapter itsRepository)>
                pairs = [];
            foreach (var repository in _repositoryAdapters)
            {
                pairs.AddRange(repository.Where(predicate)
                    .Select(m => (m, repository)));
            }
            return pairs;
        }




        public List<MerchDto> Where(Func<MerchDto, bool> predicate)
        {
            List<MerchDto> merches = [];
            foreach (var repository in _repositoryAdapters)
            {
                merches.AddRange(repository.Where(predicate));
            }
            return merches;
        }

        public List<MerchDto> GetAll()
        {
            return Where(m => true);
        }


        private (MerchDto merch, IMerchSubtypeRepositoryAdapter repository)?
            SingleOrDefaultMerchRepositoryPair(Func<MerchDto, bool> predicate)
        {
            List<(MerchDto merch, IMerchSubtypeRepositoryAdapter itsRepository)>
                pairs = [];
            foreach (var repository in _repositoryAdapters)
            {
                pairs.AddRange(repository.Where(predicate)
                    .Select(m => (m, repository)));
            }
            return pairs.SingleOrDefault();
        }

        public MerchDto? SingleOrDefault(Func<MerchDto, bool> predicate)
        {
            List<MerchDto> merches = [];
            foreach (var repository in _repositoryAdapters)
            {
                merches.AddRange(repository.Where(predicate));
            }
            return merches.SingleOrDefault();
        }

        public MerchDto? GetModel(int id)
        {
            return SingleOrDefault(m => m.Id == id);
        }

        public void Create(MerchDto entity)
        {
            foreach (var repository in _repositoryAdapters)
            {
                if (repository.HandledType.IsAssignableFrom(entity.GetType()))
                {
                    repository.Create(entity);
                    break;
                }
            }
            throw new InvalidOperationException($"{nameof(Create)}: " +
                $"Не удалось найти репозиторий с подходящим подтипом. ");
        }

        public bool Update(MerchDto entity)
        {
            List<(MerchDto merch, IMerchSubtypeRepositoryAdapter itsRepository)>
                merchesWithRepositories = [];
            foreach (var repository in _repositoryAdapters)
            {
                merchesWithRepositories.AddRange(repository.Where(e => e.Id ==
                entity.Id).Select(m => (m, repository)));
            }

            var merchRepositoryPair = merchesWithRepositories.SingleOrDefault();
            if (merchRepositoryPair !=
                default((MerchDto merch, IMerchSubtypeRepositoryAdapter itsRepository)))
            {
                return merchRepositoryPair.itsRepository.Update
                    (merchRepositoryPair.merch);
            }

            return false;
        }

        public bool Delete(int id)
        {
            var pair = SingleOrDefaultMerchRepositoryPair(m => m.Id == id);
            if (pair != default((MerchDto merch, IMerchSubtypeRepositoryAdapter itsRepository))
                && pair != null)
            {
                return pair.Value.repository.Delete(id);
            }
            else
                return false;
        }



        public bool Any(Func<MerchDto, bool> predicate)
        {
            var list = Where_WithRepositories(predicate);
            return list.Any();
        }

    }
}
