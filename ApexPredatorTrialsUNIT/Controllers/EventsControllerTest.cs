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
            SetUser(userId: 1, isAdmin: true);
        }

        private void SetUser(int userId, bool isAdmin)
        {
            var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, userId.ToString()) };
            if (isAdmin) claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth", ClaimTypes.Name, ClaimTypes.Role)) }
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
                NextMatchups = new() { new NextMatchupDto { HunterScheduleId = 1, HumanScheduleId = 1, MapId = 1 } }
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
        public async Task SetMatchResult_Valid_ReturnsOk()
        {
            var dto = new ScheduleMatchResultDto { WinnerId = 1, LoserId = 2 };
            var updatedSlot = new GameEventScheduleDto { Id = 3, EventId = 5, Round = "Final", WinnerPlayerId = 1 };
            _serviceMock.Setup(s => s.SetMatchResultAsync(5, 3, dto)).ReturnsAsync(ServiceResult<GameEventScheduleDto>.Ok(updatedSlot));

            var result = await _controller.SetMatchResult(5, 3, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(updatedSlot, ok.Value);
        }

        [Fact]
        public async Task SetMatchResult_Missing_ReturnsNotFound()
        {
            var dto = new ScheduleMatchResultDto { WinnerId = 1, LoserId = 2 };
            _serviceMock.Setup(s => s.SetMatchResultAsync(5, 99, dto)).ReturnsAsync(ServiceResult<GameEventScheduleDto>.NotFound());

            var result = await _controller.SetMatchResult(5, 99, dto);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task SetMatchResult_Invalid_ReturnsBadRequest()
        {
            var dto = new ScheduleMatchResultDto { WinnerId = 1, LoserId = 1 };
            _serviceMock.Setup(s => s.SetMatchResultAsync(5, 3, dto))
                .ReturnsAsync(ServiceResult<GameEventScheduleDto>.Invalid("Winner and Loser cannot be the same player."));

            var result = await _controller.SetMatchResult(5, 3, dto);

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
        public async Task Update_AsOrganizer_ReturnsNoContent()
        {
            SetUser(userId: 2, isAdmin: false);
            var dto = new GameEventWriteDto { Title = "Hunter Protocol", Region = "EU" };
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(new GameEventDto { Id = 5, OrganizerId = 2 });
            _serviceMock.Setup(s => s.UpdateAsync(5, dto)).ReturnsAsync(true);

            var result = await _controller.Update(5, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_AsNonOrganizerNonAdmin_ReturnsForbid()
        {
            SetUser(userId: 3, isAdmin: false);
            var dto = new GameEventWriteDto { Title = "Hunter Protocol", Region = "EU" };
            _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(new GameEventDto { Id = 5, OrganizerId = 2 });

            var result = await _controller.Update(5, dto);

            Assert.IsType<ForbidResult>(result);
            _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<GameEventWriteDto>()), Times.Never);
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
