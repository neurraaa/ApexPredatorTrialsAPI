using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class MapsControllerTest
    {
        private readonly Mock<IGameMapService> _serviceMock = new();
        private readonly MapsController _controller;

        public MapsControllerTest() => _controller = new MapsController(_serviceMock.Object, NullLogger<MapsController>.Instance);

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var maps = new List<GameMapDto> { new() { Id = 1, Name = "Slums" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(maps);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(maps, ok.Value);
        }

        [Fact]
        public async Task GetById_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((GameMapDto?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new GameMapWriteDto { Name = "Old Town" };
            var created = new GameMapDto { Id = 2, Name = "Old Town" };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Update_Missing_ReturnsNotFound()
        {
            var dto = new GameMapWriteDto { Name = "Old Town" };
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
