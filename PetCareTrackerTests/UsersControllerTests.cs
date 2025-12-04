using Xunit;
using Moq;
using PetCareTracker.Controllers;
using PetCareTracker.Repositories.Interfaces;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetCareTracker.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _controller = new UsersController(_mockRepo.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetUsers_ReturnsOkWithList()
        {
            // Arrange: mock repository med alle nødvendige properties
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Name = "Alice",
                        Email = "alice@test.com",
                        Role = "User",
                        Pets = new List<Pet>() // tom liste, ikke null
                    }
                });

            // Simuler en logget-in bruger
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<UserDTO>>(okResult.Value);

            Assert.Single(list);
            Assert.Equal("Alice", list[0].Name);
            Assert.Equal("alice@test.com", list[0].Email);
            Assert.Empty(list[0].Pets); // tom liste som vi mockede
        }

        [Fact]
        public async Task GetUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Alice",
                Email = "alice@test.com",
                Role = "User",
                Pets = new List<Pet>()
            };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            // Act
            var result = await _controller.GetUser(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<UserDTO>(okResult.Value);
            Assert.Equal("Alice", dto.Name);
            Assert.Equal("alice@test.com", dto.Email);
        }

        [Fact]
        public async Task CreateUser_AsAdmin_ReturnsCreated()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(99, "Admin");

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                     .ReturnsAsync(new User { Id = 1, Name = "Alice", Email = "alice@test.com" });

            var dto = new UserDTO { Name = "Alice", Email = "alice@test.com" };
            var result = await _controller.CreateUser(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<UserDTO>(created.Value);
            Assert.Equal("Alice", returnedDto.Name);
        }

        [Fact]
        public async Task UpdateUser_AsSelf_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new User { Id = 1, Name = "OldName", Email = "old@test.com" });
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var dto = new UserDTO { Id = 1, Name = "NewName", Email = "new@test.com" };
            var result = await _controller.UpdateUser(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_AsSelf_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new User { Id = 1, Name = "DeleteMe", Email = "delete@test.com" });
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var result = await _controller.DeleteUser(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
