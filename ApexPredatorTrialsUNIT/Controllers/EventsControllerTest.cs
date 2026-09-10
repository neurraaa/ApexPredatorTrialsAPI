using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class EventsControllerTest
    {
        private readonly Mock<IGameEventService> _serviceMock = new();
        private readonly EventsController _controller;

        public EventsControllerTest() => _controller = new EventsController(_serviceMock.Object, NullLogger<EventsController>.Instance);

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var events = new List<GameEventDto> { new() { Id = 1, Title = "Hunter Protocol" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(events);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(events, ok.Value);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((GameEventDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetBracket_ReturnsOkWithSchedules()
        {
            var schedules = new List<GameEventScheduleDto> { new() { Id = 1, EventId = 5, Round = "Final" } };
            _serviceMock.Setup(s => s.GetBracketAsync(5)).ReturnsAsync(schedules);

            var result = await _controller.GetBracket(5);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(schedules, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new GameEventWriteDto { Title = "Hunter Protocol", Region = "EU" };
            var created = new GameEventDto { Id = 1, Title = "Hunter Protocol", Region = "EU" };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            var dto = new GameEventWriteDto { Title = "Hunter Protocol", Region = "EU" };
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
