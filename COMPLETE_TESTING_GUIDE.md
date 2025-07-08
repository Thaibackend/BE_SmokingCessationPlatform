# 🎯 COMPLETE TESTING GUIDE - SMOKING QUIT SUPPORT API

## 📋 **Tổng quan dự án**

**Smoking Quit Support API** là hệ thống hỗ trợ cai thuốc lá với 3 roles chính:
- **User**: Người dùng cai thuốc lá
- **Coach**: Chuyên gia hỗ trợ cai thuốc  
- **Admin**: Quản trị viên hệ thống

## 🏗️ **Kiến trúc hệ thống**

```
┌─────────────────┬─────────────────┬─────────────────┐
│     USER        │     COACH       │     ADMIN       │
├─────────────────┼─────────────────┼─────────────────┤
│ • Basic Package │ • Coach Panel   │ • User Mgmt     │
│ • Premium Upgrade│ • Chat Support  │ • Coach Mgmt    │
│ • Progress Track│ • Meeting Mgmt  │ • Package Mgmt  │
│ • Community     │ • Progress View │ • Statistics    │
│ • Achievements  │ • Reports       │ • Revenue Track │
└─────────────────┴─────────────────┴─────────────────┘
```

## 🚀 **TESTING WORKFLOW BY ROLES**

---

# 👤 **1. USER ROLE TESTING**

## **1.1. User Registration & Basic Setup**

### **Step 1: Đăng ký User**
```http
POST /api/Auth/register
Content-Type: application/json

{
  "username": "user001",
  "email": "user@test.com",
  "password": "password123",
  "fullName": "Nguyen Van User",
  "role": "User"
}
```

### **Step 2: Đăng nhập User**
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "user@test.com",
  "password": "password123"
}
```
📝 **→ Copy JWT token để dùng cho các request tiếp theo**

### **Step 3: Tạo Smoking Status Profile**
```http
POST /api/SmokingStatus
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "quitDate": "2024-12-26T00:00:00Z",
  "cigarettesPerDay": 15,
  "yearsOfSmoking": 8,
  "costPerPack": 50000,
  "cigarettesPerPack": 20
}
```

## **1.2. Basic Package Features (Miễn phí)**

### **Package Management**
```http
# Xem package hiện tại (auto-create Basic)
GET /api/Package/my-package
Authorization: Bearer USER_JWT_TOKEN

# Xem suggested quit plan  
GET /api/Package/suggested-quit-plan
Authorization: Bearer USER_JWT_TOKEN

# Xem available features
GET /api/Package/available-features
Authorization: Bearer USER_JWT_TOKEN
```

### **Progress Tracking**
```http
# Xem progress hiện tại
GET /api/Progress/my-progress
Authorization: Bearer USER_JWT_TOKEN

# Tạo daily progress
POST /api/Progress
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "date": "2024-12-26T00:00:00Z",
  "healthScore": 85,
  "notes": "Hôm nay cảm thấy khỏe hơn",
  "mood": 4,
  "cravingLevel": 2,
  "weight": 65.5,
  "exerciseMinutes": 30,
  "sleepHours": 8.0
}

# Xem statistics
GET /api/Progress/statistics
Authorization: Bearer USER_JWT_TOKEN
```

### **Quit Plan Management**
```http
# Tạo quit plan
POST /api/QuitPlan
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "name": "My 30-day Quit Plan",
  "description": "Kế hoạch cai thuốc trong 30 ngày",
  "startDate": "2024-12-26T00:00:00Z",
  "endDate": "2025-01-26T00:00:00Z",
  "targetCigarettesPerDay": 0
}

# Xem quit plans của mình
GET /api/QuitPlan/my-plans
Authorization: Bearer USER_JWT_TOKEN

# Xem quit plan active
GET /api/QuitPlan/active-plan
Authorization: Bearer USER_JWT_TOKEN
```

### **Community Features**
```http
# Xem posts cộng đồng
GET /api/CommunityPost?page=1&pageSize=10

# Tạo post mới
POST /api/CommunityPost
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "title": "Ngày đầu cai thuốc",
  "content": "Hôm nay là ngày đầu tiên tôi cai thuốc...",
  "category": "EXPERIENCE"
}

# Like post
POST /api/CommunityPost/1/like
Authorization: Bearer USER_JWT_TOKEN

# Comment post
POST /api/CommunityPost/1/comment
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "content": "Chúc bạn thành công!"
}
```

### **Achievements**
```http
# Xem achievements của mình
GET /api/Achievement/my-achievements
Authorization: Bearer USER_JWT_TOKEN

# Xem available achievements
GET /api/Achievement/available

# Check new achievements
POST /api/Achievement/check-new-achievements
Authorization: Bearer USER_JWT_TOKEN
```

## **1.3. Premium Upgrade Workflow**

### **Step 1: Check Premium Access (Before Upgrade)**
```http
GET /api/Package/premium-access
Authorization: Bearer USER_JWT_TOKEN
# Expected: hasPremiumAccess = false
```

### **Step 2: Test Premium Restriction**
```http
GET /api/Premium/coach/available-coaches
Authorization: Bearer USER_JWT_TOKEN
# Expected: 403 Forbidden
```

### **Step 3: Force Upgrade to Premium (Testing)**
```http
POST /api/Package/force-upgrade-premium
Authorization: Bearer USER_JWT_TOKEN
# Expected: 200 OK, upgrade success
```

### **Step 4: Verify Premium Access**
```http
GET /api/Package/premium-access
Authorization: Bearer USER_JWT_TOKEN
# Expected: hasPremiumAccess = true

GET /api/Package/my-package
Authorization: Bearer USER_JWT_TOKEN
# Expected: PackageType = "PREMIUM"
```

## **1.4. Premium Features Testing**

### **Coach Management**
```http
# Xem coaches available
GET /api/Premium/coach/available-coaches
Authorization: Bearer USER_JWT_TOKEN

# Assign coach
POST /api/Premium/coach/assign/1
Authorization: Bearer USER_JWT_TOKEN

# Xem coach hiện tại
GET /api/Premium/coach/my-coach
Authorization: Bearer USER_JWT_TOKEN
```

### **Chat với Coach**
```http
# Gửi message
POST /api/Premium/chat/send
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "receiverId": 2,
  "content": "Chào coach, tôi cần hỗ trợ cai thuốc",
  "messageType": "TEXT"
}

# Xem chat history
GET /api/Premium/chat/history/2?pageNumber=1&pageSize=20
Authorization: Bearer USER_JWT_TOKEN

# Mark messages as read
POST /api/Premium/chat/mark-read
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

[1, 2, 3]
```

### **Meeting với Coach**
```http
# Book meeting
POST /api/Premium/meeting/book
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "preferredDate": "2024-12-30T10:00:00Z",
  "meetingType": "INITIAL_CONSULTATION",
  "notes": "Cần tư vấn kế hoạch cai thuốc"
}

# Xem meetings
GET /api/Premium/meeting/my-meetings
Authorization: Bearer USER_JWT_TOKEN
```

### **Stage Progress (Premium)**
```http
# Xem current stage
GET /api/Premium/stage/current
Authorization: Bearer USER_JWT_TOKEN

# Update stage progress
PUT /api/Premium/stage/update-progress
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

{
  "progressPercentage": 25,
  "userNotes": "Đã cai được 1 tuần",
  "cigarettesSmoked": 0,
  "cravingLevel": 3,
  "stressLevel": 4,
  "supportLevel": 8,
  "achievements": "Không hút thuốc 7 ngày liên tiếp"
}

# Advance to next stage
POST /api/Premium/stage/advance
Authorization: Bearer USER_JWT_TOKEN
Content-Type: application/json

"EARLY_RECOVERY"

# Xem stage history
GET /api/Premium/stage/history
Authorization: Bearer USER_JWT_TOKEN
```

---

# 👨‍💼 **2. COACH ROLE TESTING**

## **2.1. Coach Setup**

### **Step 1: Đăng ký Coach (bởi Admin)**
```http
POST /api/Admin/coaches
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "username": "coach001",
  "email": "coach@test.com",
  "password": "coach123",
  "fullName": "Dr. Coach Expert",
  "phone": "0900000001",
  "specialization": "Smoking Cessation Therapy",
  "bio": "Chuyên gia tâm lý với 10 năm kinh nghiệm hỗ trợ cai thuốc lá"
}
```

### **Step 2: Coach Login**
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "coach@test.com",
  "password": "coach123"
}
```

## **2.2. Coach Features**

### **Client Management**
```http
# Xem clients được assign
GET /api/Coach/my-clients
Authorization: Bearer COACH_JWT_TOKEN

# Xem client detail
GET /api/Coach/clients/1
Authorization: Bearer COACH_JWT_TOKEN

# Xem client progress
GET /api/Coach/clients/1/progress
Authorization: Bearer COACH_JWT_TOKEN
```

### **Meeting Management**
```http
# Xem meetings của coach
GET /api/Coach/meetings
Authorization: Bearer COACH_JWT_TOKEN

# Update meeting status
PUT /api/Coach/meetings/1/status
Authorization: Bearer COACH_JWT_TOKEN
Content-Type: application/json

"COMPLETED"

# Add meeting notes
POST /api/Coach/meetings/1/notes
Authorization: Bearer COACH_JWT_TOKEN
Content-Type: application/json

{
  "notes": "Client đã có tiến bộ rõ rệt, giảm được 50% số điếu/ngày"
}
```

### **Chat Support**
```http
# Reply to client messages
POST /api/Coach/chat/reply
Authorization: Bearer COACH_JWT_TOKEN
Content-Type: application/json

{
  "clientId": 1,
  "content": "Chúc mừng bạn đã đạt được mục tiêu tuần này!",
  "messageType": "TEXT"
}

# Xem chat history với client
GET /api/Coach/chat/client/1
Authorization: Bearer COACH_JWT_TOKEN
```

---

# 👨‍💻 **3. ADMIN ROLE TESTING**

## **3.1. Admin Setup**

### **Step 1: Đăng ký Admin**
```http
POST /api/Auth/register
Content-Type: application/json

{
  "username": "admin001",
  "email": "admin@test.com",
  "password": "admin123",
  "fullName": "System Administrator",
  "role": "Admin"
}
```

### **Step 2: Admin Login**
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "admin123"
}
```

## **3.2. User Management**

```http
# Xem tất cả accounts
GET /api/Admin/accounts
Authorization: Bearer ADMIN_JWT_TOKEN

# Xem user details
GET /api/Admin/users/1
Authorization: Bearer ADMIN_JWT_TOKEN

# Change user role
PUT /api/Admin/change-role/1
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "newRole": "Coach"
}

# Block user
PUT /api/Admin/users/1/block
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "reason": "Vi phạm quy định cộng đồng",
  "blockDays": 7
}

# Unblock user
PUT /api/Admin/users/1/unblock
Authorization: Bearer ADMIN_JWT_TOKEN
```

## **3.3. Coach Management**

```http
# Xem tất cả coaches
GET /api/Admin/coaches
Authorization: Bearer ADMIN_JWT_TOKEN

# Tạo coach mới
POST /api/Admin/coaches
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "username": "newcoach",
  "email": "newcoach@test.com",
  "password": "coach123",
  "fullName": "New Coach Expert",
  "phone": "0900000002",
  "specialization": "Behavioral Therapy",
  "bio": "Chuyên gia tâm lý hành vi"
}

# Update coach
PUT /api/Admin/coaches/1
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "specialization": "Advanced Smoking Cessation",
  "bio": "Updated bio with more experience",
  "status": "ACTIVE"
}
```

## **3.4. Package Management**

```http
# Xem tất cả packages
GET /api/Admin/packages
Authorization: Bearer ADMIN_JWT_TOKEN

# Tạo package mới
POST /api/Admin/packages
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "name": "VIP Package",
  "description": "Gói VIP với coach chuyên gia",
  "price": 999000,
  "durationDays": 90,
  "features": "Coach 1:1, Daily support, Custom plan",
  "isActive": true
}

# Update package
PUT /api/Admin/packages/1
Authorization: Bearer ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "price": 1200000,
  "features": "Updated features list",
  "isActive": true
}
```

## **3.5. Statistics & Analytics**

```http
# System overview
GET /api/Admin/statistics
Authorization: Bearer ADMIN_JWT_TOKEN

# User statistics
GET /api/Admin/statistics/users
Authorization: Bearer ADMIN_JWT_TOKEN

# Revenue statistics
GET /api/Admin/statistics/revenue
Authorization: Bearer ADMIN_JWT_TOKEN

# Quit success statistics
GET /api/Admin/statistics/quit-success
Authorization: Bearer ADMIN_JWT_TOKEN

# Recent activities
GET /api/Admin/recent-activities
Authorization: Bearer ADMIN_JWT_TOKEN
```

---

# 🔄 **4. COMPLETE USER JOURNEY TESTING**

## **Scenario 1: New User to Premium Success**

### **Phase 1: Onboarding (10 minutes)**
1. ✅ User đăng ký account
2. ✅ Tạo smoking status profile
3. ✅ Auto-create Basic package
4. ✅ Generate suggested quit plan
5. ✅ Track first day progress

### **Phase 2: Basic Usage (1 week)**
1. ✅ Daily progress tracking
2. ✅ Community participation (posts, comments)
3. ✅ Achievement unlocking
4. ✅ Brinkman Index monitoring

### **Phase 3: Premium Upgrade (Decision point)**
1. ✅ Check Premium features
2. ✅ Compare Basic vs Premium
3. ✅ Upgrade decision & payment
4. ✅ Coach assignment

### **Phase 4: Premium Experience (1 month)**
1. ✅ Chat with coach
2. ✅ Schedule & attend meetings
3. ✅ Stage-by-stage progress
4. ✅ Personalized support

### **Phase 5: Success Metrics**
1. ✅ Reduced cigarettes/day
2. ✅ Money saved tracking
3. ✅ Improved health scores
4. ✅ Community contributions

---

# 🧪 **5. TESTING SCENARIOS**

## **A. Happy Path Testing**
- ✅ All APIs return 200 OK
- ✅ Data persists correctly
- ✅ Role permissions work
- ✅ Business logic executes

## **B. Error Testing**
- ❌ 401 Unauthorized (no token)
- ❌ 403 Forbidden (wrong role)
- ❌ 400 Bad Request (invalid data)
- ❌ 404 Not Found (resource missing)
- ❌ 500 Internal Error (server issues)

## **C. Edge Cases**
- 🔍 User upgrades multiple times
- 🔍 Coach handles 50+ clients
- 🔍 Admin manages 1000+ users
- 🔍 Database performance under load

## **D. Security Testing**
- 🔒 JWT token validation
- 🔒 Role-based access control
- 🔒 SQL injection prevention
- 🔒 Data encryption compliance

---

# 📊 **6. BUSINESS METRICS TO TRACK**

## **User Engagement**
- Daily active users
- Progress tracking frequency  
- Community post engagement
- Achievement completion rate

## **Premium Conversion**
- Basic to Premium upgrade rate
- Average time to upgrade
- Premium feature usage
- Coach satisfaction ratings

## **Health Outcomes**
- Average cigarettes reduction
- Quit success rate
- Money saved per user
- Health improvement scores

## **System Performance**
- API response times
- Database query efficiency
- Error rates by endpoint
- User session duration

---

# 🎯 **7. TESTING CHECKLIST**

## **Before Testing**
- [ ] Database migrated and seeded
- [ ] JWT configuration correct
- [ ] All services registered in DI
- [ ] Connection strings valid

## **Authentication Testing**
- [ ] User registration works
- [ ] Login returns valid JWT
- [ ] Token validation works
- [ ] Role-based access enforced

## **Core Features Testing**
- [ ] Package system (Basic/Premium)
- [ ] Progress tracking accuracy
- [ ] Quit plan functionality
- [ ] Community features

## **Premium Features Testing**
- [ ] Coach assignment
- [ ] Chat system
- [ ] Meeting booking
- [ ] Stage progression

## **Admin Features Testing**
- [ ] User management
- [ ] Coach management  
- [ ] Package management
- [ ] Statistics accuracy

## **Integration Testing**
- [ ] Full user journey
- [ ] Cross-role interactions
- [ ] Payment workflow
- [ ] Data consistency

---

# 🚀 **8. RECOMMENDED TESTING ORDER**

1. **Setup & Authentication** (30 minutes)
   - Test all 3 role registrations
   - Verify JWT tokens work
   - Test role permissions

2. **User Basic Features** (45 minutes)
   - Complete user onboarding
   - Test all Basic package features
   - Verify community functionality

3. **Premium Upgrade Flow** (30 minutes)
   - Test upgrade process
   - Verify Premium features unlock
   - Test coach assignment

4. **Coach Features** (30 minutes)
   - Test coach panel access
   - Test client management
   - Test chat & meetings

5. **Admin Features** (45 minutes)
   - Test user management
   - Test coach management
   - Test package management
   - Verify statistics

6. **Integration Testing** (60 minutes)
   - Complete user journeys
   - Cross-role interactions
   - Error scenarios
   - Performance testing

**Total Testing Time: ~4 hours for comprehensive coverage**

---

# 📝 **9. TESTING TOOLS SETUP**

## **Recommended Tools:**
- **Postman/Insomnia**: API testing
- **Swagger UI**: Interactive documentation
- **HTTP files**: VS Code REST Client
- **Database client**: Query and verify data

## **Testing Files Provided:**
- `test_real_apis.http` - All APIs by controller
- `test_premium_upgrade.http` - Premium upgrade flow
- `test_admin_package.http` - Admin functions
- `test_database_connection.http` - Connection debugging

---

# 🎉 **SUCCESS CRITERIA**

Your Smoking Quit Support API is ready for production when:

✅ **All 3 roles can authenticate and access their features**
✅ **Users can complete the full journey from Basic to Premium**  
✅ **Coaches can effectively manage and support clients**
✅ **Admins can manage the entire system**
✅ **Business metrics are tracked accurately**
✅ **System performs well under normal load**
✅ **Security measures are properly implemented**

**Good luck with your testing! 🚀** 