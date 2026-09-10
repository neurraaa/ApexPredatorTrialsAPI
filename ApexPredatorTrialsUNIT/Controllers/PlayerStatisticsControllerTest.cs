using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class PlayerStatisticsControllerTest
    {
        private readonly Mock<IPlayerStatsService> _serviceMock = new();
        private readonly PlayerStatisticsController _controller;

        public PlayerStatisticsControllerTest() => _controller = new PlayerStatisticsController(_serviceMock.Object, NullLogger<PlayerStatisticsController>.Instance);

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var stats = new List<PlayerStatsDto> { new() { Id = 1, HunterRank = "Hunter" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(stats);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(stats, ok.Value);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((PlayerStatsDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new PlayerStatsWriteDto { HunterRank = "Hunter", HumanRank = "Indomitable", LegendLevel = 245 };
            var created = new PlayerStatsDto { Id = 1, HunterRank = "Hunter", HumanRank = "Indomitable", LegendLevel = 245 };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            var dto = new PlayerStatsWriteDto { HunterRank = "Apex Predator", HumanRank = "Ultimate Survivor", LegendLevel = 250 };
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
