using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Package;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller xử lý các tính năng Premium: Chat, Meeting, Stage Management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PremiumController : ControllerBase
    {
        private readonly IPremiumService _premiumService;
        private readonly IPackageService _packageService;

        public PremiumController(IPremiumService premiumService, IPackageService packageService)
        {
            _premiumService = premiumService;
            _packageService = packageService;
        }

        /// <summary>
        /// Gửi message cho coach (Premium only)
        /// </summary>
        /// <param name="messageDto">Thông tin message</param>
        /// <returns>Message đã được gửi</returns>
        [HttpPost("chat/send")]
        public async Task<ActionResult<ChatMessageDto>> SendMessageToCoach([FromBody] CreateChatMessageDto messageDto)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng chat chỉ dành cho gói Premium");
                }

                var sentMessage = await _premiumService.SendMessageToCoachAsync(accountId, messageDto);
                
                return Ok(new
                {
                    message = "Gửi tin nhắn thành công!",
                    data = sentMessage
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử chat với coach
        /// </summary>
        /// <param name="coachId">ID của coach</param>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>Danh sách messages</returns>
        [HttpGet("chat/history/{coachId}")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistory(
            int coachId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng chat chỉ dành cho gói Premium");
                }

                var chatHistory = await _premiumService.GetChatHistoryWithCoachAsync(accountId, coachId, pageNumber, pageSize);
                
                return Ok(new
                {
                    data = chatHistory,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalMessages = chatHistory.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu messages đã đọc
        /// </summary>
        /// <param name="messageIds">Danh sách message IDs</param>
        /// <returns>Kết quả thực hiện</returns>
        [HttpPost("chat/mark-read")]
        public async Task<ActionResult> MarkMessagesAsRead([FromBody] List<int> messageIds)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng chat chỉ dành cho gói Premium");
                }

                await _premiumService.MarkMessagesAsReadAsync(accountId, messageIds);
                
                return Ok(new { message = "Đánh dấu đã đọc thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Booking meeting với coach
        /// </summary>
        /// <param name="bookingDto">Thông tin booking</param>
        /// <returns>ID của meeting được tạo</returns>
        [HttpPost("meeting/book")]
        public async Task<ActionResult<object>> BookMeeting([FromBody] BookMeetingDto bookingDto)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng booking meeting chỉ dành cho gói Premium");
                }

                var sessionId = await _premiumService.BookMeetingWithCoachAsync(accountId, bookingDto);
                
                return Ok(new
                {
                    message = "Booking meeting thành công!",
                    sessionId = sessionId,
                    meetingDate = bookingDto.PreferredDate
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách meetings của user
        /// </summary>
        /// <returns>Danh sách meetings</returns>
        [HttpGet("meeting/my-meetings")]
        public async Task<ActionResult<List<MeetingDto>>> GetMyMeetings()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng meeting chỉ dành cho gói Premium");
                }

                var meetings = await _premiumService.GetUserMeetingsAsync(accountId);
                
                return Ok(new
                {
                    data = meetings,
                    totalMeetings = meetings.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin stage hiện tại
        /// </summary>
        /// <returns>Thông tin stage progress</returns>
        [HttpGet("stage/current")]
        public async Task<ActionResult<QuitStageProgressDto>> GetCurrentStage()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng quản lý giai đoạn chỉ dành cho gói Premium");
                }

                var currentStage = await _premiumService.GetCurrentStageProgressAsync(accountId);
                
                if (currentStage == null)
                {
                    return NotFound("Chưa có thông tin giai đoạn cai thuốc");
                }

                return Ok(currentStage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật progress của stage hiện tại
        /// </summary>
        /// <param name="updateDto">Thông tin cập nhật</param>
        /// <returns>Thông tin stage sau khi cập nhật</returns>
        [HttpPut("stage/update-progress")]
        public async Task<ActionResult<QuitStageProgressDto>> UpdateStageProgress([FromBody] UpdateStageProgressDto updateDto)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng quản lý giai đoạn chỉ dành cho gói Premium");
                }

                var updatedStage = await _premiumService.UpdateStageProgressAsync(accountId, updateDto);
                
                return Ok(new
                {
                    message = "Cập nhật tiến độ thành công!",
                    data = updatedStage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Chuyển sang giai đoạn tiếp theo
        /// </summary>
        /// <param name="nextStage">Tên giai đoạn tiếp theo</param>
        /// <returns>Thông tin giai đoạn mới</returns>
        [HttpPost("stage/advance")]
        public async Task<ActionResult<QuitStageProgressDto>> AdvanceToNextStage([FromBody] string nextStage)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng quản lý giai đoạn chỉ dành cho gói Premium");
                }

                var newStage = await _premiumService.AdvanceToNextStageAsync(accountId, nextStage);
                
                return Ok(new
                {
                    message = "Chuyển giai đoạn thành công!",
                    data = newStage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử tất cả các giai đoạn
        /// </summary>
        /// <returns>Danh sách tất cả stages</returns>
        [HttpGet("stage/history")]
        public async Task<ActionResult<List<QuitStageProgressDto>>> GetStageHistory()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                
                // Check Premium access
                if (!await _packageService.HasPremiumAccessAsync(accountId))
                {
                    return Forbid("Tính năng quản lý giai đoạn chỉ dành cho gói Premium");
                }

                var stageHistory = await _premiumService.GetStageHistoryAsync(accountId);
                
                return Ok(new
                {
                    data = stageHistory,
                    totalStages = stageHistory.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy AccountId từ JWT token
        /// </summary>
        private int GetCurrentAccountId()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            return accountId;
        }
    }
} 