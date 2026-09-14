using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class MatchResultsControllerTest
    {
        private readonly Mock<IMatchResultsService> _serviceMock = new();
        private readonly MatchResultsController _controller;

        public MatchResultsControllerTest() => _controller = new MatchResultsController(_serviceMock.Object, NullLogger<MatchResultsController>.Instance);

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var results = new List<MatchResultsDto> { new() { Id = 1, MatchId = 1, WinnerId = 1, LoserId = 2 } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(results);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(results, ok.Value);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((MatchResultsDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_Valid_ReturnsCreatedAtAction()
        {
            var dto = new MatchResultsCreateDto { MatchId = 1, WinnerId = 1, LoserId = 2 };
            var created = new MatchResultsDto { Id = 1, MatchId = 1, WinnerId = 1, LoserId = 2 };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<MatchResultsDto>.Ok(created));

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task Create_MatchDoesNotExist_ReturnsBadRequest()
        {
            var dto = new MatchResultsCreateDto { MatchId = 999, WinnerId = 1, LoserId = 2 };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(ServiceResult<MatchResultsDto>.NotFound());

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_InvalidWinnerLoser_ReturnsBadRequest()
        {
            var dto = new MatchResultsCreateDto { MatchId = 1, WinnerId = 1, LoserId = 1 };
            _serviceMock.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ServiceResult<MatchResultsDto>.Invalid("Winner and Loser cannot be the same player."));

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetByMatch_Found_ReturnsOk()
        {
            var results = new MatchResultsDto { Id = 1, MatchId = 7, WinnerId = 1, LoserId = 2 };
            _serviceMock.Setup(s => s.GetByMatchIdAsync(7)).ReturnsAsync(results);

            var result = await _controller.GetByMatch(7);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(results, ok.Value);
        }

        [Fact]
        public async Task GetByMatch_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByMatchIdAsync(7)).ReturnsAsync((MatchResultsDto?)null);

            var result = await _controller.GetByMatch(7);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_Valid_ReturnsOk()
        {
            var dto = new MatchResultsCreateDto { MatchId = 1, WinnerId = 1, LoserId = 2 };
            var updated = new MatchResultsDto { Id = 1, MatchId = 1, WinnerId = 1, LoserId = 2 };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(ServiceResult<MatchResultsDto>.Ok(updated));

            var result = await _controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updated, ok.Value);
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            var dto = new MatchResultsCreateDto { MatchId = 1, WinnerId = 1, LoserId = 2 };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync(ServiceResult<MatchResultsDto>.NotFound());

            var result = await _controller.Update(99, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_InvalidWinnerLoser_ReturnsBadRequest()
        {
            var dto = new MatchResultsCreateDto { MatchId = 1, WinnerId = 1, LoserId = 1 };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto))
                .ReturnsAsync(ServiceResult<MatchResultsDto>.Invalid("Winner and Loser cannot be the same player."));

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

        [Fact]
        public async Task Delete_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
