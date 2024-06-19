using noted_database.Data.Repositories;
using noted_database.Models;

namespace noted_database.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;

        public UserService(IUserRepository userRepository, ICampaignRepository campaignRepository)
        {
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }
    }
}
