using Microsoft.AspNetCore.Authorization;
using SmokingQuitSupportAPI.Constants;

namespace SmokingQuitSupportAPI.Attributes
{
    /// <summary>
    /// Custom attribute để kiểm tra role required
    /// </summary>
    public class RoleRequiredAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Constructor yêu cầu một role cụ thể
        /// </summary>
        /// <param name="role">Role được yêu cầu</param>
        public RoleRequiredAttribute(string role) : base()
        {
            this.Roles = role;
        }

        /// <summary>
        /// Constructor yêu cầu nhiều roles
        /// </summary>
        /// <param name="roles">Danh sách roles được yêu cầu</param>
        public RoleRequiredAttribute(params string[] roles) : base()
        {
            this.Roles = string.Join(",", roles);
        }
    }

    /// <summary>
    /// Attribute yêu cầu quyền Admin
    /// </summary>
    public class AdminRequiredAttribute : AuthorizeAttribute
    {
        public AdminRequiredAttribute() 
        {
            this.Roles = "Admin";
        }
    }

    /// <summary>
    /// Attribute yêu cầu quyền Coach
    /// </summary>
    public class CoachRequiredAttribute : AuthorizeAttribute
    {
        public CoachRequiredAttribute() 
        {
            this.Roles = "Coach";
        }
    }

    /// <summary>
    /// Attribute yêu cầu quyền Admin hoặc Coach
    /// </summary>
    public class AdminOrCoachRequiredAttribute : AuthorizeAttribute
    {
        public AdminOrCoachRequiredAttribute() 
        {
            this.Roles = "Admin,Coach";
        }
    }

    /// <summary>
    /// Attribute yêu cầu người dùng phải được authenticate (bất kỳ role nào)
    /// </summary>
    public class UserRequiredAttribute : AuthorizeAttribute
    {
        public UserRequiredAttribute() : base() { }
    }
} 