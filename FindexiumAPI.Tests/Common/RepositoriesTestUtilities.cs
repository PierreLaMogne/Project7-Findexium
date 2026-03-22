using FindexiumAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FindexiumAPI.Tests.Common
{
    public class LocalDbFixture : IDisposable
    {
        public LocalDbContext Context { get; }
        private readonly string _dbName;

        // Each test class gets a unique in-memory database instance to ensure isolation and prevent data leakage between tests.
        public LocalDbFixture()
        {
            _dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;
            Context = new LocalDbContext(options);
            Context.Database.EnsureCreated();
        }
        
        public void Dispose() => Context.Dispose();
    }
}
