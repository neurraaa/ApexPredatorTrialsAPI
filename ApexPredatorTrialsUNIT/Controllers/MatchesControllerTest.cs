using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class MatchesControllerTest
    {
        private readonly Mock<IMatchService> _serviceMock = new();
        private readonly MatchesController _controller;

        public MatchesControllerTest() => _controller = new MatchesController(_serviceMock.Object, NullLogger<MatchesController>.Instance);

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var matches = new List<MatchDto> { new() { Id = 1, Region = "EU" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(matches);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(matches, ok.Value);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((MatchDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ValidMatch_ReturnsCreatedAtAction()
        {
            var dto = new MatchWriteDto { HunterPlayerId = 1, HumanPlayerId = 2, MapId = 1, Region = "EU" };
            var created = new MatchDto { Id = 1, HunterPlayerId = 1, HumanPlayerId = 2, MapId = 1, Region = "EU" };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<MatchDto>.Ok(created));

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task Create_SamePlayerBothSides_ReturnsBadRequest()
        {
            var dto = new MatchWriteDto { HunterPlayerId = 1, HumanPlayerId = 1, MapId = 1, Region = "EU" };
            _serviceMock.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ServiceResult<MatchDto>.Invalid("A player cannot play both sides of the same match."));

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_Valid_ReturnsNoContent()
        {
            var dto = new MatchWriteDto { HunterPlayerId = 1, HumanPlayerId = 2, MapId = 1, Region = "EU" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult<bool>.Ok(true));

            var result = await _controller.Update(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            var dto = new MatchWriteDto { HunterPlayerId = 1, HumanPlayerId = 2, MapId = 1, Region = "EU" };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync(ServiceResult<bool>.NotFound());

            var result = await _controller.Update(99, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_SamePlayerBothSides_ReturnsBadRequest()
        {
            var dto = new MatchWriteDto { HunterPlayerId = 1, HumanPlayerId = 1, MapId = 1, Region = "EU" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(ServiceResult<bool>.Invalid("A player cannot play both sides of the same match."));

            var result = await _controller.Update(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
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
