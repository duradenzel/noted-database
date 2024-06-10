using noted_database.Data.Repositories;
using noted_database.Models;

namespace noted_database.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly CampaignRepository _campaignRepository;

        public UserService(UserRepository userRepository, CampaignRepository campaignRepository)
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
