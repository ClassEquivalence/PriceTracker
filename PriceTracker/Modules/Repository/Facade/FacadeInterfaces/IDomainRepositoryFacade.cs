using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.Repository.Facade.FacadeInterfaces
{
    public interface IDomainRepositoryFacade<Domain>
        where Domain : DomainDto
    {
        public List<Domain> Where(Func<Domain, bool> predicate);

        public List<Domain> GetAll();

        public Domain? SingleOrDefault(Func<Domain, bool> predicate);

        public void Create(Domain entity);

        public bool Update(Domain entity);

        public bool Delete(int id);

        public bool Any(Func<Domain, bool> predicate);

        public Domain? GetModel(int id);

    }
}
