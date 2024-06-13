using noted_database.Models;
using System.Threading.Tasks;

namespace noted_database.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task AddUserAsync(User user);
    }
}
