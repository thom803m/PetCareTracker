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
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User> { new User { Id = 1, Name = "Alice" } });

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var result = await _controller.GetUsers();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<UserDTO>>(okResult.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task UpdateUser_NotOwner_ReturnsForbid()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var dto = new UserDTO { Id = 2, Name = "Bob", Email = "bob@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new User { Id = 2, Name = "Bob" });

            var result = await _controller.UpdateUser(2, dto);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateUser_AsOwner_ReturnsNoContent()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var dto = new UserDTO { Id = 1, Name = "Alice", Email = "alice@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Name = "Alice" });
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateUser(1, dto);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
