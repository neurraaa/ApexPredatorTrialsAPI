using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class PlayersControllerTest
    {
        private readonly Mock<IPlayerService> _serviceMock = new();
        private readonly PlayersController _controller;

        public PlayersControllerTest() => _controller = new PlayersController(_serviceMock.Object, NullLogger<PlayersController>.Instance);

        [Fact]
        public async Task GetPlayers_ReturnsOkWithList()
        {
            var players = new List<PlayerDto> { new() { Id = 1, Name = "Neura" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(players);

            var result = await _controller.GetPlayers();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(players, ok.Value);
        }

        [Fact]
        public async Task GetPlayer_ExistingId_ReturnsOk()
        {
            var player = new PlayerDto { Id = 1, Name = "Neura" };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(player);

            var result = await _controller.GetPlayer(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(player, ok.Value);
        }

        [Fact]
        public async Task GetPlayer_MissingId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((PlayerDto?)null);

            var result = await _controller.GetPlayer(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePlayer_ReturnsCreatedAtAction()
        {
            var writeDto = new PlayerWriteDto { Name = "Neura", Platform = "Steam", Region = "EU" };
            var created = new PlayerDto { Id = 5, Name = "Neura", Platform = "Steam", Region = "EU" };
            _serviceMock.Setup(s => s.CreateAsync(writeDto)).ReturnsAsync(created);

            var result = await _controller.CreatePlayer(writeDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task UpdatePlayer_Existing_ReturnsNoContent()
        {
            var dto = new PlayerWriteDto { Name = "Neu'ra", Platform = "Steam", Region = "EU" };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.UpdatePlayer(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePlayer_Missing_ReturnsNotFound()
        {
            var dto = new PlayerWriteDto { Name = "Neu'ra", Platform = "Steam", Region = "EU" };
            _serviceMock.Setup(s => s.UpdateAsync(99, dto)).ReturnsAsync(false);

            var result = await _controller.UpdatePlayer(99, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePlayer_Existing_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeletePlayer(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePlayer_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeletePlayer(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
