using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using noted_database.Models;
using Newtonsoft.Json;

namespace noted_database.Integration.ControllerTests
{
    public class CampaignControllerTests : IDisposable
    {
        private readonly NotedWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CampaignControllerTests()
        {
            _factory = new NotedWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetCampaign_ReturnsSuccessStatusCode()
        {
            // Arrange
            var campaign = new Campaign { CampaignId = 1, Title = "Test Campaign" };
            _factory.CampaignRepositoryMock.Setup(repo => repo.GetCampaignById(1))
                .Returns(campaign);

            // Act
            var response = await _client.GetAsync("/campaigns/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public void Dispose()
        {
            _factory.Dispose();
            _client.Dispose();
        }
    }
}
