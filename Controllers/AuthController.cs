using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Auth;
using SmokingQuitSupportAPI.Services;
using Microsoft.Extensions.Logging;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = errors });
                }

                var response = await _authService.RegisterUserAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid registration attempt");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return BadRequest(new { message = "Đã xảy ra lỗi khi đăng ký tài khoản" });
            }
        }

        [HttpPost("register/coach")]
        public async Task<IActionResult> RegisterCoach([FromBody] RegisterCoachRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = errors });
                }

                var response = await _authService.RegisterCoachAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during coach registration");
                return BadRequest(new { message = "Đã xảy ra lỗi khi đăng ký coach" });
            }
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = errors });
                }

                var response = await _authService.RegisterAdminAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid admin registration attempt");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin registration");
                return BadRequest(new { message = "Đã xảy ra lỗi khi đăng ký admin" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Convert to RegisterUserRequestDto
            var userRequest = new RegisterUserRequestDto
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };
            
            return await RegisterUser(userRequest);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = errors });
                }

                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Failed login attempt");
                return BadRequest(new { message = "Email hoặc mật khẩu không đúng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return BadRequest(new { message = "Đã xảy ra lỗi khi đăng nhập" });
            }
        }
    }
} 