using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Progress
{
    /// <summary>
    /// DTO để ghi nhận tiến trình cai thuốc hàng ngày
    /// </summary>
    public class CreateProgressDto
    {
        [Required(ErrorMessage = "Ngày ghi nhận là bắt buộc")]
        public DateTime Date { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Số điếu thuốc tránh được phải >= 0")]
        public int CigarettesAvoided { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền tiết kiệm phải >= 0")]
        public decimal MoneySaved { get; set; }
        
        [Range(1, 10, ErrorMessage = "Điểm sức khỏe phải từ 1 đến 10")]
        public int? HealthScore { get; set; }
        
        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string? Notes { get; set; }
        
        [Range(1, 5, ErrorMessage = "Tâm trạng phải từ 1 đến 5")]
        public int? Mood { get; set; }
        
        [Range(1, 5, ErrorMessage = "Mức độ thèm thuốc phải từ 1 đến 5")]
        public int? CravingLevel { get; set; }
        
        [Range(0, 500, ErrorMessage = "Cân nặng phải từ 0 đến 500 kg")]
        public decimal? Weight { get; set; }
        
        [Range(0, 1440, ErrorMessage = "Thời gian tập thể dục phải từ 0 đến 1440 phút")]
        public int? ExerciseMinutes { get; set; }
        
        [Range(0, 24, ErrorMessage = "Giờ ngủ phải từ 0 đến 24 giờ")]
        public decimal? SleepHours { get; set; }
    }
} 