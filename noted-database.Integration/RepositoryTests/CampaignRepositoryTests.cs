using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using noted_database.Models;
using noted_database.Data.Repositories;
using noted_database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace noted_database.Integration.ControllerTests
{
    public class CampaignRepositoryTests : IDisposable
    {
        private ApplicationDbContext _dbContext;
        private ICampaignRepository _campaignRepository;

        public CampaignRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            PopulateFakeData();
            _campaignRepository = new CampaignRepository(_dbContext);
        }

        public void PopulateFakeData()
        {
            var campaigns = new[]
            {
                new Campaign { CampaignId = 1, Title = "Test Campaign1", Description = "Test Description1", MaxPlayers = 3, DmId = 1 },
                new Campaign { CampaignId = 2, Title = "Test Campaign2", Description = "Test Description2", MaxPlayers = 4, DmId = 2 },
                new Campaign { CampaignId = 3, Title = "Test Campaign3", Description = "Test Description3", MaxPlayers = 5, DmId = 3 }
            };

            var campaignParticipants = new[]
            {
                new CampaignParticipant { CampaignId = 1, UserId = 1, IsDm = true },
                new CampaignParticipant { CampaignId = 2, UserId = 1, IsDm = false },
                new CampaignParticipant { CampaignId = 3, UserId = 2, IsDm = true }
            };

            _dbContext.Campaigns.AddRange(campaigns);
            _dbContext.CampaignParticipants.AddRange(campaignParticipants);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetCampaignsByParticipantId_ReturnsCorrectCampaigns()
        {
            // Act
            var campaigns = await _campaignRepository.GetCampaignsByParticipantId(1);

            // Assert
            Assert.Equal(2, campaigns.Count);
            Assert.Contains(campaigns, c => c.CampaignId == 1);
            Assert.Contains(campaigns, c => c.CampaignId == 2);
        }

        [Fact]
        public async Task GetCampaignById_ReturnsCorrectCampaign()
        {
            // Act
            var campaign = await _campaignRepository.GetCampaignById(1);

            // Assert
            Assert.NotNull(campaign);
            Assert.Equal("Test Campaign1", campaign.Title);
        }

        [Fact]
        public async Task InsertCampaign_AddsNewCampaign()
        {
            // Arrange
            var newCampaign = new Campaign { CampaignId = 4, Title = "New Campaign", Description = "New Description", MaxPlayers = 6, DmId = 4 };

            // Act
            var result = await _campaignRepository.InsertCampaign(newCampaign);

            // Assert
            Assert.True(result);
            Assert.Equal(4, _dbContext.Campaigns.Count());
            Assert.NotNull(await _dbContext.Campaigns.FindAsync(4));
            Assert.NotNull(await _dbContext.CampaignParticipants.FirstOrDefaultAsync(cp => cp.CampaignId == 4 && cp.UserId == 4 && cp.IsDm));
        }

        [Fact]
        public async Task UpdateCampaign_UpdatesExistingCampaign()
        {
            // Arrange
            var updatedCampaign = new Campaign { CampaignId = 1, Title = "Updated Campaign1", Description = "Updated Description1", MaxPlayers = 7 };

            // Act
            var result = await _campaignRepository.UpdateCampaign(updatedCampaign);

            // Assert
            Assert.True(result);
            var campaign = await _dbContext.Campaigns.FindAsync(1);
            Assert.Equal("Updated Campaign1", campaign.Title);
            Assert.Equal("Updated Description1", campaign.Description);
            Assert.Equal(7, campaign.MaxPlayers);
        }

        [Fact]
        public async Task DeleteCampaign_DeletesExistingCampaign()
        {
            // Act
            var result = await _campaignRepository.DeleteCampaign(1);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.Campaigns.FindAsync(1));
            Assert.Equal(2, _dbContext.Campaigns.Count());
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
