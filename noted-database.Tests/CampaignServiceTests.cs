using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using noted_database.Data.Repositories;
using noted_database.Hubs;
using noted_database.Models;
using noted_database.Services;
using Xunit;
using Microsoft.AspNetCore.SignalR;

namespace noted_database.Tests
{
    public class CampaignServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<IHubContext<NotificationHub>> _mockHubContext;
        private readonly CampaignService _campaignService;

        public CampaignServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _campaignService = new CampaignService(
                _mockUserRepository.Object,
                _mockCampaignRepository.Object,
                _mockHubContext.Object
            );
        }

        [Fact]
        public async Task GetCampaignsByEmail_UserExists_ReturnsCampaigns()
        {
            // Arrange
            var email = "duradenzel13@gmail.com";
            var user = new User { UserId = 1, Email = email };
            var campaigns = new List<Campaign> { new Campaign { CampaignId = 1, Title = "Test Campaign" } };

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(user);
            _mockCampaignRepository.Setup(repo => repo.GetCampaignsByParticipantId(user.UserId)).ReturnsAsync(campaigns);

            // Act
            var result = await _campaignService.GetCampaignsByEmail(email);

            // Assert
            Assert.Equal(campaigns, result);
        }
    }
}
