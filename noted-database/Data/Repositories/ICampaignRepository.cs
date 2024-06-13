using noted_database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace noted_database.Data.Repositories
{
    public interface ICampaignRepository
    {
        Task<List<Campaign>> GetCampaignsByParticipantId(int userId);
        Task<Campaign> GetCampaignById(int id);
        Task<bool> InsertCampaign(Campaign campaign);
        Task<bool> UpdateCampaign(Campaign campaign);
        Task<bool> DeleteCampaign(int id);
    }
}