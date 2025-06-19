using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.SmokingStatus
{
    /// <summary>
    /// DTO để tạo/cập nhật tình trạng hút thuốc
    /// </summary>
    public class CreateSmokingStatusDto
    {
        [Required(ErrorMessage = "Ngày bắt đầu cai thuốc là bắt buộc")]
        public DateTime QuitDate { get; set; }
        
        [Required(ErrorMessage = "Số điếu thuốc mỗi ngày là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Số điếu thuốc mỗi ngày phải từ 1 đến 100")]
        public int CigarettesPerDay { get; set; }
        
        [Required(ErrorMessage = "Số năm hút thuốc là bắt buộc")]
        [Range(1, 80, ErrorMessage = "Số năm hút thuốc phải từ 1 đến 80")]
        public int YearsOfSmoking { get; set; }
        
        [Required(ErrorMessage = "Giá tiền một gói thuốc là bắt buộc")]
        [Range(0.01, 1000000, ErrorMessage = "Giá tiền phải lớn hơn 0")]
        public decimal CostPerPack { get; set; }
        
        [Required(ErrorMessage = "Số điếu thuốc trong một gói là bắt buộc")]
        [Range(1, 50, ErrorMessage = "Số điếu thuốc trong gói phải từ 1 đến 50")]
        public int CigarettesPerPack { get; set; }
    }
} 