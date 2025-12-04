using Xunit;
using Moq;
using PetCareTracker.Controllers;
using PetCareTracker.Repositories.Interfaces;
using PetCareTracker.DTOs;
using PetCareTracker.Models;
using PetCareTracker.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PetCareTracker.Tests
{
    public class CareInstructionsControllerTests
    {
        private readonly Mock<ICareInstructionRepository> _mockRepo;
        private readonly Mock<IPetRepository> _mockPetRepo;
        private readonly CareInstructionsController _controller;

        public CareInstructionsControllerTests()
        {
            _mockRepo = new Mock<ICareInstructionRepository>();
            _mockPetRepo = new Mock<IPetRepository>();
            _controller = new CareInstructionsController(_mockRepo.Object, _mockPetRepo.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetCareInstructions_ReturnsOkWithList()
        {
            // Arrange
            var instructions = new List<CareInstruction>
            {
                new CareInstruction { Id = 1, PetId = 1, FoodAmountPerDay = 5, FoodType = "Dry Food", Likes = "Playing", Dislikes = "Loud Noise", Notes = "None" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(instructions);

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            // Act
            var result = await _controller.GetCareInstructions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<CareInstructionDTO>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal(5, list[0].FoodAmountPerDay);
        }

        [Fact]
        public async Task GetCareInstruction_ReturnsOk_WhenExists()
        {
            // Arrange
            var instruction = new CareInstruction
            {
                Id = 1,
                PetId = 1,
                FoodAmountPerDay = 5,
                FoodType = "Dry Food",
                Likes = "Playing",
                Dislikes = "Loud Noise",
                Notes = "None"
            };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(instruction);
            _mockPetRepo.Setup(r => r.GetByIdAsync(1))
                        .ReturnsAsync(new Pet { Id = 1, OwnerId = 1 }); // tjek ejerskab

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            // Act
            var result = await _controller.GetCareInstruction(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<CareInstructionDTO>(okResult.Value);
            Assert.Equal(5, dto.FoodAmountPerDay);
        }

        [Fact]
        public async Task CreateCareInstruction_NotOwner_ReturnsForbid()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            _mockPetRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Pet { Id = 2, OwnerId = 2 });

            var dto = new CareInstructionDTO { PetId = 2, FoodAmountPerDay = 10 };
            var result = await _controller.CreateCareInstruction(dto);
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task CreateCareInstruction_AsOwner_ReturnsCreated()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            _mockPetRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Pet { Id = 1, OwnerId = 1 });
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<CareInstruction>()))
             .ReturnsAsync(new CareInstruction { Id = 1, PetId = 1, FoodAmountPerDay = 10, FoodType = "Dry" });

            var dto = new CareInstructionDTO { PetId = 1, FoodAmountPerDay = 10 };
            var result = await _controller.CreateCareInstruction(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<CareInstructionDTO>(created.Value);
            Assert.Equal(10, returnedDto.FoodAmountPerDay);
            Assert.Equal(1, returnedDto.PetId);
        }

        [Fact]
        public async Task UpdateCareInstruction_AsOwner_ReturnsNoContent()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var careInstruction = new CareInstruction
            {
                Id = 1,
                PetId = 1,
                FoodAmountPerDay = 5,
                Pet = new Pet { Id = 1, OwnerId = 1 } // vigtig for ejerskabstjek
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(careInstruction);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<CareInstruction>())).Returns(Task.CompletedTask);

            var dto = new CareInstructionDTO { Id = 1, PetId = 1, FoodAmountPerDay = 15 };
            var result = await _controller.UpdateCareInstruction(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCareInstruction_AsOwner_ReturnsNoContent()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var careInstruction = new CareInstruction
            {
                Id = 1,
                PetId = 1,
                Pet = new Pet { Id = 1, OwnerId = 1 } // vigtig for ejerskabstjek
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(careInstruction);
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<CareInstruction>())).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCareInstruction(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
