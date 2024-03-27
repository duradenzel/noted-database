using noted_database.Models;
using Microsoft.EntityFrameworkCore;

namespace noted_database.Data.Repositories
{
    public class CampaignRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CampaignRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Campaign>> GetCampaignsByParticipantIdAsync(int userId)
        {
            var campaignIds = await _dbContext.CampaignParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.CampaignId)
                .ToListAsync();

            return await _dbContext.Campaigns
                .Where(c => campaignIds.Contains(c.CampaignId))
                .ToListAsync();
        }
    }
}
