using Moq;
using noted_database.Data.Repositories;
using noted_database.Models;
using noted_database.Services;
using System.Threading.Tasks;
using Xunit;

namespace noted_database.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _userService = new UserService(_mockUserRepository.Object, _mockCampaignRepository.Object);
        }

        [Fact]
        public async Task GetUserByEmail_ValidEmail_ReturnsUser()
        {
            // Arrange
            var email = "testuser@outlook.com";
            var expectedUser = new User { UserId = 1, Email = email }; // Mock user object

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
            Assert.Equal(expectedUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmail_InvalidEmail_ReturnsNull()
        {
            // Arrange
            var email = "testuser@outlook.com";

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email)).ReturnsAsync(() => null);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            Assert.Null(result);
        }
    }
}
