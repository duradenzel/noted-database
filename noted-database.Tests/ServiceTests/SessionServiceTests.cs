using Moq;
using noted_database.Data.Repositories;
using noted_database.Models;
using noted_database.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace noted_database.Tests.ServiceTests
{
    public class SessionServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ISessionRepository> _mockSessionRepository;
        private readonly SessionService _sessionService;

        public SessionServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockSessionRepository = new Mock<ISessionRepository>();
            _sessionService = new SessionService(_mockUserRepository.Object, _mockSessionRepository.Object);
        }

        [Fact]
        public async Task GetSessionsByCampaignId_ValidCampaignId_ReturnsSessions()
        {
            // Arrange
            int campaignId = 1;
            var expectedSessions = new List<Session>
            {
                new Session { SessionId = 1, CampaignId = campaignId, Date = DateTime.UtcNow, Title = "Session 1", Summary = "Summary of Session 1" },
                new Session { SessionId = 2, CampaignId = campaignId, Date = DateTime.UtcNow, Title = "Session 2", Summary = "Summary of Session 2" }
            };

            _mockSessionRepository.Setup(repo => repo.GetSessionsByCampaignId(campaignId)).ReturnsAsync(expectedSessions);

            // Act
            var result = await _sessionService.GetSessionsByCampaignId(campaignId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSessions.Count, result.Count);
            Assert.Equal(expectedSessions[0].SessionId, result[0].SessionId);
            Assert.Equal(expectedSessions[1].Title, result[1].Title);
        }

        [Fact]
        public async Task GetSessionById_ExistingId_ReturnsSession()
        {
            // Arrange
            int sessionId = 1;
            var expectedSession = new Session { SessionId = sessionId, Title = "Session 1" };

            _mockSessionRepository.Setup(repo => repo.GetSessionById(sessionId)).ReturnsAsync(expectedSession);

            // Act
            var result = await _sessionService.GetSessionById(sessionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSession.SessionId, result.SessionId);
            Assert.Equal(expectedSession.Title, result.Title);
        }

        [Fact]
        public async Task InsertSession_ValidSession_ReturnsTrue()
        {
            // Arrange
            var session = new Session { SessionId = 1, Title = "New Session" };

            _mockSessionRepository.Setup(repo => repo.InsertSession(session)).ReturnsAsync(true);

            // Act
            var result = await _sessionService.InsertSession(session);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateSession_ExistingSession_ReturnsTrue()
        {
            // Arrange
            var session = new Session { SessionId = 1, Title = "Updated Session" };

            _mockSessionRepository.Setup(repo => repo.UpdateSession(session)).ReturnsAsync(true);

            // Act
            var result = await _sessionService.UpdateSession(session);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteSession_ExistingId_ReturnsTrue()
        {
            // Arrange
            int sessionId = 1;

            _mockSessionRepository.Setup(repo => repo.DeleteSession(sessionId)).ReturnsAsync(true);

            // Act
            var result = await _sessionService.DeleteSession(sessionId);

            // Assert
            Assert.True(result);
        }
    }
}
