# 🚭 Smoking Quit Support API - Luồng Chạy API

## 📋 Mục Lục
1. [Authentication APIs](#authentication-apis)
2. [Smoking Status APIs](#smoking-status-apis)
3. [Progress Tracking APIs](#progress-tracking-apis)
4. [Achievement APIs](#achievement-apis)
5. [Community APIs](#community-apis)
6. [Dashboard APIs](#dashboard-apis)
7. [Reports APIs](#reports-apis)

---

## 🔐 Authentication APIs

### POST `/api/auth/register` - Đăng Ký

**Luồng:**
```
Request → Validation → Check Duplicate → Hash Password → Save DB → Generate JWT → Response
```

**Chi tiết:**
1. **Request**: Client gửi `RegisterDto` (username, email, password)
2. **Validation**: Kiểm tra ModelState, required fields
3. **Check Duplicate**: Query database kiểm tra email/username đã tồn tại
4. **Hash Password**: Sử dụng BCrypt hash password
5. **Save DB**: Tạo Account entity, lưu vào database
6. **Generate JWT**: Tạo token với claims (AccountId, Role)
7. **Response**: Trả về token + user info

---

### 2. POST `/api/auth/login` - Đăng Nhập

#### **Luồng Chạy:**
```
1. Request đến Controller
   ↓
2. AuthController.Login(LoginDto)
   ↓
3. Validation LoginDto
   ↓
4. AuthService.LoginAsync()
   ↓
5. Tìm user theo email/username
   ↓
6. Verify password với BCrypt
   ↓
7. Cập nhật LastLoginAt
   ↓
8. Tạo JWT token
   ↓
9. Trả về response với token
```

---

## 🚬 Smoking Status APIs

### GET `/api/smokingstatus` - Lấy Trạng Thái

**Luồng:**
```
JWT Auth → Extract AccountId → Query DB → Map DTO → Response
```

**Chi tiết:**
1. **JWT Auth**: Middleware validate token
2. **Extract AccountId**: Lấy từ JWT Claims
3. **Query DB**: Include Account, filter by AccountId
4. **Map DTO**: Convert Entity sang SmokingStatusDto
5. **Response**: Return DTO hoặc 404 nếu không tìm thấy

### POST `/api/smokingstatus` - Tạo/Cập Nhật

**Luồng:**
```
JWT Auth → Validation → Check Exists → Calculate Stats → Save/Update → Response
```

**Tính toán tự động:**
- **SmokeFreenDays**: `(DateTime.Now - QuitDate).Days`
- **CigarettesAvoided**: `SmokeFreenDays * CigarettesPerDay`
- **MoneySaved**: `CigarettesAvoided * (CostPerPack / CigarettesPerPack)`
- **HealthImprovement**: Message động theo milestone

---

## 📊 Progress APIs

### POST `/api/progress` - Ghi Nhận Tiến Trình

**Luồng:**
```
JWT Auth → Validate Date → Check Duplicate → Calculate → Save → Update Stats → Response
```

**Logic đặc biệt:**
- Không cho phép duplicate record cùng ngày
- Tự động tính SmokeFreenDays từ quit date
- Trigger update statistics tự động
- Map mood/craving numbers sang descriptions

---

### 2. GET `/api/progress` - Lấy Danh Sách Tiến Trình

#### **Luồng Chạy:**
```
1. Request với query parameters (page, pageSize, dateFrom, dateTo)
   ↓
2. JWT Authentication
   ↓
3. ProgressService.GetUserProgressAsync()
   ↓
4. Build query với filters:
   ├─ AccountId = current user
   ├─ Date range (nếu có)
   └─ Pagination
   ↓
5. Execute query với OrderBy Date desc
   ↓
6. Map to ProgressDto collection
   ↓
7. Return paginated result
```

---

## 🏆 Achievement APIs

### GET `/api/achievement/user` - Huy Hiệu User

**Luồng:**
```
JWT Auth → Query All Achievements → Calculate Progress → Check Unlocked → Map DTO → Response
```

**Progress calculation:**
```csharp
achievementType switch
{
    "SMOKE_FREE_DAYS" => GetSmokeFreenDays(accountId),
    "MONEY_SAVED" => GetMoneySaved(accountId), 
    "POSTS_CREATED" => GetPostsCreated(accountId),
    _ => 0
};
```

### POST `/api/achievement/check` - Kiểm Tra Unlock

**Luồng:**
```
Get Unlocked → Filter Available → Calculate Progress → Auto Unlock → Save → Response
```

---

## 👥 Community APIs

### GET `/api/communitypost` - Danh Sách Bài Viết

**Luồng:**
```
Build Query → Apply Filters → Include Relations → Paginate → Map DTO → Response
```

**Query optimization:**
- Include Author, Comments, Likes trong 1 query
- Filter by category, search text
- Order by CreatedAt desc
- Pagination để performance

---

### 2. POST `/api/communitypost` - Tạo Bài Viết Mới

#### **Luồng Chạy:**
```
1. Request với CreateCommunityPostDto
   ↓
2. JWT Authentication
   ↓
3. Validation DTO (Title, Content required)
   ↓
4. CommunityPostService.CreatePostAsync()
   ↓
5. Tạo CommunityPost entity:
   ├─ AuthorId = current user
   ├─ Title, Content từ DTO
   ├─ Category (default hoặc từ DTO)
   ├─ IsActive = true
   └─ CreatedAt = DateTime.UtcNow
   ↓
6. Save to database
   ↓
7. Trigger achievement check (nếu có):
   ├─ Increment POSTS_CREATED counter
   └─ Check achievement unlock
   ↓
8. Return CommunityPostDto
```

---

## 📈 Dashboard APIs

### GET `/api/dashboard/overview` - Tổng Quan

**Luồng:**
```
Parallel Queries → Aggregate Data → Calculate Summary → Format Response
```

**Parallel execution:**
```csharp
Task.WhenAll(
    GetSmokingStatus(),
    GetRecentProgress(), 
    GetAchievements(),
    GetActivePlan()
);
```

---

### 2. GET `/api/dashboard/weekly-report` - Báo Cáo Tuần

#### **Luồng Chạy:**
```
1. Tính toán date range (7 ngày gần nhất)
   ↓
2. ProgressService.GetProgressByDateRangeAsync()
   ↓
3. Query progress records trong khoảng thời gian
   ↓
4. Aggregate statistics:
   ├─ Total smoke-free days
   ├─ Total cigarettes smoked
   ├─ Total money saved
   ├─ Average stress level
   └─ Success rate
   ↓
5. Group data by day cho chart
   ↓
6. Return structured report
```

---

## 📋 Reports APIs

### 1. GET `/api/report/progress` - Báo Cáo Tiến Trình Chi Tiết

#### **Luồng Chạy:**
```
1. Request với date range parameters
   ↓
2. JWT Authentication
   ↓
3. ReportController.GetProgressReport()
   ↓  
4. Validation date range (max 90 days)
   ↓
5. ProgressService.GetDetailedProgressReportAsync()
   ↓
6. Complex aggregation query:
   ├─ Group by week/month
   ├─ Calculate trend indicators
   ├─ Identify patterns
   └─ Generate insights
   ↓
7. Format data cho charts:
   ├─ Line chart data
   ├─ Bar chart data
   └─ Pie chart data
   ↓
8. Return comprehensive report
```

---

## 🔄 Background Processes và Scheduled Tasks

### 1. Daily Statistics Update
```
1. Scheduled task chạy hàng ngày lúc 00:00
   ↓
2. Update SmokingStatus statistics:
   ├─ Recalculate SmokeFreenDays
   ├─ Update MoneySaved
   ├─ Update CigarettesAvoided
   └─ Update HealthImprovement messages
   ↓
3. Check achievements cho tất cả users
   ↓
4. Generate daily insights
```

### 2. Achievement Auto-Check
```
1. Trigger sau mỗi user action:
   ├─ Create progress record
   ├─ Create community post
   ├─ Complete quit plan milestone
   └─ Update smoking status
   ↓
2. Background job:
   ├─ Calculate current progress
   ├─ Compare with achievement requirements
   ├─ Auto unlock qualified achievements
   └─ Send notifications
```

---

## 🔒 Security & Authentication

### JWT Flow:
```
Client Request → JWT Middleware → Validate Token → Extract Claims → Set User Identity → Continue
```

### Token Validation:
- Signature verification
- Expiry check  
- Issuer/Audience validation
- Claims extraction (AccountId, Role)

---

## 🎯 Performance Patterns

### Database Optimization:
- **Projection**: `Select()` thay vì load full entities
- **Eager Loading**: `Include()` cho related data
- **Pagination**: Skip/Take cho large datasets
- **Async**: Tất cả DB operations non-blocking

### Caching Strategy:
- Memory cache cho reference data
- Query result caching
- Response caching cho static endpoints

*API đã sẵn sàng chạy trên http://localhost:5000 🚀* 