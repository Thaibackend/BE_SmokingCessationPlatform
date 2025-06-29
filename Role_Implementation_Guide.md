# Hướng Dẫn Phân Biệt Role User, Admin và Coach

## 1. Tổng Quan Hệ Thống Role

### 🔐 **Cấu Trúc Role**
```
Admin (Quản trị viên)
├── Có quyền cao nhất
├── Quản lý toàn bộ hệ thống
└── Có thể thay đổi role của user khác

Coach (Huấn luyện viên)
├── Có quyền trung bình
├── Quản lý và hỗ trợ users
└── Tạo và theo dõi quit plans

User (Người dùng)
├── Quyền cơ bản
├── Sử dụng các tính năng chính
└── Không thể truy cập quản trị
```

## 2. Cách Phân Biệt Role

### A. **Trong Database**
- Bảng `Accounts` có cột `Role` với giá trị:
  - `"Admin"` - Quản trị viên
  - `"Coach"` - Huấn luyện viên  
  - `"User"` - Người dùng (mặc định)

### B. **Trong JWT Token**
- Token chứa claim `ClaimTypes.Role` với giá trị role
- Được validate khi gọi API

### C. **Trong Code**
```csharp
// Sử dụng Constants
using SmokingQuitSupportAPI.Constants;

// Kiểm tra role
if (user.Role == Roles.Admin) { ... }
if (user.Role == Roles.Coach) { ... }
if (user.Role == Roles.User) { ... }
```

## 3. Cách Sử dụng Role trong Controllers

### A. **Sử dụng Attributes**

#### 1. **Chỉ Admin**
```csharp
[AdminRequired]
public class AdminController : ControllerBase
{
    // Chỉ Admin mới truy cập được
}

// Hoặc trên method
[HttpGet]
[AdminRequired]
public async Task<ActionResult> AdminOnlyMethod() { ... }
```

#### 2. **Chỉ Coach**
```csharp
[CoachRequired]
public async Task<ActionResult> CoachOnlyMethod() { ... }
```

#### 3. **Admin hoặc Coach**
```csharp
[AdminOrCoachRequired]
public async Task<ActionResult> HighPrivilegeMethod() { ... }
```

#### 4. **Bất kỳ user đã authenticate**
```csharp
[UserRequired]
public async Task<ActionResult> AuthenticatedMethod() { ... }
```

### B. **Sử dụng Policies**

#### 1. **Trong Program.cs** (đã cấu hình)
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CoachOnly", policy => policy.RequireRole("Coach"));
    options.AddPolicy("AdminOrCoach", policy => policy.RequireRole("Admin", "Coach"));
});
```

#### 2. **Sử dụng trong Controller**
```csharp
[Authorize(Policy = "AdminOnly")]
public async Task<ActionResult> AdminMethod() { ... }

[Authorize(Policy = "AdminOrCoach")]
public async Task<ActionResult> HighPrivilegeMethod() { ... }
```

## 4. Phân Quyền Chi Tiết

### 🔴 **Admin - Quyền Cao Nhất**

#### **Endpoints Admin**
- `GET /api/admin/accounts` - Xem tất cả tài khoản
- `GET /api/admin/statistics` - Thống kê hệ thống
- `PUT /api/admin/change-role/{id}` - Thay đổi role user
- `DELETE /api/admin/accounts/{id}` - Xóa tài khoản
- `GET /api/admin/recent-activities` - Xem hoạt động gần đây
- `GET /api/coach/all-coaches` - Quản lý coaches

#### **Chức Năng Admin**
- ✅ Quản lý toàn bộ users
- ✅ Thay đổi role của user khác
- ✅ Xóa tài khoản (trừ Admin khác)
- ✅ Xem thống kê tổng quan hệ thống
- ✅ Truy cập tất cả dữ liệu
- ✅ Quản lý coaches

### 🟡 **Coach - Quyền Trung Bình**

#### **Endpoints Coach**
- `GET /api/coach/my-clients` - Xem clients của mình
- `GET /api/coach/my-statistics` - Thống kê coaching
- `POST /api/coach/create-plan-for-client` - Tạo quit plan cho client
- `POST /api/coach/update-client-progress` - Cập nhật progress cho client

#### **Chức Năng Coach**
- ✅ Xem danh sách clients được assign
- ✅ Tạo quit plans cho clients
- ✅ Cập nhật progress cho clients
- ✅ Xem thống kê coaching của mình
- ✅ Truy cập dữ liệu clients được phân công
- ❌ Không thể quản lý user khác
- ❌ Không thể thay đổi role

### 🟢 **User - Quyền Cơ Bản**

#### **Endpoints User**
- `GET /api/progress/my-progress` - Xem progress của mình
- `POST /api/progress/daily` - Ghi nhận progress hàng ngày
- `GET /api/achievements` - Xem achievements
- `GET /api/quitplans` - Xem quit plans của mình
- `POST /api/communityposts` - Tạo bài viết cộng đồng
- `GET /api/dashboard` - Xem dashboard cá nhân

#### **Chức Năng User**
- ✅ Quản lý dữ liệu cá nhân
- ✅ Ghi nhận tiến trình cai thuốc
- ✅ Tham gia cộng đồng
- ✅ Xem achievements và quit plans
- ✅ Sử dụng tính năng dashboard
- ❌ Không thể truy cập dữ liệu user khác
- ❌ Không có quyền quản trị

## 5. Hướng Dẫn Test Role trên Swagger

### **Bước 1: Tạo Tài Khoản Test**

#### **Tạo Admin Account**
```json
POST /api/auth/register
{
  "username": "admin1",
  "email": "admin@test.com",
  "password": "Admin123@",
  "fullName": "System Admin",
  "role": "Admin"
}
```

#### **Tạo Coach Account**
```json
POST /api/auth/register
{
  "username": "coach1",
  "email": "coach@test.com",
  "password": "Coach123@",
  "fullName": "John Coach",
  "role": "Coach"
}
```

#### **Tạo User Account**
```json
POST /api/auth/register
{
  "username": "user1",
  "email": "user@test.com",
  "password": "User123@",
  "fullName": "Regular User",
  "role": "User"
}
```

### **Bước 2: Login và Lấy Token**

#### **Login với mỗi role**
```json
POST /api/auth/login
{
  "email": "admin@test.com",
  "password": "Admin123@"
}
```

**Copy token từ response để test**

### **Bước 3: Test Endpoints theo Role**

#### **Test Admin Endpoints**
1. Authorize với Admin token
2. Test: `GET /api/admin/accounts`
3. Test: `GET /api/admin/statistics`
4. Test: `PUT /api/admin/change-role/{id}`

#### **Test Coach Endpoints**
1. Authorize với Coach token
2. Test: `GET /api/coach/my-clients`
3. Test: `GET /api/coach/my-statistics`
4. Thử truy cập Admin endpoint → Expect 403 Forbidden

#### **Test User Endpoints**
1. Authorize với User token
2. Test: `GET /api/progress/my-progress`
3. Test: `POST /api/progress/daily`
4. Thử truy cập Admin/Coach endpoint → Expect 403 Forbidden

## 6. Error Codes và Meanings

### **Response Codes**
- **200 OK**: Thành công
- **401 Unauthorized**: Chưa đăng nhập hoặc token không hợp lệ
- **403 Forbidden**: Đã đăng nhập nhưng không có quyền truy cập
- **404 Not Found**: Resource không tồn tại

### **Common Error Messages**
```json
// Chưa đăng nhập
{
  "message": "Unauthorized"
}

// Không có quyền
{
  "message": "Forbidden"
}

// Role không hợp lệ khi đăng ký
{
  "message": "Invalid role: InvalidRole. Valid roles are: Admin, Coach, User"
}
```

## 7. Best Practices

### **A. Security Best Practices**
1. **Luôn validate role** trước khi thực hiện action
2. **Không hardcode roles** - sử dụng Constants
3. **Log security events** cho audit
4. **Implement rate limiting** cho admin endpoints

### **B. Code Best Practices**
```csharp
// ✅ Good - Sử dụng Constants
if (user.Role == Roles.Admin) { ... }

// ❌ Bad - Hardcode
if (user.Role == "Admin") { ... }

// ✅ Good - Kiểm tra quyền trước action
[AdminRequired]
public async Task<ActionResult> DeleteAccount(int id) { ... }

// ✅ Good - Validate ownership
var accountId = GetCurrentUserId();
var progress = await _context.Progress
    .FirstOrDefaultAsync(p => p.Id == id && p.AccountId == accountId);
```

### **C. Testing Best Practices**
1. **Test với từng role** riêng biệt
2. **Test unauthorized access** (expect 403)
3. **Test cross-role functionality**
4. **Test role change scenarios**

## 8. Troubleshooting

### **Common Issues**

#### **Issue: 403 Forbidden**
**Causes:**
- Token có role không đúng
- Endpoint yêu cầu role cao hơn
- Token expired

**Solutions:**
- Kiểm tra role trong token
- Re-login với account có role phù hợp
- Verify endpoint requirements

#### **Issue: Role không được validate**
**Causes:**
- Attribute không được apply
- Authorization policy không được cấu hình
- JWT token không chứa role claim

**Solutions:**
- Check attribute trên controller/method
- Verify authorization configuration
- Debug JWT token content

## 9. Extension Points

### **A. Thêm Role Mới**
1. Add constant trong `Roles.cs`
2. Update validation logic
3. Create new attributes nếu cần
4. Add authorization policies
5. Update documentation

### **B. Custom Authorization Logic**
```csharp
// Implement custom authorization handler
public class CustomAuthorizationHandler : AuthorizationHandler<CustomRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CustomRequirement requirement)
    {
        // Custom logic here
        return Task.CompletedTask;
    }
}
```

---

## 📋 **Tóm Tắt**

| Role | Mô Tả | Endpoints | Quyền Chính |
|------|-------|-----------|-------------|
| **Admin** | Quản trị viên | `/api/admin/*` | Quản lý toàn hệ thống |
| **Coach** | Huấn luyện viên | `/api/coach/*` | Hỗ trợ và coaching users |
| **User** | Người dùng | `/api/progress/*`, `/api/dashboard/*` | Sử dụng tính năng cơ bản |

**Quy tắc quan trọng**: Admin > Coach > User (từ cao xuống thấp) 