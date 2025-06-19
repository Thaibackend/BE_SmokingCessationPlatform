using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Constants;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Package;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý các tính năng Premium: Chat, Meeting, Stage Management
    /// </summary>
    public class PremiumService : IPremiumService
    {
        private readonly AppDbContext _context;

        public PremiumService(AppDbContext context)
        {
            _context = context;
        }

        #region Chat Features

        /// <summary>
        /// Gửi message cho coach (Premium only)
        /// </summary>
        public async Task<ChatMessageDto> SendMessageToCoachAsync(int senderId, CreateChatMessageDto messageDto)
        {
            var userSubscription = await _context.UserSubscriptions
                .Include(s => s.AssignedCoach)
                .Where(s => s.AccountId == senderId && s.PackageType == PackageTypes.PREMIUM && s.Status == SubscriptionStatus.ACTIVE)
                .FirstOrDefaultAsync();

            if (userSubscription?.AssignedCoachId == null)
            {
                throw new UnauthorizedAccessException("Bạn cần gói Premium và được assign coach để sử dụng chat");
            }

            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Content,
                MessageType = messageDto.MessageType ?? "TEXT",
                AttachmentUrl = messageDto.AttachmentUrl,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return await GetChatMessageAsync(chatMessage.MessageId);
        }

        /// <summary>
        /// Lấy lịch sử chat với coach
        /// </summary>
        public async Task<List<ChatMessageDto>> GetChatHistoryWithCoachAsync(int accountId, int coachId, int pageNumber = 1, int pageSize = 50)
        {
            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == accountId && m.ReceiverId == coachId) ||
                           (m.SenderId == coachId && m.ReceiverId == accountId))
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return messages.Select(MapToChatMessageDto).ToList();
        }

        /// <summary>
        /// Đánh dấu messages đã đọc
        /// </summary>
        public async Task MarkMessagesAsReadAsync(int receiverId, List<int> messageIds)
        {
            var messages = await _context.ChatMessages
                .Where(m => messageIds.Contains(m.MessageId) && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Meeting Features

        /// <summary>
        /// Book meeting với coach
        /// </summary>
        public async Task<int> BookMeetingWithCoachAsync(int accountId, BookMeetingDto bookingDto)
        {
            var userSubscription = await _context.UserSubscriptions
                .Where(s => s.AccountId == accountId && s.PackageType == PackageTypes.PREMIUM && s.Status == SubscriptionStatus.ACTIVE)
                .FirstOrDefaultAsync();

            if (userSubscription?.AssignedCoachId == null)
            {
                throw new UnauthorizedAccessException("Bạn cần gói Premium và được assign coach để book meeting");
            }

            // Check coach availability (simplified - in real app, check coach schedule)
            var existingMeeting = await _context.CoachSessions
                .Where(cs => cs.CoachId == userSubscription.AssignedCoachId && 
                           cs.SessionDate.Date == bookingDto.PreferredDate.Date &&
                           cs.Status != "CANCELLED")
                .FirstOrDefaultAsync();

            if (existingMeeting != null)
            {
                throw new InvalidOperationException("Coach đã có lịch trong thời gian này");
            }

            var coachSession = new CoachSession
            {
                CoachId = userSubscription.AssignedCoachId.Value,
                AccountId = accountId,
                SessionDate = bookingDto.PreferredDate,
                StartTime = bookingDto.PreferredDate,
                EndTime = bookingDto.PreferredDate.AddHours(1),
                SessionType = bookingDto.MeetingType,
                Notes = bookingDto.Notes,
                Status = "SCHEDULED"
            };

            _context.CoachSessions.Add(coachSession);
            await _context.SaveChangesAsync();

            return coachSession.SessionId;
        }

        /// <summary>
        /// Lấy danh sách meetings của user
        /// </summary>
        public async Task<List<MeetingDto>> GetUserMeetingsAsync(int accountId)
        {
            var meetings = await _context.CoachSessions
                .Include(cs => cs.Coach)
                .ThenInclude(c => c.Account)
                .Where(cs => cs.AccountId == accountId)
                .OrderByDescending(cs => cs.SessionDate)
                .ToListAsync();

            return meetings.Select(m => new MeetingDto
            {
                SessionId = m.SessionId,
                CoachId = m.CoachId,
                CoachName = m.Coach.Account.Username,
                SessionDate = m.SessionDate,
                SessionType = m.SessionType ?? "CONSULTATION",
                Status = m.Status,
                Notes = m.Notes,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        #endregion

        #region Stage Management

        /// <summary>
        /// Lấy thông tin stage hiện tại của Premium user
        /// </summary>
        public async Task<QuitStageProgressDto?> GetCurrentStageProgressAsync(int accountId)
        {
            var stageProgress = await _context.QuitStageProgresses
                .Where(q => q.AccountId == accountId)
                .OrderByDescending(q => q.StageStartDate)
                .FirstOrDefaultAsync();

            if (stageProgress == null)
                return null;

            return MapToQuitStageProgressDto(stageProgress);
        }

        /// <summary>
        /// Cập nhật progress của stage hiện tại
        /// </summary>
        public async Task<QuitStageProgressDto> UpdateStageProgressAsync(int accountId, UpdateStageProgressDto updateDto)
        {
            var stageProgress = await _context.QuitStageProgresses
                .Where(q => q.AccountId == accountId)
                .OrderByDescending(q => q.StageStartDate)
                .FirstOrDefaultAsync();

            if (stageProgress == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin stage progress");
            }

            // Cập nhật thông tin
            stageProgress.ProgressPercentage = updateDto.ProgressPercentage;
            stageProgress.UserNotes = updateDto.UserNotes;
            stageProgress.CigarettesSmoked = updateDto.CigarettesSmoked;
            stageProgress.CravingLevel = updateDto.CravingLevel;
            stageProgress.StressLevel = updateDto.StressLevel;
            stageProgress.SupportLevel = updateDto.SupportLevel;
            stageProgress.Challenges = updateDto.Challenges;
            stageProgress.Achievements = updateDto.Achievements;
            stageProgress.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToQuitStageProgressDto(stageProgress);
        }

        /// <summary>
        /// Chuyển sang stage tiếp theo
        /// </summary>
        public async Task<QuitStageProgressDto> AdvanceToNextStageAsync(int accountId, string nextStage)
        {
            var currentStage = await _context.QuitStageProgresses
                .Where(q => q.AccountId == accountId)
                .OrderByDescending(q => q.StageStartDate)
                .FirstOrDefaultAsync();

            if (currentStage == null)
            {
                throw new InvalidOperationException("Không tìm thấy stage hiện tại");
            }

            // Đánh dấu stage hiện tại hoàn thành
            currentStage.StageEndDate = DateTime.UtcNow;
            currentStage.ProgressPercentage = 100;

            // Tạo stage mới
            var newStage = new QuitStageProgress
            {
                AccountId = accountId,
                CurrentStage = nextStage,
                StageStartDate = DateTime.UtcNow,
                ProgressPercentage = 0,
                StageGoals = GetStageGoals(nextStage),
                UserNotes = $"Chuyển từ {currentStage.CurrentStage} sang {nextStage}"
            };

            _context.QuitStageProgresses.Add(newStage);
            await _context.SaveChangesAsync();

            return MapToQuitStageProgressDto(newStage);
        }

        /// <summary>
        /// Lấy history của tất cả stages
        /// </summary>
        public async Task<List<QuitStageProgressDto>> GetStageHistoryAsync(int accountId)
        {
            var stages = await _context.QuitStageProgresses
                .Where(q => q.AccountId == accountId)
                .OrderBy(q => q.StageStartDate)
                .ToListAsync();

            return stages.Select(MapToQuitStageProgressDto).ToList();
        }

        #endregion

        #region Private Helper Methods

        private async Task<ChatMessageDto> GetChatMessageAsync(int messageId)
        {
            var message = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null)
                throw new InvalidOperationException("Message not found");

            return MapToChatMessageDto(message);
        }

        private ChatMessageDto MapToChatMessageDto(ChatMessage message)
        {
            return new ChatMessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                SenderName = message.Sender.Username,
                ReceiverId = message.ReceiverId,
                ReceiverName = message.Receiver.Username,
                Content = message.Content,
                MessageType = message.MessageType,
                AttachmentUrl = message.AttachmentUrl,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                SentAt = message.SentAt
            };
        }

        private QuitStageProgressDto MapToQuitStageProgressDto(QuitStageProgress stage)
        {
            return new QuitStageProgressDto
            {
                StageProgressId = stage.StageProgressId,
                AccountId = stage.AccountId,
                CurrentStage = stage.CurrentStage,
                StageDisplayName = GetStageDisplayName(stage.CurrentStage),
                StageDescription = GetStageDescription(stage.CurrentStage),
                StageStartDate = stage.StageStartDate,
                StageEndDate = stage.StageEndDate,
                ProgressPercentage = stage.ProgressPercentage,
                StageGoals = stage.StageGoals,
                CoachNotes = stage.CoachNotes,
                UserNotes = stage.UserNotes,
                CigarettesSmoked = stage.CigarettesSmoked,
                CravingLevel = stage.CravingLevel,
                StressLevel = stage.StressLevel,
                SupportLevel = stage.SupportLevel,
                Challenges = stage.Challenges,
                Achievements = stage.Achievements,
                CreatedAt = stage.CreatedAt,
                UpdatedAt = stage.UpdatedAt,
                NextStage = GetNextStage(stage.CurrentStage),
                NextStageActions = GetNextStageActions(stage.CurrentStage)
            };
        }

        private string GetStageDisplayName(string stage)
        {
            return stage switch
            {
                QuitStages.PREPARATION => "Chuẩn bị",
                QuitStages.INITIAL_QUIT => "Bắt đầu cai (0-3 ngày)",
                QuitStages.EARLY_RECOVERY => "Phục hồi sớm (4-30 ngày)",
                QuitStages.ONGOING_RECOVERY => "Phục hồi tiếp tục (1-12 tháng)",
                QuitStages.MAINTENANCE => "Duy trì (12+ tháng)",
                _ => stage
            };
        }

        private string GetStageDescription(string stage)
        {
            return stage switch
            {
                QuitStages.PREPARATION => "Giai đoạn chuẩn bị tinh thần và môi trường để bắt đầu cai thuốc",
                QuitStages.INITIAL_QUIT => "Giai đoạn quan trọng nhất - 72 giờ đầu tiên không hút thuốc",
                QuitStages.EARLY_RECOVERY => "Giai đoạn khó khăn với cơn thèm và triệu chứng cai thuốc",
                QuitStages.ONGOING_RECOVERY => "Giai đoạn ổn định và xây dựng thói quen mới",
                QuitStages.MAINTENANCE => "Giai đoạn duy trì lối sống không thuốc lá",
                _ => "Mô tả stage"
            };
        }

        private string GetStageGoals(string stage)
        {
            return stage switch
            {
                QuitStages.PREPARATION => "Chuẩn bị tinh thần, loại bỏ thuốc lá, thông báo với gia đình",
                QuitStages.INITIAL_QUIT => "Vượt qua 72 giờ đầu tiên không hút thuốc",
                QuitStages.EARLY_RECOVERY => "Quản lý cơn thèm và triệu chứng cai thuốc",
                QuitStages.ONGOING_RECOVERY => "Xây dựng thói quen lành mạnh, tránh tái phát",
                QuitStages.MAINTENANCE => "Duy trì lối sống không thuốc lá lâu dài",
                _ => "Mục tiêu chung"
            };
        }

        private string GetNextStage(string currentStage)
        {
            return currentStage switch
            {
                QuitStages.PREPARATION => QuitStages.INITIAL_QUIT,
                QuitStages.INITIAL_QUIT => QuitStages.EARLY_RECOVERY,
                QuitStages.EARLY_RECOVERY => QuitStages.ONGOING_RECOVERY,
                QuitStages.ONGOING_RECOVERY => QuitStages.MAINTENANCE,
                QuitStages.MAINTENANCE => QuitStages.MAINTENANCE,
                _ => ""
            };
        }

        private List<string> GetNextStageActions(string currentStage)
        {
            return currentStage switch
            {
                QuitStages.PREPARATION => new[] { "Đặt ngày cai cụ thể", "Loại bỏ thuốc lá", "Chuẩn bị tinh thần" }.ToList(),
                QuitStages.INITIAL_QUIT => new[] { "Tập thể dục", "Uống nhiều nước", "Tránh trigger" }.ToList(),
                QuitStages.EARLY_RECOVERY => new[] { "Xây dựng routine mới", "Tìm hoạt động thay thế", "Tham gia nhóm hỗ trợ" }.ToList(),
                QuitStages.ONGOING_RECOVERY => new[] { "Duy trì thói quen lành mạnh", "Theo dõi tiến độ", "Chia sẻ kinh nghiệm" }.ToList(),
                QuitStages.MAINTENANCE => new[] { "Tiếp tục theo dõi", "Hỗ trợ người khác", "Duy trì động lực" }.ToList(),
                _ => new List<string>()
            };
        }

        #endregion
    }
} 