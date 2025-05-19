using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Mapping;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DataAccess.Repositories.MerchRepository;
using System;
using System.Collections.Generic;

namespace PriceTracker.Models.DataAccess.Repositories.MerchRepository
{

    public class MerchRepository : IRepository<MerchModel>
    {
        public readonly List<IMerchSubtypeRepositoryAdapter> _repositoryAdapters;

        public MerchRepository(List<IMerchSubtypeRepositoryAdapter> repositoryAdapters)
        {
            _repositoryAdapters = repositoryAdapters;
        }




        private List<(MerchModel merch, IMerchSubtypeRepositoryAdapter repository)>
            Where_WithRepositories(Func<MerchModel, bool> predicate)
        {
            List<(MerchModel merch, IMerchSubtypeRepositoryAdapter itsRepository)>
                pairs = [];
            foreach (var repository in _repositoryAdapters)
            {
                pairs.AddRange(repository.Where(predicate)
                    .Select(m => (m, repository)));
            }
            return pairs;
        }




        public List<MerchModel> Where(Func<MerchModel, bool> predicate)
        {
            List<MerchModel> merches = [];
            foreach(var repository in _repositoryAdapters)
            {
                merches.AddRange(repository.Where(predicate));
            }
            return merches;
        }

        public List<MerchModel> GetAll()
        {
            return Where(m => true);
        }


        private (MerchModel merch, IMerchSubtypeRepositoryAdapter repository)?
            SingleOrDefaultMerchRepositoryPair(Func<MerchModel, bool> predicate)
        {
            List<(MerchModel merch, IMerchSubtypeRepositoryAdapter itsRepository)>
                pairs = [];
            foreach (var repository in _repositoryAdapters)
            {
                pairs.AddRange(repository.Where(predicate)
                    .Select(m=>(m, repository)));
            }
            return pairs.SingleOrDefault();
        }

        public MerchModel? SingleOrDefault(Func<MerchModel, bool> predicate)
        {
            List<MerchModel> merches = [];
            foreach (var repository in _repositoryAdapters)
            {
                merches.AddRange(repository.Where(predicate));
            }
            return merches.SingleOrDefault();
        }

        public MerchModel? GetModel(int id)
        {
            return SingleOrDefault(m => m.Id == id);
        }

        public void Create(MerchModel entity)
        {
            foreach(var repository in _repositoryAdapters)
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

        public bool Update(MerchModel entity)
        {
            List<(MerchModel merch, IMerchSubtypeRepositoryAdapter itsRepository)> 
                merchesWithRepositories = [];
            foreach (var repository in _repositoryAdapters)
            {
                merchesWithRepositories.AddRange(repository.Where(e => e.Id ==
                entity.Id).Select(m => (m, repository)));
            }

            var merchRepositoryPair = merchesWithRepositories.SingleOrDefault();
            if(merchRepositoryPair != 
                default((MerchModel merch, IMerchSubtypeRepositoryAdapter itsRepository)))
            {
                return merchRepositoryPair.itsRepository.Update
                    (merchRepositoryPair.merch);
            }

            return false;
        }

        public bool Delete(int id)
        {
            var pair = SingleOrDefaultMerchRepositoryPair(m => m.Id == id);
            if (pair != default((MerchModel merch, IMerchSubtypeRepositoryAdapter itsRepository))
                && pair != null)
            {
                return pair.Value.repository.Delete(id);
            }
            else
                return false;
        }


        public void SaveChanges()
        {
            foreach(var repository in _repositoryAdapters)
            {
                repository.SaveChanges();
            }
        }

        public bool Any(Func<MerchModel, bool> predicate)
        {
            var list = Where_WithRepositories(predicate);
            return list.Any();
        }

    }
}
