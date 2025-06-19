using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.QuitPlan
{
    /// <summary>
    /// DTO để tạo kế hoạch cai thuốc mới
    /// </summary>
    public class CreateQuitPlanDto
    {
        [Required(ErrorMessage = "Tên kế hoạch là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên kế hoạch không được quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required(ErrorMessage = "Gói thành viên là bắt buộc")]
        public int PackageId { get; set; }
        
        public int? CoachId { get; set; }
        
        // Lý do cai thuốc
        [StringLength(1000, ErrorMessage = "Lý do cai thuốc không được quá 1000 ký tự")]
        public string? QuitReason { get; set; }
    }
} 