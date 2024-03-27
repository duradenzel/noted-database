using noted_database.Data.Repositories;
using noted_database.Models;

namespace noted_database.Services
{
    public class CampaignService
    {
        private readonly UserRepository _userRepository;
        private readonly CampaignRepository _campaignRepository;

        public CampaignService(UserRepository userRepository, CampaignRepository campaignRepository)
        {
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
        }

        public async Task<List<Campaign>> GetCampaignsByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User { Email = email };
                await _userRepository.AddUserAsync(user);
            }

            return await _campaignRepository.GetCampaignsByParticipantIdAsync(user.UserId);
        }
    }
}
