# Hướng Dẫn Test API trên Swagger UI

## 1. Truy Cập Swagger UI

1. **Khởi động API:**
   ```bash
   dotnet run --launch-profile http
   ```

2. **Mở trình duyệt và truy cập:**
   ```
   http://localhost:5000/swagger
   ```

## 2. Xác Thực (Authentication)

### Bước 1: Đăng Ký Tài Khoản
1. Mở section **Auth**
2. Click **POST /api/auth/register**
3. Click **Try it out**
4. Nhập dữ liệu mẫu:
```json
{
  "email": "test@example.com",
  "password": "Test123@",
  "confirmPassword": "Test123@",
  "fullName": "Test User",
  "phoneNumber": "0123456789",
  "dateOfBirth": "1990-01-01T00:00:00.000Z",
  "smokingStartDate": "2010-01-01T00:00:00.000Z",
  "dailyCigarettes": 20,
  "cigarettePrice": 25000,
  "membershipPackage": "Basic"
}
```
5. Click **Execute**
6. Kiểm tra response status **200 OK**

### Bước 2: Đăng Nhập
1. Click **POST /api/auth/login**
2. Click **Try it out**
3. Nhập:
```json
{
  "email": "test@example.com",
  "password": "Test123@"
}
```
4. Click **Execute**
5. **Copy token** từ response

### Bước 3: Authorize
1. Click nút **Authorize** ở đầu trang Swagger
2. Nhập: `Bearer [your-token]`
3. Click **Authorize**
4. Click **Close**

## 3. Test Các Controller

### A. Progress Controller

#### Test GET Progress
1. **GET /api/progress**
   - Click **Try it out** → **Execute**
   - Kỳ vọng: Danh sách progress records

#### Test Create Progress
1. **POST /api/progress**
   - Dữ liệu mẫu:
```json
{
  "date": "2024-01-15T00:00:00.000Z",
  "mood": 4,
  "cravingLevel": 3,
  "weight": 70.5,
  "exerciseMinutes": 30,
  "sleepHours": 8,
  "notes": "Feeling good today!"
}
```

#### Test Update Progress
1. **PUT /api/progress/{id}**
   - Sử dụng ID từ response trước
   - Update dữ liệu:
```json
{
  "mood": 5,
  "cravingLevel": 2,
  "weight": 70.3,
  "exerciseMinutes": 45,
  "sleepHours": 8.5,
  "notes": "Even better today!"
}
```

### B. Achievement Controller

#### Test Get Achievements
1. **GET /api/achievements**
   - Kỳ vọng: Danh sách achievements với progress

#### Test Unlock Achievement
1. **POST /api/achievements/{id}/unlock**
   - Chọn một achievement ID
   - Kỳ vọng: Achievement được unlock

### C. Quit Plan Controller

#### Test Get Quit Plans
1. **GET /api/quitplans**
   - Kỳ vọng: Danh sách quit plans

#### Test Create Quit Plan
1. **POST /api/quitplans**
```json
{
  "title": "30 Days Challenge",
  "description": "Complete 30-day smoke-free challenge",
  "targetDays": 30,
  "strategies": [
    "Use nicotine gum",
    "Exercise daily",
    "Avoid triggers"
  ]
}
```

### D. Smoking Status Controller

#### Test Get Smoking Status
1. **GET /api/smokingstatus**
   - Kỳ vọng: Current smoking status

#### Test Update Smoking Status
1. **PUT /api/smokingstatus**
```json
{
  "smokeFreenDays": 10,
  "totalCigarettesAvoided": 200,
  "moneySaved": 500000,
  "healthImprovement": "Lung capacity improved by 10%"
}
```

### E. Community Post Controller

#### Test Get Posts
1. **GET /api/communityposts**
   - Kỳ vọng: Danh sách posts

#### Test Create Post
1. **POST /api/communityposts**
```json
{
  "title": "My 30-day journey",
  "content": "Sharing my experience after 30 days smoke-free...",
  "category": "Success Story"
}
```

#### Test Like Post
1. **POST /api/communityposts/{id}/like**
   - Sử dụng post ID vừa tạo

#### Test Add Comment
1. **POST /api/communityposts/{id}/comments**
```json
{
  "content": "Great story! Keep it up!"
}
```

### F. Dashboard Controller

#### Test Dashboard Data
1. **GET /api/dashboard**
   - Kỳ vọng: Dashboard analytics data

## 4. Test Cases Quan Trọng

### A. Authentication Flow
```
1. Register → Login → Get Token → Use Token
2. Test expired token
3. Test invalid credentials
```

### B. Data Validation
```
1. Empty required fields
2. Invalid email format
3. Weak password
4. Invalid date formats
5. Negative numbers where inappropriate
```

### C. Authorization
```
1. Access endpoints without token
2. Access other user's data
3. Admin vs User permissions
```

### D. Business Logic
```
1. Progress tracking sequence
2. Achievement unlock conditions
3. Smoking status calculations
4. Community interaction rules
```

## 5. Kiểm Tra Response

### Success Responses
- **200 OK**: Successful GET/PUT
- **201 Created**: Successful POST
- **204 No Content**: Successful DELETE

### Error Responses
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Missing/invalid token
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server errors

## 6. Test Data Mẫu

### User Registration
```json
{
  "email": "user1@test.com",
  "password": "Test123@",
  "confirmPassword": "Test123@",
  "fullName": "Nguyễn Văn A",
  "phoneNumber": "0901234567",
  "dateOfBirth": "1985-05-15T00:00:00.000Z",
  "smokingStartDate": "2005-01-01T00:00:00.000Z",
  "dailyCigarettes": 15,
  "cigarettePrice": 30000,
  "membershipPackage": "Premium"
}
```

### Progress Entry
```json
{
  "date": "2024-01-16T00:00:00.000Z",
  "mood": 3,
  "cravingLevel": 4,
  "weight": 68.2,
  "exerciseMinutes": 0,
  "sleepHours": 6.5,
  "notes": "Tough day, had strong cravings"
}
```

### Community Post
```json
{
  "title": "Tips để vượt qua cơn thèm",
  "content": "Chia sẻ một số mẹo giúp tôi vượt qua những cơn thèm thuốc khó khăn nhất...",
  "category": "Tips & Tricks"
}
```

## 7. Lưu Ý Khi Test

### Performance Testing
- Test với nhiều records
- Test concurrent requests
- Monitor response times

### Security Testing
- SQL injection attempts
- XSS attempts
- Invalid token formats
- Rate limiting

### Edge Cases
- Empty databases
- Maximum field lengths
- Special characters
- Different time zones

## 8. Automated Testing Script

### Postman Collection
Tạo collection với:
1. Environment variables (baseUrl, token)
2. Pre-request scripts for authentication
3. Test assertions
4. Data-driven testing

### Sample Newman Command
```bash
newman run smoking-quit-api.postman_collection.json -e production.postman_environment.json
```

## 9. Common Issues & Solutions

### Issue: 401 Unauthorized
**Solution**: 
- Check token format: `Bearer [token]`
- Verify token hasn't expired
- Re-login if needed

### Issue: 400 Bad Request
**Solution**:
- Check required fields
- Validate data formats
- Review API documentation

### Issue: 500 Internal Server Error
**Solution**:
- Check server logs
- Verify database connection
- Check service implementations

## 10. Best Practices

1. **Always authenticate first** before testing protected endpoints
2. **Use realistic test data** that matches business requirements
3. **Test both happy path and error scenarios**
4. **Verify response data structure** matches documentation
5. **Clean up test data** after testing
6. **Document any bugs found** with reproduction steps
7. **Test API version compatibility**
8. **Verify CORS settings** for frontend integration

---

**Lưu ý**: Hướng dẫn này dành cho việc test API trong môi trường development. Đối với production, cần thêm các biện pháp bảo mật và monitoring phù hợp. 