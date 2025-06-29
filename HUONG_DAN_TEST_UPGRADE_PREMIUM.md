# 🚀 HƯỚNG DẪN TEST UPGRADE PREMIUM

## **3 CÁCH ĐỂ UPGRADE PREMIUM**

### **Cách 1: FORCE UPGRADE (NHANH NHẤT) ⚡**

```http
POST https://localhost:7147/api/Package/force-upgrade-premium
Authorization: Bearer YOUR_JWT_TOKEN
```

✅ **Ưu điểm:** 
- Chỉ 1 API call
- Không cần thanh toán
- Tự động activate Premium

❌ **Nhược điểm:**
- Chỉ cho testing
- Không test được payment flow

---

### **Cách 2: UPGRADE + MANUAL VERIFICATION (KHUYẾN NGHỊ) 📋**

**Bước 1:** Upgrade package
```http
POST https://localhost:7147/api/Package/upgrade
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "targetPackageType": "PREMIUM",
  "newPackageType": "PREMIUM",
  "price": 299000,
  "durationDays": 30,
  "paymentMethod": "VNPAY",
  "paymentReference": "TEST_REF_123"
}
```

**Bước 2:** Copy `transactionId` từ response

**Bước 3:** Fake thanh toán thành công
```http
POST https://localhost:7147/api/Package/verify-payment/{transactionId}
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "transactionId": "{transactionId}",
  "paymentMethod": "VNPAY",
  "amount": 299000,
  "status": "PAID",
  "verificationDate": "2024-12-25T16:49:00.000Z",
  "verificationNotes": "FAKE PAYMENT FOR TESTING"
}
```

✅ **Ưu điểm:**
- Test được payment flow
- Giống thực tế
- Có transaction tracking

---

### **Cách 3: REAL VNPAY INTEGRATION (ĐẦY ĐỦ NHẤT) 💳**

**Bước 1:** Upgrade package (như Cách 2)

**Bước 2:** Tạo VNPay URL
```http
POST https://localhost:7147/api/Payment/vnpay/create-payment
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "subscriptionId": 1,
  "returnUrl": "https://localhost:7147/payment-result"
}
```

**Bước 3:** Mở VNPay URL và thanh toán với test data:
- Thẻ: `9704198526191432198`
- Tên: `NGUYEN VAN A`
- CVV: `123456`
- OTP: `123456`

✅ **Ưu điểm:**
- Test hoàn toàn real
- Có callback handling
- Production-ready

---

## **📝 HƯỚNG DẪN CHI TIẾT**

### **BƯỚC 1: SETUP ENVIRONMENT**

1. **Start API:**
```bash
cd BE_Updating
dotnet run
```

2. **Tạo Admin account (nếu chưa có):**
```sql
-- Run script: create_admin_account.sql
INSERT INTO Accounts (Username, Email, PasswordHash, FullName, Role, CreatedAt)
VALUES ('admin', 'admin@example.com', 'hash123', 'Admin User', 'ADMIN', GETDATE())
```

3. **Tạo Coach (nếu chưa có):**
```http
POST https://localhost:7147/api/Admin/coaches
Content-Type: application/json

{
  "username": "coach.test",
  "email": "coach@example.com",
  "password": "coach123",
  "fullName": "Dr. Test Coach",
  "qualifications": "PhD Psychology",
  "experience": "5 years",
  "bio": "Test coach for development"
}
```

### **BƯỚC 2: TẠO USER VÀ ĐĂNG NHẬP**

1. **Đăng ký user:**
```http
POST https://localhost:7147/api/Auth/register
Content-Type: application/json

{
  "username": "testuser123",
  "email": "test@example.com",
  "password": "password123",
  "fullName": "Nguyen Van Test",
  "phoneNumber": "0912345678",
  "dateOfBirth": "1990-01-01T00:00:00.000Z",
  "cigarettesPerDay": 10,
  "yearsOfSmoking": 5
}
```

2. **Đăng nhập và lấy token:**
```http
POST https://localhost:7147/api/Auth/login
Content-Type: application/json

{
  "email": "test@example.com", 
  "password": "password123"
}
```

3. **Copy JWT token từ response**

### **BƯỚC 3: KIỂM TRA PACKAGE HIỆN TẠI**

```http
GET https://localhost:7147/api/Package/my-package
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Response:**
```json
{
  "data": {
    "packageType": "BASIC",
    "status": "ACTIVE",
    "isPremium": false,
    "assignedCoachId": null
  }
}
```

### **BƯỚC 4: CHỌN CÁCH UPGRADE**

#### **Option A: Force Upgrade (Nhanh nhất)**
```http
POST https://localhost:7147/api/Package/force-upgrade-premium
Authorization: Bearer YOUR_JWT_TOKEN
```

#### **Option B: Manual Verification**
1. Upgrade package
2. Copy transactionId
3. Verify payment
(Chi tiết ở trên)

### **BƯỚC 5: KIỂM TRA PREMIUM ACTIVATED**

```http
GET https://localhost:7147/api/Package/my-package
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Response:**
```json
{
  "data": {
    "packageType": "PREMIUM",
    "status": "ACTIVE", 
    "isPremium": true,
    "daysRemaining": 30
  }
}
```

### **BƯỚC 6: TEST PREMIUM FEATURES**

1. **Xem coaches available:**
```http
GET https://localhost:7147/api/Premium/coach/available-coaches
Authorization: Bearer YOUR_JWT_TOKEN
```

2. **Chọn coach:**
```http
POST https://localhost:7147/api/Premium/coach/assign/1
Authorization: Bearer YOUR_JWT_TOKEN
```

3. **Book meeting:**
```http
POST https://localhost:7147/api/Premium/meeting/book
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "preferredDate": "2024-12-30T10:00:00.000Z",
  "meetingType": "INITIAL_CONSULTATION", 
  "notes": "Test booking sau khi upgrade Premium"
}
```

4. **Chat với coach:**
```http
POST https://localhost:7147/api/Premium/chat/send
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "receiverId": 2,
  "content": "Chào coach, tôi vừa upgrade Premium",
  "messageType": "TEXT"
}
```

---

## **🔧 TROUBLESHOOTING**

### **❌ "Invalid token"**
- Đăng nhập lại và copy token mới
- Kiểm tra token có "Bearer " ở đầu

### **❌ "Bạn cần gói Premium"**
- Kiểm tra payment verification thành công chưa
- Xem package status có "ACTIVE" không

### **❌ "Coach không tồn tại"**
- Tạo coach mới qua Admin API
- Hoặc dùng coachId khác

### **❌ "Database connection error"**
- Kiểm tra SQL Server running
- Check connection string trong appsettings.json

---

## **📊 KẾT QUẢ MONG ĐỢI**

Sau khi test thành công, user sẽ có:

✅ **Package:** PREMIUM (ACTIVE)  
✅ **Coach:** Đã được assign  
✅ **Meeting:** Đã book được  
✅ **Chat:** Có thể chat với coach  
✅ **Features:** Truy cập đầy đủ Premium features  

---

## **🧪 TEST FILES**

- `quick_test_upgrade_premium.http` - Test nhanh với VS Code
- `test_vnpay_complete_flow.ps1` - Automated PowerShell script
- `test_vnpay_payment_flow.http` - Full VNPay integration test

---

## **💡 TIPS**

1. **Dùng Force Upgrade** cho dev testing nhanh
2. **Dùng Manual Verification** cho test payment flow
3. **Dùng Real VNPay** cho production testing
4. **Tạo nhiều test users** để test concurrent access
5. **Check database** để verify data consistency

---

**�� Happy Testing!** 