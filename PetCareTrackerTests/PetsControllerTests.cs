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

        // PUT ok
        [Fact]
        public async Task UpdatePet_AsOwner_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Pet { Id = 1, OwnerId = 1 });

            var result = await _controller.UpdatePet(1, new PetDTO { Id = 1, OwnerId = 1, Name = "New", Type = "Dog", Breed = "Lab", Age = 3 });
            Assert.IsType<NoContentResult>(result);
        }

        // DELETE ok
        [Fact]
        public async Task DeletePet_AsOwner_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Pet { Id = 1, OwnerId = 1 });

            var result = await _controller.DeletePet(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}