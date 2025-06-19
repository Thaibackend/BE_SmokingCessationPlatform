using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Account;
using SmokingQuitSupportAPI.Models.DTOs.Auth;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý xác thực người dùng
    /// Implement interface IAuthService
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context; // Database context để truy cập dữ liệu
        private readonly IConfiguration _configuration; // Configuration để đọc JWT settings

        /// <summary>
        /// Constructor inject dependencies
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="configuration">App configuration</param>
        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        public async Task<AccountDto> RegisterAsync(CreateAccountDto createAccountDto)
        {
            // Validate role
            if (!createAccountDto.IsValidRole)
                throw new ArgumentException($"Invalid role: {createAccountDto.Role}. Valid roles are: {string.Join(", ", SmokingQuitSupportAPI.Constants.Roles.AllRoles)}");

            // Kiểm tra username đã tồn tại chưa
            var existingUser = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == createAccountDto.Username);
            
            if (existingUser != null)
                throw new InvalidOperationException("Username đã tồn tại");

            // Kiểm tra email đã tồn tại chưa
            var existingEmail = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == createAccountDto.Email);
            
            if (existingEmail != null)
                throw new InvalidOperationException("Email đã tồn tại");

            // Tạo entity Account mới
            var account = new Account
            {
                Username = createAccountDto.Username,
                Email = createAccountDto.Email,
                // Hash password bằng BCrypt để bảo mật
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createAccountDto.Password),
                FullName = createAccountDto.FullName,
                Role = createAccountDto.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Thêm vào database
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Trả về DTO (không bao gồm password)
            return new AccountDto
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        /// <summary>
        /// Đăng nhập và tạo JWT token
        /// </summary>
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // Tìm user theo username
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == loginDto.Username);

            // Kiểm tra user tồn tại và password đúng
            if (account == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, account.PasswordHash))
                throw new UnauthorizedAccessException("Username hoặc password không đúng");

            // Tạo JWT token
            return GenerateJwtToken(account);
        }

        /// <summary>
        /// Lấy thông tin tài khoản theo ID
        /// </summary>
        public async Task<AccountDto?> GetAccountByIdAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            
            if (account == null)
                return null;

            return new AccountDto
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        /// <summary>
        /// Tạo JWT token cho user
        /// </summary>
        /// <param name="account">Thông tin tài khoản</param>
        /// <returns>JWT token string</returns>
        private string GenerateJwtToken(Account account)
        {
            // Đọc JWT settings từ appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60");

            // Tạo security key từ secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo claims (thông tin trong token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role)
            };

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: credentials
            );

            // Trả về token dưới dạng string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 