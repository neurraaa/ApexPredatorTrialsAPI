using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class UsersControllerTest
    {
        private readonly Mock<IUserService> _serviceMock = new();
        private readonly UsersController _controller;

        public UsersControllerTest() => _controller = new UsersController(_serviceMock.Object, NullLogger<UsersController>.Instance);

        [Fact]
        public async Task GetUsers_ReturnsOkWithList()
        {
            var users = new List<UserDto> { new() { Id = 1, Username = "neurraaa" } };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            var result = await _controller.GetUsers();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(users, ok.Value);
        }

        [Fact]
        public async Task GetUser_MissingId_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((UserDto?)null);

            var result = await _controller.GetUser(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Register_NewUsername_ReturnsCreatedAtAction()
        {
            var dto = new UserRegisterDto { Username = "neurraaa", Password = "aaarruen" };
            var created = new UserDto { Id = 1, Username = "neurraaa", Role = "admin" };
            _serviceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(ServiceResult<UserDto>.Ok(created));

            var result = await _controller.Register(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task Register_DuplicateUsername_ReturnsBadRequest()
        {
            var dto = new UserRegisterDto { Username = "neurraaa", Password = "aaarruen" };
            _serviceMock.Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync(ServiceResult<UserDto>.Invalid("Username is already taken."));

            var result = await _controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUser_Existing_ReturnsNoContent()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteUser(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_Missing_ReturnsNotFound()
        {
            _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeleteUser(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
