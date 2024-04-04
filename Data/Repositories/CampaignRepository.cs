using noted_database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Diagnostics;

namespace noted_database.Data.Repositories
{
    public class CampaignRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CampaignRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Campaign>> GetCampaignsByParticipantId(int userId)
        {
            var campaignIds = await _dbContext.CampaignParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.CampaignId)
                .ToListAsync();

            return await _dbContext.Campaigns
                .Where(c => campaignIds.Contains(c.CampaignId))
                .ToListAsync();
        }

         public async Task<Campaign> GetCampaignById(int id)
        {
            return await _dbContext.Campaigns.FirstOrDefaultAsync(c => c.CampaignId == id);
        }
      
        public async Task<bool> InsertCampaign(Campaign campaign)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Campaigns.Add(campaign);
                    await _dbContext.SaveChangesAsync();

                    CampaignParticipant dmParticipant = new CampaignParticipant
                    {
                        CampaignId = campaign.CampaignId,
                        UserId = campaign.DmId,
                        IsDm = true
                    };

                    _dbContext.CampaignParticipants.Add(dmParticipant);
                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

       public async Task<bool> UpdateCampaign(Campaign campaign)
        {
            try
            {
                var existingCampaign = await _dbContext.Campaigns.FirstOrDefaultAsync(c => c.CampaignId == campaign.CampaignId);

                if (existingCampaign == null)
                {
                    return false; 
                }

                existingCampaign.Title = campaign.Title;
                existingCampaign.Description = campaign.Description;
                existingCampaign.MaxPlayers = campaign.MaxPlayers;

                await _dbContext.SaveChangesAsync();
                
                return true; 
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update error: " + ex.Message);
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                return false; 
            }
        }

    }
}
