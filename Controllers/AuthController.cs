using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Account;
using SmokingQuitSupportAPI.Models.DTOs.Auth;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller xử lý các API liên quan đến xác thực
    /// Bao gồm đăng ký, đăng nhập
    /// </summary>
    [ApiController] // Attribute đánh dấu đây là API Controller
    [Route("api/[controller]")] // Route pattern: api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; // Inject AuthService

        /// <summary>
        /// Constructor inject AuthService
        /// </summary>
        /// <param name="authService">Service xử lý xác thực</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// API đăng ký tài khoản mới
        /// POST: api/auth/register
        /// </summary>
        /// <param name="createAccountDto">Thông tin tài khoản cần tạo</param>
        /// <returns>Thông tin tài khoản đã tạo</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AccountDto>> Register([FromBody] CreateAccountDto createAccountDto)
        {
            try
            {
                // Gọi service để đăng ký
                var result = await _authService.RegisterAsync(createAccountDto);
                
                // Trả về status 201 Created với thông tin tài khoản
                return CreatedAtAction(nameof(GetAccount), new { id = result.AccountId }, result);
            }
            catch (InvalidOperationException ex)
            {
                // Trả về lỗi 400 Bad Request nếu có lỗi validation
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 Internal Server Error cho các lỗi khác
                return StatusCode(500, new { message = "Có lỗi xảy ra khi đăng ký tài khoản", error = ex.Message });
            }
        }

        /// <summary>
        /// API đăng nhập
        /// POST: api/auth/login
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập</param>
        /// <returns>JWT token</returns>
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Gọi service để đăng nhập
                var token = await _authService.LoginAsync(loginDto);
                
                // Trả về token và thông tin thành công
                return Ok(new 
                { 
                    message = "Đăng nhập thành công",
                    token = token,
                    tokenType = "Bearer"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Trả về lỗi 401 Unauthorized nếu thông tin đăng nhập sai
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 Internal Server Error cho các lỗi khác
                return StatusCode(500, new { message = "Có lỗi xảy ra khi đăng nhập", error = ex.Message });
            }
        }

        /// <summary>
        /// API lấy thông tin tài khoản theo ID
        /// GET: api/auth/account/{id}
        /// </summary>
        /// <param name="id">ID của tài khoản</param>
        /// <returns>Thông tin tài khoản</returns>
        [HttpGet("account/{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(int id)
        {
            try
            {
                // Gọi service để lấy thông tin tài khoản
                var account = await _authService.GetAccountByIdAsync(id);
                
                if (account == null)
                {
                    // Trả về 404 Not Found nếu không tìm thấy tài khoản
                    return NotFound(new { message = "Không tìm thấy tài khoản" });
                }

                // Trả về thông tin tài khoản
                return Ok(account);
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 Internal Server Error cho các lỗi khác
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thông tin tài khoản", error = ex.Message });
            }
        }
    }
} 