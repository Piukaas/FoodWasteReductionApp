using FoodWasteReduction.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteReduction.Tests.Repositories
{
    public abstract class RepositoryTestBase : IDisposable
    {
        protected readonly ApplicationDbContext Context;

        protected RepositoryTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
