using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using noted_database.Data.Repositories;

namespace noted_database.Integration
{
    public class NotedWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<ICampaignRepository> CampaignRepositoryMock { get; }

        public NotedWebApplicationFactory()
        {
            CampaignRepositoryMock = new Mock<ICampaignRepository>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(CampaignRepositoryMock.Object);
            });
        }
    }
}
