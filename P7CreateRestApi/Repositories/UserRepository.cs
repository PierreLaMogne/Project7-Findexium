using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace FindexiumAPI.Repositories
{
    public class UserRepository
    {
        public LocalDbContext DbContext { get; }

        public UserRepository(LocalDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public User FindByUserName(string userName)
        {
            return DbContext.Users.Where(user => user.UserName == userName)
                                  .FirstOrDefault();
        }

        public async Task<List<User>> FindAll()
        {
            return await DbContext.Users.ToListAsync();
        }

        public void Add(User user)
        {
        }

        public User FindById(int id)
        {
            return null;
        }
    }
}