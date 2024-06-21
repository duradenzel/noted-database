using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using noted_database.Models;
using noted_database.Data.Repositories;
using noted_database.Data;
using Microsoft.EntityFrameworkCore;

namespace noted_database.Integration.ControllerTests
{
    public class SessionRepositoryTests : IDisposable
    {
        private ApplicationDbContext _dbContext;
        private ISessionRepository _sessionRepository;

        public SessionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            PopulateFakeData();
            _sessionRepository = new SessionRepository(_dbContext);
        }

        public void PopulateFakeData()
        {
            var campaigns = new[]
            {
                new Campaign { CampaignId = 1, Title = "Test Campaign1", Description = "Test Description1", MaxPlayers = 3, DmId = 1 },
                new Campaign { CampaignId = 2, Title = "Test Campaign2", Description = "Test Description2", MaxPlayers = 4, DmId = 2 },
                new Campaign { CampaignId = 3, Title = "Test Campaign3", Description = "Test Description3", MaxPlayers = 5, DmId = 3 }
            };

            var sessions = new[]
            {
                new Session { SessionId = 1, CampaignId = 1, Title = "Session 1", Summary = "Summary 1" },
                new Session { SessionId = 2, CampaignId = 1, Title = "Session 2", Summary = "Summary 2" },
                new Session { SessionId = 3, CampaignId = 2, Title = "Session 3", Summary = "Summary 3" }
            };

            _dbContext.Campaigns.AddRange(campaigns);
            _dbContext.Sessions.AddRange(sessions);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetSessionsByCampaignId_ReturnsCorrectSessions()
        {
            // Act
            var sessions = await _sessionRepository.GetSessionsByCampaignId(1);

            // Assert
            Assert.Equal(2, sessions.Count);
            Assert.Contains(sessions, s => s.SessionId == 1);
            Assert.Contains(sessions, s => s.SessionId == 2);
        }

        [Fact]
        public async Task GetSessionById_ReturnsCorrectSession()
        {
            // Act
            var session = await _sessionRepository.GetSessionById(1);

            // Assert
            Assert.NotNull(session);
            Assert.Equal("Session 1", session.Title);
        }

        [Fact]
        public async Task InsertSession_AddsNewSession()
        {
            // Arrange
            var newSession = new Session { SessionId = 4, CampaignId = 3, Title = "New Session", Summary = "New Summary" };

            // Act
            var result = await _sessionRepository.InsertSession(newSession);

            // Assert
            Assert.True(result);
            Assert.Equal(4, _dbContext.Sessions.Count());
            Assert.NotNull(await _dbContext.Sessions.FindAsync(4));
        }

        [Fact]
        public async Task UpdateSession_UpdatesExistingSession()
        {
            // Arrange
            var updatedSession = new Session { SessionId = 1, CampaignId = 1, Title = "Updated Session 1", Summary = "Updated Summary 1" };

            // Act
            var result = await _sessionRepository.UpdateSession(updatedSession);

            // Assert
            Assert.True(result);
            var session = await _dbContext.Sessions.FindAsync(1);
            Assert.Equal("Updated Session 1", session.Title);
            Assert.Equal("Updated Summary 1", session.Summary);
        }

        [Fact]
        public async Task DeleteSession_DeletesExistingSession()
        {
            // Act
            var result = await _sessionRepository.DeleteSession(1);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.Sessions.FindAsync(1));
            Assert.Equal(2, _dbContext.Sessions.Count());
        }

        [Fact]
        public async Task UpdateSession_FailsForNonExistingSession()
        {
            // Arrange
            var updatedSession = new Session { SessionId = 999, CampaignId = 1, Title = "Non Existing Session", Summary = "Non Existing Summary" };

            // Act
            var result = await _sessionRepository.UpdateSession(updatedSession);

            // Assert
            Assert.False(result); 
        }

        [Fact]
        public async Task DeleteSession_FailsForNonExistingSession()
        {
            // Act
            var result = await _sessionRepository.DeleteSession(999);

            // Assert
            Assert.False(result); 
        }

        [Fact]
        public async Task InsertSession_FailsForDuplicateSessionId()
        {
            // Arrange
            var newSession = new Session { SessionId = 1, CampaignId = 3, Title = "Duplicate Session", Summary = "Duplicate Summary" };

            // Act
            var result = await _sessionRepository.InsertSession(newSession);

            // Assert
            Assert.False(result); 
        }


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
