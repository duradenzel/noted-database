using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using noted_database.Data.Repositories;
using noted_database.Hubs;
using noted_database.Models;
using noted_database.Services.Interfaces;

namespace noted_database.Services
{
    public class CampaignService : CampaignService
    {
        private readonly UserRepository _userRepository;
        private readonly CampaignRepository _campaignRepository;

        private readonly IHubContext<NotificationHub> _hubContext;

        public CampaignService(UserRepository userRepository, CampaignRepository campaignRepository, IHubContext<NotificationHub> hubContext)
        {
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
            _hubContext = hubContext;
        }

        public async Task<List<Campaign>> GetCampaignsByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                user = new User { Email = email };
                await _userRepository.AddUserAsync(user);
            }

            return await _campaignRepository.GetCampaignsByParticipantId(user.UserId);
        }

         public async Task<Campaign> GetCampaignById(int id)
        {
            return await _campaignRepository.GetCampaignById(id);
        }

        public async Task<bool> InsertCampaign(Campaign campaign)
        {
          return await _campaignRepository.InsertCampaign(campaign);
        }

        public async Task<bool> UpdateCampaign(Campaign campaign){

            await NotifyUsersAsync("Your campaign '" + campaign.Title + "' has been updated");
            return await _campaignRepository.UpdateCampaign(campaign);
        }
         public async Task<bool> DeleteCampaign(int id){
            return await _campaignRepository.DeleteCampaign(id);
        }

         public async Task NotifyUsersAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        
    }
}
