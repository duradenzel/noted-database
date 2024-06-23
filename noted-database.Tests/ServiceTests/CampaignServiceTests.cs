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
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly CampaignService _campaignService;

        public CampaignServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            // Setup Clients.All to return the mocked IClientProxy
            _mockClients.Setup(clients => clients.All).Returns(_mockClientProxy.Object);
            _mockHubContext.Setup(hub => hub.Clients).Returns(_mockClients.Object);

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
            var email = "testurer@gmail.com";
            var user = new User { UserId = 1, Email = email };
            var campaigns = new List<Campaign> { new Campaign { CampaignId = 1, Title = "Test Campaign", Description="Test Description", MaxPlayers= 1, DmId=user.UserId } };
            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(user);
            _mockCampaignRepository.Setup(repo => repo.GetCampaignsByParticipantId(user.UserId)).ReturnsAsync(campaigns);

            // Act
            var result = await _campaignService.GetCampaignsByEmail(email);

            // Assert
            Assert.Equal(campaigns, result);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockCampaignRepository.Verify(repo => repo.GetCampaignsByParticipantId(user.UserId), Times.Once);
        }

        [Fact]
        public async Task GetCampaignsByEmail_UserDoesNotExist_AddsUserAndReturnsEmptyList()
        {
            // Arrange
            var email = "testurer@gmail.com";
            User user = null; 
            var newUser = new User { UserId = 2, Email = email };
            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.AddUserAsync(It.IsAny<User>())).Callback<User>(u => u.UserId = newUser.UserId).Returns(Task.CompletedTask);
            _mockCampaignRepository.Setup(repo => repo.GetCampaignsByParticipantId(newUser.UserId)).ReturnsAsync(new List<Campaign>());

            // Act
            var result = await _campaignService.GetCampaignsByEmail(email);

            // Assert

            Assert.Empty(result);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockUserRepository.Verify(repo => repo.AddUserAsync(It.Is<User>(u => u.Email == email)), Times.Once);
            _mockCampaignRepository.Verify(repo => repo.GetCampaignsByParticipantId(newUser.UserId), Times.Never);
        }

        [Fact]
        public async Task GetCampaignsByEmail_UserExists_NoCampaigns_ReturnsEmptyList()
        {
            // Arrange
            var email = "testurer@outlook.com";
            var user = new User { UserId = 3, Email = email };
            var campaigns = new List<Campaign>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(user);
            _mockCampaignRepository.Setup(repo => repo.GetCampaignsByParticipantId(user.UserId)).ReturnsAsync(campaigns);

            // Act
            var result = await _campaignService.GetCampaignsByEmail(email);

            // Assert
            Assert.Equal(campaigns, result);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockCampaignRepository.Verify(repo => repo.GetCampaignsByParticipantId(user.UserId), Times.Once);
        }

        [Fact]
        public async Task GetCampaignById_CampaignExists_ReturnsCampaign()
        {
            // Arrange
            var campaignId = 1;
            var campaign = new Campaign { CampaignId = campaignId, Title = "Test Campaign" };

            _mockCampaignRepository.Setup(repo => repo.GetCampaignById(campaignId)).ReturnsAsync(campaign);

            // Act
            var result = await _campaignService.GetCampaignById(campaignId);

            // Assert
            Assert.Equal(campaign, result);
            _mockCampaignRepository.Verify(repo => repo.GetCampaignById(campaignId), Times.Once);
        }

        [Fact]
        public async Task GetCampaignById_CampaignDoesNotExist_ReturnsNull()
        {
            // Arrange
            var campaignId = 2;
            Campaign campaign = null; // Campaign does not exist

            _mockCampaignRepository.Setup(repo => repo.GetCampaignById(campaignId)).ReturnsAsync(campaign);

            // Act
            var result = await _campaignService.GetCampaignById(campaignId);

            // Assert
            Assert.Null(result);
            _mockCampaignRepository.Verify(repo => repo.GetCampaignById(campaignId), Times.Once);
        }

        [Fact]
        public async Task InsertCampaign_Success_ReturnsTrue()
        {
            // Arrange
            var campaign = new Campaign { CampaignId = 1, Title = "New Campaign" };

            _mockCampaignRepository.Setup(repo => repo.InsertCampaign(campaign)).ReturnsAsync(true);

            // Act
            var result = await _campaignService.InsertCampaign(campaign);

            // Assert
            Assert.True(result);
            _mockCampaignRepository.Verify(repo => repo.InsertCampaign(campaign), Times.Once);
        }

        [Fact]
        public async Task InsertCampaign_Failure_ReturnsFalse()
        {
            // Arrange
            var campaign = new Campaign { CampaignId = 2, Title = "Failed Campaign" };

            _mockCampaignRepository.Setup(repo => repo.InsertCampaign(campaign)).ReturnsAsync(false);

            // Act
            var result = await _campaignService.InsertCampaign(campaign);

            // Assert
            Assert.False(result);
            _mockCampaignRepository.Verify(repo => repo.InsertCampaign(campaign), Times.Once);
        }

        [Fact]
        public async Task UpdateCampaign_Success_SendsNotificationAndReturnsTrue()
        {
            // Arrange
            var campaign = new Campaign { CampaignId = 1, Title = "Updated Campaign" };
            _mockCampaignRepository.Setup(repo => repo.UpdateCampaign(campaign)).ReturnsAsync(true);

            // Act
            var result = await _campaignService.UpdateCampaign(campaign);

            // Assert
            Assert.True(result);
            _mockCampaignRepository.Verify(repo => repo.UpdateCampaign(campaign), Times.Once);

            // Verify notification was sent
            _mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                    It.Is<object[]>(o => o != null && o.Length == 1 && (string)o[0] == "Your campaign 'Updated Campaign' has been updated"),
                    default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCampaign_Failure_ReturnsFalse()
        {
            // Arrange
            var campaign = new Campaign { CampaignId = 2, Title = "Failed Update Campaign" };

            _mockCampaignRepository.Setup(repo => repo.UpdateCampaign(campaign)).ReturnsAsync(false);

            // Act
            var result = await _campaignService.UpdateCampaign(campaign);

            // Assert
            Assert.False(result);
            _mockCampaignRepository.Verify(repo => repo.UpdateCampaign(campaign), Times.Once);

            // Verify no notification was sent on failure
            _mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }


        [Fact]
        public async Task DeleteCampaign_Success_ReturnsTrue()
        {
            // Arrange
            var campaignId = 1;

            _mockCampaignRepository.Setup(repo => repo.DeleteCampaign(campaignId)).ReturnsAsync(true);

            // Act
            var result = await _campaignService.DeleteCampaign(campaignId);

            // Assert
            Assert.True(result);
            _mockCampaignRepository.Verify(repo => repo.DeleteCampaign(campaignId), Times.Once);
        }

        [Fact]
        public async Task DeleteCampaign_Failure_ReturnsFalse()
        {
            // Arrange
            var campaignId = 2;

            _mockCampaignRepository.Setup(repo => repo.DeleteCampaign(campaignId)).ReturnsAsync(false);

            // Act
            var result = await _campaignService.DeleteCampaign(campaignId);

            // Assert
            Assert.False(result);
            _mockCampaignRepository.Verify(repo => repo.DeleteCampaign(campaignId), Times.Once);
        }

       

    }
}
