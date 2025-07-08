using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Services;
using SmokingQuitSupportAPI.Services.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===================================
// CẤU HỈN SERVICES (DEPENDENCY INJECTION)
// ===================================

/// <summary>
/// Cấu hình Entity Framework với SQL Server
/// Đọc connection string từ appsettings.json
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Đăng ký các services vào DI container
/// Scoped: Tạo instance mới cho mỗi HTTP request
/// </summary>
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommunityPostService, CommunityPostService>();
builder.Services.AddScoped<ISmokingStatusService, SmokingStatusService>();
builder.Services.AddScoped<IQuitPlanService, QuitPlanService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();

// Package System Services
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IPremiumService, PremiumService>();

/// <summary>
/// Cấu hình JWT Authentication
/// </summary>
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    // Sử dụng JWT Bearer làm scheme mặc định
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate các thông số của token
        ValidateIssuer = true, // Kiểm tra issuer
        ValidateAudience = true, // Kiểm tra audience
        ValidateLifetime = true, // Kiểm tra thời gian hết hạn
        ValidateIssuerSigningKey = true, // Kiểm tra signing key
        
        // Thiết lập các giá trị để validate
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        
        // Không cho phép clock skew (độ lệch thời gian)
        ClockSkew = TimeSpan.Zero
    };
});

/// <summary>
/// Thêm Authorization services với policies
/// </summary>
builder.Services.AddAuthorization(options =>
{
    // Policy yêu cầu role Admin
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
        
    // Policy yêu cầu role Coach
    options.AddPolicy("CoachOnly", policy => 
        policy.RequireRole("Coach"));
        
    // Policy yêu cầu role User
    options.AddPolicy("UserOnly", policy => 
        policy.RequireRole("User"));
        
    // Policy yêu cầu Admin hoặc Coach
    options.AddPolicy("AdminOrCoach", policy => 
        policy.RequireRole("Admin", "Coach"));
        
    // Policy yêu cầu bất kỳ role nào (authenticated user)
    options.AddPolicy("AuthenticatedUser", policy => 
        policy.RequireAuthenticatedUser());
        
    // Premium Package Policy
    options.AddPolicy("PremiumOnly", policy => 
        policy.RequireAuthenticatedUser()); // Will be checked in controller
});

/// <summary>
/// Thêm Controllers với API Explorer cho Swagger
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Cấu hình Swagger/OpenAPI với JWT Authentication
/// </summary>
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Smoking Quit Support API", 
        Version = "v1",
        Description = "API hỗ trợ cai thuốc lá với hệ thống phân cấp gói Basic/Premium"
    });
    
    // Cấu hình JWT Authentication trong Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header sử dụng Bearer scheme. Nhập 'Bearer' [space] và token của bạn",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

/// <summary>
/// Cấu hình CORS để cho phép frontend truy cập
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===================================
// XÂY DỰNG ỨNG DỤNG
// ===================================
var app = builder.Build();

// ===================================
// CẤU HÌNH MIDDLEWARE PIPELINE
// ===================================

/// <summary>
/// Cấu hình Swagger chỉ trong Development environment
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smoking Quit Support API v1");
        c.RoutePrefix = string.Empty; // Swagger UI sẽ hiển thị tại root URL
    });
}

/// <summary>
/// Middleware để redirect HTTP sang HTTPS
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Middleware CORS - phải đặt trước Authentication
/// </summary>
app.UseCors("AllowAll");

/// <summary>
/// Middleware Authentication - xác thực JWT token
/// </summary>
app.UseAuthentication();

/// <summary>
/// Middleware Authorization - kiểm tra quyền truy cập
/// </summary>
app.UseAuthorization();

/// <summary>
/// Map Controllers để xử lý các API endpoints
/// </summary>
app.MapControllers();

/// <summary>
/// Tạo database và migrate nếu chưa tồn tại (chỉ trong Development)
<<<<<<< HEAD
=======
/// KHÔNG XÓA DATABASE - chỉ tạo nếu chưa có
>>>>>>> 647da918bd740fe6d490de8f4a9596e882126310
/// </summary>
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
<<<<<<< HEAD
            // Xóa database cũ và tạo lại với cấu trúc mới
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Console.WriteLine("✅ Database đã được tạo lại thành công với cấu trúc mới!");
            
            // Tạo dữ liệu mẫu
            await SeedData(context);
            Console.WriteLine("✅ Dữ liệu mẫu đã được tạo!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi tạo database: {ex.Message}");
=======
            // CHỈ tạo database nếu chưa tồn tại (KHÔNG xóa data cũ)
            var created = context.Database.EnsureCreated();
            
            if (created)
            {
                Console.WriteLine("✅ Database được tạo mới - đang thêm dữ liệu mẫu...");
                await SeedData(context);
                Console.WriteLine("✅ Dữ liệu mẫu đã được tạo!");
            }
            else
            {
                Console.WriteLine("📊 Database đã tồn tại - giữ nguyên dữ liệu hiện có");
                
                // Chỉ thêm dữ liệu mẫu nếu bảng chưa có data
                if (!context.Achievements.Any())
                {
                    await SeedData(context);
                    Console.WriteLine("✅ Đã thêm dữ liệu mẫu còn thiếu!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi kiểm tra database: {ex.Message}");
>>>>>>> 647da918bd740fe6d490de8f4a9596e882126310
        }
    }
}

/// <summary>
/// Chạy ứng dụng
/// </summary>
Console.WriteLine("🚀 Smoking Quit Support API đang khởi động...");
Console.WriteLine("📖 Swagger UI: https://localhost:7000");
Console.WriteLine("🔗 API Base URL: https://localhost:7000/api");
Console.WriteLine("📦 Package System: Basic (Free) + Premium (Paid)");
Console.WriteLine("💎 Premium Features: Coach Support, Chat, Stage Management");

app.Run();

/// <summary>
/// Tạo dữ liệu mẫu
/// </summary>
static async Task SeedData(AppDbContext context)
{
    // Tạo các achievement mẫu
    if (!context.Achievements.Any())
    {
        var achievements = new List<Achievement>
        {
            new Achievement { Name = "Ngày đầu tiên", Description = "Hoàn thành ngày đầu tiên không hút thuốc", Type = "DaysSmokeFree", RequiredValue = 1, BadgeColor = "#4CAF50" },
            new Achievement { Name = "Một tuần", Description = "Hoàn thành 1 tuần không hút thuốc", Type = "DaysSmokeFree", RequiredValue = 7, BadgeColor = "#2196F3" },
            new Achievement { Name = "Một tháng", Description = "Hoàn thành 1 tháng không hút thuốc", Type = "DaysSmokeFree", RequiredValue = 30, BadgeColor = "#FF9800" },
            new Achievement { Name = "Ba tháng", Description = "Hoàn thành 3 tháng không hút thuốc", Type = "DaysSmokeFree", RequiredValue = 90, BadgeColor = "#9C27B0" },
            new Achievement { Name = "Tiết kiệm đầu tiên", Description = "Tiết kiệm được 100,000 VND", Type = "MoneySaved", RequiredValue = 100000, BadgeColor = "#FF5722" }
        };
        
        context.Achievements.AddRange(achievements);
        await context.SaveChangesAsync();
    }

    // Tạo member package mẫu
    if (!context.MemberPackages.Any())
    {
        var packages = new List<MemberPackage>
        {
            new MemberPackage 
            { 
                Name = "Basic Plan", 
                Description = "Gói cơ bản cho người mới bắt đầu - Theo dõi tiến trình, Achievement cơ bản", 
                Price = 0, 
                DurationDays = 30, 
                IsActive = true,
                CreatedById = 1
            },
            new MemberPackage 
            { 
                Name = "Premium Plan", 
                Description = "Gói cao cấp với coach hỗ trợ - Theo dõi tiến trình, Coach cá nhân, Tất cả achievement", 
                Price = 500000, 
                DurationDays = 90, 
                IsActive = true,
                CreatedById = 1
            }
        };
        
        context.MemberPackages.AddRange(packages);
        await context.SaveChangesAsync();
    }
} 