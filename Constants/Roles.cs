namespace SmokingQuitSupportAPI.Constants
{
    /// <summary>
    /// Định nghĩa các roles trong hệ thống
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Coach = "Coach";
        public const string User = "User";

        /// <summary>
        /// Danh sách tất cả roles
        /// </summary>
        public static readonly string[] AllRoles = { Admin, Coach, User };

        /// <summary>
        /// Roles có quyền cao (Admin + Coach)
        /// </summary>
        public static readonly string[] HighPrivilegeRoles = { Admin, Coach };

        /// <summary>
        /// Kiểm tra role có hợp lệ không
        /// </summary>
        /// <param name="role">Role cần kiểm tra</param>
        /// <returns>True nếu role hợp lệ</returns>
        public static bool IsValidRole(string role)
        {
            return AllRoles.Contains(role);
        }

        /// <summary>
        /// Lấy mô tả của role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>Mô tả role</returns>
        public static string GetRoleDescription(string role)
        {
            return role switch
            {
                Admin => "Quản trị viên - Có quyền quản lý toàn bộ hệ thống",
                Coach => "Huấn luyện viên - Có quyền hỗ trợ và tư vấn người dùng",
                User => "Người dùng - Có quyền sử dụng các tính năng cơ bản",
                _ => "Role không xác định"
            };
        }
    }
} 