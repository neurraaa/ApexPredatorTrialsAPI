using ApexPredatorTrialsAPI.Controllers;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApexPredatorTrialsUNIT.Controllers
{
    public class AuthControllerTest
    {
        private readonly Mock<IAuthService> _serviceMock = new();
        private readonly AuthController _controller;

        public AuthControllerTest() => _controller = new AuthController(_serviceMock.Object, NullLogger<AuthController>.Instance);

        [Fact]
        public async Task Register_NewUsername_ReturnsOkWithToken()
        {
            var dto = new UserRegisterDto { Username = "alice", Password = "password123" };
            var response = new AuthResponseDto { Token = "fake-jwt", User = new UserDto { Id = 1, Username = "alice", Role = "User" } };
            _serviceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(ServiceResult<AuthResponseDto>.Ok(response));

            var result = await _controller.Register(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Register_DuplicateUsername_ReturnsBadRequest()
        {
            var dto = new UserRegisterDto { Username = "alice", Password = "password123" };
            _serviceMock.Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync(ServiceResult<AuthResponseDto>.Invalid("Username is already taken."));

            var result = await _controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var dto = new UserLoginDto { Username = "alice", Password = "password123" };
            var response = new AuthResponseDto { Token = "fake-jwt", User = new UserDto { Id = 1, Username = "alice", Role = "User" } };
            _serviceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(ServiceResult<AuthResponseDto>.Ok(response));

            var result = await _controller.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var dto = new UserLoginDto { Username = "alice", Password = "wrong" };
            _serviceMock.Setup(s => s.LoginAsync(dto))
                .ReturnsAsync(ServiceResult<AuthResponseDto>.Invalid("Invalid username or password."));

            var result = await _controller.Login(dto);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}
