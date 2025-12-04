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
    public class CarePeriodsControllerTests
    {
        private readonly Mock<ICarePeriodRepository> _mockRepo;
        private readonly Mock<IPetRepository> _mockPetRepo;
        private readonly CarePeriodsController _controller;

        public CarePeriodsControllerTests()
        {
            _mockRepo = new Mock<ICarePeriodRepository>();
            _mockPetRepo = new Mock<IPetRepository>();
            _controller = new CarePeriodsController(_mockRepo.Object, _mockPetRepo.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetCarePeriods_ReturnsOkWithList()
        {
            // Arrange
            var periods = new List<CarePeriod>
            {
                new CarePeriod { Id = 1, PetId = 1, Status = "Active", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1) }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(periods);

            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            // Act
            var result = await _controller.GetCarePeriods();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsType<List<CarePeriodDTO>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal(1, list[0].PetId);
        }


        [Fact]
        public async Task CreateCarePeriod_NotOwner_ReturnsForbid()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");
            _mockPetRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Pet { Id = 2, OwnerId = 2 });

            var dto = new CarePeriodDTO { PetId = 2, StartDate = System.DateTime.UtcNow, EndDate = System.DateTime.UtcNow.AddDays(1) };
            var result = await _controller.CreateCarePeriod(dto);
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task CreateCarePeriod_AsOwner_ReturnsCreated()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");
            _mockPetRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Pet { Id = 1, OwnerId = 1 });
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<CarePeriod>()))
             .ReturnsAsync(new CarePeriod { Id = 1, PetId = 1, StartDate = System.DateTime.UtcNow, EndDate = System.DateTime.UtcNow.AddDays(1) });

            var dto = new CarePeriodDTO { PetId = 1, StartDate = System.DateTime.UtcNow, EndDate = System.DateTime.UtcNow.AddDays(1) };
            var result = await _controller.CreateCarePeriod(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<CarePeriodDTO>(created.Value);
            Assert.Equal(1, returnedDto.PetId);
        }

        [Fact]
        public async Task UpdateCarePeriod_AsOwner_ReturnsNoContent()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var carePeriod = new CarePeriod
            {
                Id = 1,
                PetId = 1,
                Pet = new Pet { Id = 1, OwnerId = 1 } // vigtig for ejerskabstjek
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(carePeriod);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<CarePeriod>())).Returns(Task.CompletedTask);

            var dto = new CarePeriodDTO { Id = 1, PetId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(2) };
            var result = await _controller.UpdateCarePeriod(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCarePeriod_AsOwner_ReturnsNoContent()
        {
            _controller.ControllerContext.HttpContext.User = TestHelpers.CreateTestUser(1, "User");

            var carePeriod = new CarePeriod
            {
                Id = 1,
                PetId = 1,
                Pet = new Pet { Id = 1, OwnerId = 1 } // vigtig for ejerskabstjek
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(carePeriod);
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<CarePeriod>())).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCarePeriod(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
