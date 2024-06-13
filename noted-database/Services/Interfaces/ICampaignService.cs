using noted_database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace noted_database.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetCampaignsByEmail(string email);
        Task<Campaign> GetCampaignById(int id);
        Task<bool> InsertCampaign(Campaign campaign);
        Task<bool> UpdateCampaign(Campaign campaign);
        Task<bool> DeleteCampaign(int id);
        Task NotifyUsersAsync(string message);
    }
}