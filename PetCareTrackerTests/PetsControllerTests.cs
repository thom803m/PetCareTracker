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
    public class PetsControllerTests
    {
        private readonly Mock<IPetRepository> _mockRepo;
        private readonly PetsController _controller;

        public PetsControllerTests()
        {
            _mockRepo = new Mock<IPetRepository>();
            _controller = new PetsController(_mockRepo.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetPets_ReturnsOkWithList()
        {
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Pet> { new Pet { Id = 1, Name = "Fido", OwnerId = 1 } });

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var result = await _controller.GetPets();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<PetDTO>>(okResult.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task CreatePet_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var dto = new PetDTO();
            var result = await _controller.CreatePet(dto);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreatePet_NotOwner_ReturnsForbid()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var dto = new PetDTO { OwnerId = 2, Name = "Fido", Type = "Dog", Breed = "Beagle", ImageUrl = "", Age = 1, Notes = "" };
            var result = await _controller.CreatePet(dto);
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetPet_ReturnsOk()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser(1, "User");
            _controller.ControllerContext.HttpContext.User = user;

            var pet = new Pet
            {
                Id = 1,
                Name = "Fido",
                Type = "Dog",
                Breed = "Beagle",
                Age = 2,
                Notes = "Happy dog",
                OwnerId = 1,
                ImageUrl = "image.jpg"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pet);

            // Act
            var result = await _controller.GetPet(1);

            // Assert
            var okResult = Assert.IsType<ActionResult<PetDTO>>(result);
            var dto = Assert.IsType<PetDTO>(okResult.Value);
            Assert.Equal("Fido", dto.Name);
            Assert.Equal(1, dto.OwnerId);
        }

        [Fact]
        public async Task CreatePet_AsOwner_ReturnsCreated()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var dto = new PetDTO { OwnerId = 1, Name = "Fido", Type = "Dog", Breed = "Beagle", ImageUrl = "", Age = 1, Notes = "" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Pet>()))
             .ReturnsAsync(new Pet { Id = 1, OwnerId = 1, Name = "Fido", Type = "Dog", Breed = "Beagle", Age = 1 });

            var result = await _controller.CreatePet(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<PetDTO>(created.Value);
            Assert.Equal("Fido", returnedDto.Name);
        }

        [Fact]
        public async Task UpdatePet_AsOwner_ReturnsNoContent()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Pet { Id = 1, OwnerId = 1, Name = "OldName" });
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);

            var dto = new PetDTO { Id = 1, OwnerId = 1, Name = "NewName", Type = "Dog", Breed = "Beagle", ImageUrl = "", Age = 2, Notes = "" };

            // Act
            var result = await _controller.UpdatePet(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePet_AsOwner_ReturnsNoContent()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Pet { Id = 1, OwnerId = 1 });
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePet(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}