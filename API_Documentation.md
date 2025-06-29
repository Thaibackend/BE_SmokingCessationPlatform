# 🚭 Smoking Quit Support API - Tài Liệu Hướng Dẫn

## 📋 Tổng Quan Dự Án

Đây là API hỗ trợ cai thuốc lá được xây dựng bằng .NET 8 với Entity Framework Core và SQL Server. API cung cấp các tính năng:

- ✅ Xác thực và phân quyền người dùng (JWT)
- ✅ Quản lý kế hoạch cai thuốc
- ✅ Theo dõi tiến trình hàng ngày
- ✅ Hệ thống huy hiệu thành tích
- ✅ Cộng đồng chia sẻ kinh nghiệm
- ✅ Thống kê và báo cáo

---

## 🏗️ Cấu Trúc Dự Án

```
SmokingQuitSupportAPI/
├── Controllers/          # API Endpoints
├── Services/            # Business Logic
├── Models/              # Data Models
│   ├── Entities/        # Database Entities
│   └── DTOs/           # Data Transfer Objects
├── Data/               # Database Context
└── Program.cs          # Cấu hình ứng dụng
```

---

## 🔧 Program.cs - Cấu Hình Chính

### Phần 1: Cấu hình Database
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
**Giải thích:** Cấu hình Entity Framework với SQL Server, đọc connection string từ appsettings.json

### Phần 2: Dependency Injection
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommunityPostService, CommunityPostService>();
builder.Services.AddScoped<ISmokingStatusService, SmokingStatusService>();
builder.Services.AddScoped<IQuitPlanService, QuitPlanService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
```
**Giải thích:** 
- `AddScoped`: Tạo instance mới cho mỗi HTTP request
- Đăng ký các service interfaces với implementations tương ứng

### Phần 3: JWT Authentication
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,          // Kiểm tra người phát hành token
        ValidateAudience = true,        // Kiểm tra đối tượng nhận token
        ValidateLifetime = true,        // Kiểm tra thời gian hết hạn
        ValidateIssuerSigningKey = true,// Kiểm tra chữ ký
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero       // Không cho phép độ lệch thời gian
    };
});
```

---

## 📊 Models/Entities - Cấu Trúc Database

### Achievement.cs - Huy Hiệu Thành Tích
```csharp
public class Achievement
{
    [Key]
    public int AchievementId { get; set; }          // Khóa chính
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }                // Tên huy hiệu (bắt buộc, tối đa 100 ký tự)
    
    [StringLength(500)]
    public string? Description { get; set; }        // Mô tả (tùy chọn, tối đa 500 ký tự)
    
    [StringLength(200)]
    public string? Icon { get; set; }               // URL icon
    
    [Required]
    [StringLength(50)]
    public string Type { get; set; }                // Loại thành tích (SMOKE_FREE_DAYS, MONEY_SAVED...)
    
    public int RequiredValue { get; set; }          // Giá trị cần đạt để mở khóa
    
    [StringLength(20)]
    public string? BadgeColor { get; set; }         // Màu huy hiệu
    
    public DateTime CreatedAt { get; set; }         // Thời gian tạo
    
    // Navigation Property - Mối quan hệ với UserAchievement
    public virtual ICollection<UserAchievement> UserAchievements { get; set; }
}
```

### SmokingStatus.cs - Tình Trạng Hút Thuốc
```csharp
public class SmokingStatus
{
    [Key]
    public int StatusId { get; set; }
    
    [Required]
    public int AccountId { get; set; }              // ID tài khoản
    
    [Required]
    public DateTime QuitDate { get; set; }          // Ngày bắt đầu cai thuốc
    
    [Required]
    public int CigarettesPerDay { get; set; }       // Số điếu thuốc hút mỗi ngày trước khi cai
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal CostPerPack { get; set; }        // Giá tiền mỗi gói thuốc
    
    [Required]
    public int CigarettesPerPack { get; set; }      // Số điếu trong mỗi gói
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal MoneySaved { get; set; }         // Tiền đã tiết kiệm được
    
    public int SmokeFreenDays { get; set; }         // Số ngày đã cai thuốc
    public int CigarettesAvoided { get; set; }      // Số điếu thuốc đã tránh được
    
    [StringLength(500)]
    public string? HealthImprovement { get; set; }   // Thông điệp cải thiện sức khỏe
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; }              // Trạng thái (ACTIVE, PAUSED, COMPLETED)
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // Navigation Property
    public virtual Account Account { get; set; }
}
```

### Progress.cs - Tiến Trình Hàng Ngày
```csharp
public class Progress
{
    [Key]
    public int ProgressId { get; set; }
    
    [Required]
    public int AccountId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }              // Ngày ghi nhận tiến trình
    
    public int SmokeFreenDays { get; set; }         // Số ngày smoke-free tính đến ngày này
    public int CigarettesAvoided { get; set; }      // Số điếu thuốc tránh được trong ngày
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MoneySaved { get; set; }         // Tiền tiết kiệm trong ngày
    
    public int? HealthScore { get; set; }           // Điểm sức khỏe (1-10)
    
    [StringLength(1000)]
    public string? Notes { get; set; }              // Ghi chú của người dùng
    
    public int? Mood { get; set; }                  // Tâm trạng (1-5)
    public int? CravingLevel { get; set; }          // Mức độ thèm thuốc (1-5)
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? Weight { get; set; }            // Cân nặng
    
    public int? ExerciseMinutes { get; set; }       // Thời gian tập thể dục (phút)
    
    [Column(TypeName = "decimal(4,2)")]
    public decimal? SleepHours { get; set; }        // Số giờ ngủ
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Property
    public virtual Account Account { get; set; }
}
```

---

## 🔧 Services - Business Logic

### AchievementService.cs - Service Quản Lý Huy Hiệu

#### Phương thức GetAllAchievementsAsync()
```csharp
public async Task<IEnumerable<AchievementDto>> GetAllAchievementsAsync()
{
    var achievements = await _context.Achievements
        .Select(a => new AchievementDto
        {
            AchievementId = a.AchievementId,
            Name = a.Name,
            Description = a.Description,
            Icon = a.Icon,
            Type = a.Type,
            RequiredValue = a.RequiredValue,
            BadgeColor = a.BadgeColor,
            CreatedAt = a.CreatedAt,
            TotalUnlocks = a.UserAchievements.Count  // Đếm số người đã mở khóa
        })
        .ToListAsync();
    
    return achievements;
}
```
**Giải thích:**
- `Select()`: Projection từ Entity sang DTO để tránh over-fetching
- `TotalUnlocks`: Sử dụng Count() để đếm số lượng UserAchievements
- `ToListAsync()`: Thực thi query bất đồng bộ

#### Phương thức CalculateProgress() - Tính Toán Tiến Trình
```csharp
private int CalculateProgress(int accountId, string achievementType, int requiredValue)
{
    return achievementType.ToUpper() switch
    {
        "SMOKE_FREE_DAYS" => GetSmokeFreenDays(accountId),
        "MONEY_SAVED" => GetMoneySaved(accountId),
        "POSTS_CREATED" => GetPostsCreated(accountId),
        "COMMENTS_MADE" => GetCommentsMade(accountId),
        "PLANS_COMPLETED" => GetPlansCompleted(accountId),
        _ => 0
    };
}
```
**Giải thích:**
- Sử dụng `switch expression` (C# 8.0+) để xử lý các loại achievement khác nhau
- Mỗi loại achievement có cách tính tiến trình riêng

### SmokingStatusService.cs - Service Quản Lý Tình Trạng Hút Thuốc

#### Phương thức CalculateSmokeFreenDays()
```csharp
private int CalculateSmokeFreenDays(DateTime quitDate)
{
    var days = (DateTime.UtcNow.Date - quitDate.Date).Days;
    return Math.Max(0, days);
}
```
**Giải thích:**
- Tính số ngày từ ngày cai thuốc đến hiện tại
- Sử dụng `Math.Max(0, days)` để đảm bảo không trả về số âm

#### Phương thức GenerateHealthImprovement()
```csharp
private string GenerateHealthImprovement(int smokeFreenDays)
{
    return smokeFreenDays switch
    {
        0 => "Bắt đầu hành trình cai thuốc - Cơ thể bắt đầu thanh lọc nicotine",
        1 => "Ngày đầu tiên thành công! Nguy cơ đau tim bắt đầu giảm",
        7 => "1 tuần không thuốc! Vị giác và khứu giác đang cải thiện",
        14 => "2 tuần! Tuần hoàn máu và chức năng phổi đang cải thiện",
        30 => "1 tháng! Nguy cơ nhiễm trùng giảm đáng kể",
        90 => "3 tháng! Chức năng phổi cải thiện đến 30%",
        365 => "1 năm! Nguy cơ bệnh tim giảm một nửa so với người hút thuốc",
        >= 1825 => "5 năm! Nguy cơ đột quỵ giảm như người không hút thuốc",
        >= 3650 => "10 năm! Nguy cơ ung thư phổi giảm một nửa",
        >= 5475 => "15 năm! Nguy cơ bệnh tim như người không bao giờ hút thuốc",
        _ => smokeFreenDays switch
        {
            < 7 => "Cơ thể đang loại bỏ độc tố từ thuốc lá",
            < 30 => "Hệ thần kinh đang phục hồi, cải thiện giấc ngủ",
            < 90 => "Hệ miễn dịch mạnh hơn, ít bị cảm lạnh",
            < 365 => "Da sáng hơn, răng trắng hơn, hơi thở thơm mát",
            _ => "Cơ thể tiếp tục phục hồi và khỏe mạnh hơn mỗi ngày"
        }
    };
}
```
**Giải thích:**
- Sử dụng nested switch expressions để tạo thông điệp động
- Dựa trên các cột mốc y khoa thực tế về lợi ích của việc cai thuốc

### ProgressService.cs - Service Quản Lý Tiến Trình

#### Phương thức RecordDailyProgressAsync()
```csharp
public async Task<ProgressDto> RecordDailyProgressAsync(int accountId, CreateProgressDto createProgressDto)
{
    // Kiểm tra xem đã có record cho ngày này chưa
    var existingProgress = await _context.ProgressRecords
        .FirstOrDefaultAsync(p => p.AccountId == accountId && p.Date.Date == createProgressDto.Date.Date);

    if (existingProgress != null)
    {
        throw new InvalidOperationException("Progress for this date already exists. Use update instead.");
    }

    // Tính toán smokeFreenDays dựa trên quit date
    var smokingStatus = await _context.SmokingStatuses
        .Where(s => s.AccountId == accountId)
        .OrderByDescending(s => s.StatusId)
        .FirstOrDefaultAsync();

    var quitDate = smokingStatus?.QuitDate ?? DateTime.UtcNow.Date;
    var smokeFreenDays = (createProgressDto.Date.Date - quitDate.Date).Days + 1;

    var progress = new Progress
    {
        AccountId = accountId,
        Date = createProgressDto.Date,
        SmokeFreenDays = Math.Max(0, smokeFreenDays),
        CigarettesAvoided = createProgressDto.CigarettesAvoided,
        MoneySaved = createProgressDto.MoneySaved,
        HealthScore = createProgressDto.HealthScore,
        Notes = createProgressDto.Notes,
        Mood = createProgressDto.Mood,
        CravingLevel = createProgressDto.CravingLevel,
        Weight = createProgressDto.Weight,
        ExerciseMinutes = createProgressDto.ExerciseMinutes,
        SleepHours = createProgressDto.SleepHours
    };

    _context.ProgressRecords.Add(progress);
    await _context.SaveChangesAsync();

    // Cập nhật thống kê tự động
    await UpdateAutomaticStatisticsAsync(accountId);

    return new ProgressDto { ... };
}
```
**Giải thích:**
1. Kiểm tra duplicate record cho cùng một ngày
2. Lấy thông tin smoking status để tính smokeFreenDays
3. Tạo record mới với thông tin đầy đủ
4. Tự động cập nhật thống kê sau khi ghi nhận

#### Phương thức GetCurrentStreakAsync()
```csharp
public async Task<int> GetCurrentStreakAsync(int accountId)
{
    var progressRecords = await _context.ProgressRecords
        .Where(p => p.AccountId == accountId)
        .OrderByDescending(p => p.Date)
        .ToListAsync();

    if (!progressRecords.Any())
        return 0;

    int streak = 0;
    var currentDate = DateTime.UtcNow.Date;

    foreach (var progress in progressRecords)
    {
        if (progress.Date.Date == currentDate.AddDays(-streak))
        {
            streak++;
        }
        else
        {
            break;  // Streak bị gián đoạn
        }
    }

    return streak;
}
```
**Giải thích:**
- Tính chuỗi ngày liên tiếp ghi nhận tiến trình
- Bắt đầu từ hôm nay và đếm ngược
- Dừng lại khi gặp ngày không có record

### QuitPlanService.cs - Service Quản Lý Kế Hoạch Cai Thuốc

#### Phương thức GenerateAutomaticQuitPlanAsync()
```csharp
public async Task<QuitPlanDto> GenerateAutomaticQuitPlanAsync(int accountId, int packageId)
{
    // Lấy thông tin user và package
    var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
    var package = await _context.MemberPackages.FirstOrDefaultAsync(p => p.PackageId == packageId && p.IsActive);
    
    if (account == null || package == null)
        throw new InvalidOperationException("Account or package not found.");

    // Lấy smoking status để hiểu thói quen của user
    var smokingStatus = await _context.SmokingStatuses
        .Where(s => s.AccountId == accountId)
        .OrderByDescending(s => s.StatusId)
        .FirstOrDefaultAsync();

    // Tạo kế hoạch dựa trên duration của package
    var startDate = DateTime.UtcNow.Date;
    var endDate = package.DurationDays > 0 ? startDate.AddDays(package.DurationDays) : (DateTime?)null;

    var planName = $"Kế hoạch cai thuốc - {package.Name}";
    var description = GeneratePlanDescription(package, smokingStatus);

    var createPlanDto = new CreateQuitPlanDto
    {
        Name = planName,
        Description = description,
        StartDate = startDate,
        EndDate = endDate,
        PackageId = packageId,
        CoachId = package.AssignedCoachId,
        QuitReason = "Tự động tạo dựa trên gói thành viên"
    };

    return await CreateQuitPlanAsync(accountId, createPlanDto);
}
```
**Giải thích:**
1. Validate account và package tồn tại
2. Lấy thông tin smoking status để cá nhân hóa kế hoạch
3. Tính toán thời gian dựa trên package duration
4. Tạo mô tả động dựa trên thông tin user
5. Gọi CreateQuitPlanAsync để tạo kế hoạch thực tế

---

## 🎯 DTOs - Data Transfer Objects

### AchievementDto.cs
```csharp
public class AchievementDto
{
    public int AchievementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Type { get; set; } = string.Empty;
    public int RequiredValue { get; set; }
    public string? BadgeColor { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Thông tin cho user hiện tại
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public int CurrentProgress { get; set; }
    
    // Computed Property - Tính toán tự động
    public double ProgressPercentage => RequiredValue > 0 ? 
        Math.Min(100, (double)CurrentProgress / RequiredValue * 100) : 0;
    
    // Thống kê
    public int TotalUnlocks { get; set; }
}
```
**Giải thích:**
- DTO kết hợp thông tin của Achievement entity với thông tin cá nhân của user
- `ProgressPercentage`: Computed property tính phần trăm hoàn thành
- Sử dụng `Math.Min(100, ...)` để đảm bảo không vượt quá 100%

### ProgressDto.cs
```csharp
public class ProgressDto
{
    public int ProgressId { get; set; }
    public int AccountId { get; set; }
    public DateTime Date { get; set; }
    public int SmokeFreenDays { get; set; }
    public int CigarettesAvoided { get; set; }
    public decimal MoneySaved { get; set; }
    public int? HealthScore { get; set; }
    public string? Notes { get; set; }
    public int? Mood { get; set; }
    public int? CravingLevel { get; set; }
    public decimal? Weight { get; set; }
    public int? ExerciseMinutes { get; set; }
    public decimal? SleepHours { get; set; }
    
    // Computed Properties - Mô tả người dùng có thể hiểu
    public string MoodDescription => Mood switch
    {
        1 => "Rất tồi tệ",
        2 => "Tồi tệ",
        3 => "Bình thường", 
        4 => "Tốt",
        5 => "Rất tốt",
        _ => "Chưa đánh giá"
    };
    
    public string CravingDescription => CravingLevel switch
    {
        1 => "Không có cảm giác thèm",
        2 => "Hơi thèm",
        3 => "Thèm vừa phải",
        4 => "Thèm nhiều", 
        5 => "Rất thèm",
        _ => "Chưa đánh giá"
    };
}
```
**Giải thích:**
- Sử dụng switch expressions để convert số thành mô tả dễ hiểu
- Nullable properties (`int?`, `decimal?`) cho phép user không bắt buộc nhập

---

## 🔒 Bảo Mật và Validation

### Data Annotations
```csharp
[Required(ErrorMessage = "Ngày bắt đầu cai thuốc là bắt buộc")]
public DateTime QuitDate { get; set; }

[Range(1, 100, ErrorMessage = "Số điếu thuốc mỗi ngày phải từ 1 đến 100")]
public int CigarettesPerDay { get; set; }

[Range(0.01, 1000000, ErrorMessage = "Giá tiền phải lớn hơn 0")]
public decimal CostPerPack { get; set; }
```
**Giải thích:**
- `[Required]`: Bắt buộc nhập
- `[Range]`: Giới hạn giá trị
- `ErrorMessage`: Thông báo lỗi tiếng Việt

### JWT Token Validation
```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,              // Kiểm tra issuer
    ValidateAudience = true,            // Kiểm tra audience  
    ValidateLifetime = true,            // Kiểm tra expiry
    ValidateIssuerSigningKey = true,    // Kiểm tra signing key
    ClockSkew = TimeSpan.Zero           // Không cho phép clock skew
};
```

---

## 📈 Performance Optimizations

### 1. Async/Await Pattern
```csharp
public async Task<IEnumerable<ProgressDto>> GetUserProgressAsync(int accountId)
{
    var progress = await _context.ProgressRecords
        .Where(p => p.AccountId == accountId)
        .OrderByDescending(p => p.Date)
        .ToListAsync();  // Non-blocking call
    
    return progress;
}
```

### 2. Projection với Select()
```csharp
.Select(s => new SmokingStatusDto
{
    StatusId = s.StatusId,
    AccountId = s.AccountId,
    // ... chỉ lấy các field cần thiết
})
```
**Lợi ích:** Giảm data transfer, tránh N+1 query problem

### 3. Include cho Related Data
```csharp
.Include(q => q.Package)
.Include(q => q.Member)
.Include(q => q.Coach)
.ThenInclude(c => c!.Account)
```
**Giải thích:** Load related data trong một query duy nhất

---

## 🚀 Deployment Notes

### appsettings.json Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmokingQuitSupportDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "SmokingQuitSupportAPI",
    "Audience": "SmokingQuitSupportAPI", 
    "ExpiryInMinutes": 60
  }
}
```

### Database Migration Commands
```bash
# Tạo migration mới
dotnet ef migrations add InitialCreate

# Cập nhật database
dotnet ef database update

# Xóa database (development only)
dotnet ef database drop
```

---

## 🧪 Testing Guidelines

### Unit Test Example
```csharp
[Test]
public async Task CalculateSmokeFreenDays_ShouldReturnCorrectDays()
{
    // Arrange
    var quitDate = DateTime.UtcNow.AddDays(-10);
    var service = new SmokingStatusService(_context);
    
    // Act
    var result = service.CalculateSmokeFreenDays(quitDate);
    
    // Assert
    Assert.AreEqual(10, result);
}
```

### Integration Test
```csharp
[Test]
public async Task CreateQuitPlan_ShouldCreateSuccessfully()
{
    // Arrange
    var createDto = new CreateQuitPlanDto { ... };
    
    // Act
    var result = await _quitPlanService.CreateQuitPlanAsync(userId, createDto);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(createDto.Name, result.Name);
}
```

---

## 📝 Best Practices Implemented

1. **Repository Pattern**: Service layer tách biệt với Data Access
2. **DTO Pattern**: Tránh expose Entity trực tiếp
3. **Async/Await**: Non-blocking operations
4. **Dependency Injection**: Loose coupling
5. **Error Handling**: Try-catch với meaningful messages
6. **Validation**: Data Annotations + custom validation
7. **Security**: JWT authentication + authorization
8. **Performance**: Efficient queries with projection
9. **Maintainability**: Clear separation of concerns
10. **Documentation**: Comprehensive code comments

---

## 🔄 Future Enhancements

1. **Caching**: Redis cho frequently accessed data
2. **Logging**: Serilog cho structured logging  
3. **Health Checks**: Monitoring endpoints
4. **Rate Limiting**: Prevent API abuse
5. **Background Jobs**: Hangfire cho scheduled tasks
6. **Real-time**: SignalR cho notifications
7. **File Upload**: CloudFlare/AWS S3 cho images
8. **Localization**: Multi-language support

---

*Tài liệu này được tạo để giúp developers hiểu rõ cấu trúc và logic của Smoking Quit Support API. Vui lòng cập nhật khi có thay đổi trong code.* 