using noted_database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Diagnostics;

namespace noted_database.Data.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly bool _isTestEnvironment;
        public CampaignRepository(ApplicationDbContext dbContext, bool isTestEnvironment = false)
        {
            _dbContext = dbContext;
            _isTestEnvironment = isTestEnvironment;

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
            if (!_isTestEnvironment)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Campaigns.Add(campaign);
                await _dbContext.SaveChangesAsync();

                var participant = new CampaignParticipant
                {
                    CampaignId = campaign.CampaignId,
                    UserId = campaign.DmId,
                    IsDm = true
                };
                _dbContext.CampaignParticipants.Add(participant);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
           
            _dbContext.Campaigns.Add(campaign);
            await _dbContext.SaveChangesAsync();

            var participant = new CampaignParticipant
            {
                CampaignId = campaign.CampaignId,
                UserId = campaign.DmId,
                IsDm = true
            };
            _dbContext.CampaignParticipants.Add(participant);
            await _dbContext.SaveChangesAsync();

            return true;
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

        public async Task<bool> DeleteCampaign(int id)
        {
            try
            {
                
                var existingCampaign = await _dbContext.Campaigns.FindAsync(id);

                if (existingCampaign == null)
                {
                    return false; 
                }

              
                _dbContext.Campaigns.Remove(existingCampaign);
                await _dbContext.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {

                Console.WriteLine("An error occurred while deleting the campaign: " + ex.Message);
                return false; 
            }
        }

    }
}
