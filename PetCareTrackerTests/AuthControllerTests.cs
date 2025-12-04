using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetCareTracker.Controllers;
using PetCareTracker.Data;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PetCareTracker.Tests
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly PetCareDbContext _context;

        public AuthControllerTests()
        {
            // In-memory DB til test
            var options = new DbContextOptionsBuilder<PetCareDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new PetCareDbContext(options);

            // Konfig mock til JWT
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "SuperSecretKey12345678901234567890"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:DurationMinutes", "60"}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _controller = new AuthController(_context, config);
        }

        [Fact]
        public async Task Register_ShouldCreateUser_WhenValidData()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                Role = "User"
            };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdUser = Assert.IsAssignableFrom<object>(okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldFail_WhenEmailExists()
        {
            // Arrange: opret først en bruger
            _context.Users.Add(new User { Name = "Existing", Email = "exists@example.com", PasswordHash = "hash", Role = "User" });
            await _context.SaveChangesAsync();

            var dto = new RegisterDTO { Name = "New User", Email = "exists@example.com", Password = "Pass123" };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email already in use.", badRequest.Value);
        }

        [Fact]
        public async Task RegisterUser_ReturnsOk()
        {
            var dto = new RegisterDTO { Name = "Bob", Email = "bob@test.com", Password = "password" };
            var result = await _controller.Register(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value!;
            Assert.Equal("Bob", value.Name);
        }


        [Fact]
        public async Task Login_ShouldReturnToken_WhenValidCredentials()
        {
            // Arrange: opret bruger
            var user = new User { Name = "Login User", Email = "login@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123"), Role = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new LoginDTO { Email = "login@example.com", Password = "Pass123" };

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenDict = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.True(tokenDict.ContainsKey("token"));
            Assert.False(string.IsNullOrEmpty(tokenDict["token"]));
        }

        [Fact]
        public async Task Login_ShouldFail_WhenInvalidCredentials()
        {
            // Arrange: bruger findes ikke
            var dto = new LoginDTO { Email = "nouser@example.com", Password = "Pass123" };

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorized.Value);
        }

        [Fact]
        public async Task LoginUser_ReturnsToken()
        {
            // Arrange
            var user = new User { Name = "Bob", Email = "bob@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new LoginDTO { Email = "bob@test.com", Password = "password" };

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenDict = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.True(tokenDict.ContainsKey("token"));
            Assert.False(string.IsNullOrEmpty(tokenDict["token"]));
        }
    }
}
