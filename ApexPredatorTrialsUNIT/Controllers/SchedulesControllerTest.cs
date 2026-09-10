using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class SchedulesControllerTest
    {
        private readonly Mock<IGameEventScheduleService> _serviceMock = new();
        private readonly SchedulesController _controller;

        public SchedulesControllerTest() => _controller = new SchedulesController(_serviceMock.Object, NullLogger<SchedulesController>.Instance);

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var schedules = new List<GameEventScheduleDto> { new() { Id = 1, EventId = 1, Round = "Final" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(schedules);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(schedules, ok.Value);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((GameEventScheduleDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new GameEventScheduleWriteDto { EventId = 1, Round = "Quarter-Final" };
            var created = new GameEventScheduleDto { Id = 1, EventId = 1, Round = "Quarter-Final" };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            var dto = new GameEventScheduleWriteDto { EventId = 1, Round = "Final" };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync(false);

            var result = await _controller.Update(99, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Existing_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
