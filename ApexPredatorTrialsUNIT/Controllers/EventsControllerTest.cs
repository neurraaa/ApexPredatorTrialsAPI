using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class EventsControllerTest
    {
        private readonly Mock<IGameEventService> _serviceMock = new();
        private readonly EventsController _controller;

        public EventsControllerTest()
        {
            _controller = new EventsController(_serviceMock.Object, NullLogger<EventsController>.Instance);

            var identity = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, "1") });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

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
            var dto = new GameEventCreateDto { Title = "Hunter Protocol", Region = "EU", StartingRound = "Quarter-Final" };
            var created = new GameEventDto { Id = 1, Title = "Hunter Protocol", Region = "EU" };
            _serviceMock.Setup(s => s.CreateAsync(dto, 1)).ReturnsAsync(ServiceResult<GameEventDto>.Ok(created));

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task Create_InvalidStartingRound_ReturnsBadRequest()
        {
            var dto = new GameEventCreateDto { Title = "Hunter Protocol", Region = "EU", StartingRound = "Final" };
            _serviceMock.Setup(s => s.CreateAsync(dto, 1))
                .ReturnsAsync(ServiceResult<GameEventDto>.Invalid("Starting round must be Quarter-Final or Semi-Final."));

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AdvancePhase_Valid_ReturnsOk()
        {
            var dto = new AdvancePhaseDto
            {
                NextRound = "Semi-Final",
                Winners = new() { new ScheduleWinnerDto { ScheduleId = 1, WinnerPlayerId = 1 } },
                NextMatchups = new() { new NextMatchupDto { HunterScheduleId = 1, HumanScheduleId = 1 } }
            };
            var updated = new GameEventDto { Id = 5, CurrentRound = "Semi-Final" };
            _serviceMock.Setup(s => s.AdvancePhaseAsync(5, dto)).ReturnsAsync(ServiceResult<GameEventDto>.Ok(updated));

            var result = await _controller.AdvancePhase(5, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(updated, ok.Value);
        }

        [Fact]
        public async Task AdvancePhase_Missing_ReturnsNotFound()
        {
            var dto = new AdvancePhaseDto { NextRound = "Semi-Final" };
            _serviceMock.Setup(s => s.AdvancePhaseAsync(99, dto)).ReturnsAsync(ServiceResult<GameEventDto>.NotFound());

            var result = await _controller.AdvancePhase(99, dto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AdvancePhase_Invalid_ReturnsBadRequest()
        {
            var dto = new AdvancePhaseDto { NextRound = "Final" };
            _serviceMock.Setup(s => s.AdvancePhaseAsync(5, dto))
                .ReturnsAsync(ServiceResult<GameEventDto>.Invalid("Next round must be 'Semi-Final'."));

            var result = await _controller.AdvancePhase(5, dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Conclude_Valid_ReturnsOk()
        {
            var dto = new ConcludeEventDto { WinnerPlayerId = 2 };
            var finalSlot = new GameEventScheduleDto { Id = 3, EventId = 5, Round = "Final", WinnerPlayerId = 2 };
            _serviceMock.Setup(s => s.ConcludeAsync(5, dto)).ReturnsAsync(ServiceResult<GameEventScheduleDto>.Ok(finalSlot));

            var result = await _controller.Conclude(5, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(finalSlot, ok.Value);
        }

        [Fact]
        public async Task Conclude_Missing_ReturnsNotFound()
        {
            var dto = new ConcludeEventDto { WinnerPlayerId = 2 };
            _serviceMock.Setup(s => s.ConcludeAsync(99, dto)).ReturnsAsync(ServiceResult<GameEventScheduleDto>.NotFound());

            var result = await _controller.Conclude(99, dto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Conclude_NotAtFinal_ReturnsBadRequest()
        {
            var dto = new ConcludeEventDto { WinnerPlayerId = 2 };
            _serviceMock.Setup(s => s.ConcludeAsync(5, dto))
                .ReturnsAsync(ServiceResult<GameEventScheduleDto>.Invalid("The event must reach the Final round before it can be concluded."));

            var result = await _controller.Conclude(5, dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
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
