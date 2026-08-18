using Microsoft.EntityFrameworkCore;

namespace PriceTrackerTest.Utils.TestAppBuilder.Repository
{
    public class TestDbContextFactory<SourceType, Implementation> : IDbContextFactory<SourceType>
        where SourceType : DbContext where Implementation : SourceType
    {
        private readonly IDbContextFactory<Implementation> _dbContextFactory;
        public TestDbContextFactory(IDbContextFactory<Implementation> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public SourceType CreateDbContext()
        {
            return _dbContextFactory.CreateDbContext();
        }
    }
}
