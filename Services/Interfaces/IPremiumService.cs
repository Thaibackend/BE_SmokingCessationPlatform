using SmokingQuitSupportAPI.Models.DTOs.Package;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các operations cho Premium Service
    /// </summary>
    public interface IPremiumService
    {
        // Chat Features
        Task<ChatMessageDto> SendMessageToCoachAsync(int senderId, CreateChatMessageDto messageDto);
        Task<List<ChatMessageDto>> GetChatHistoryWithCoachAsync(int accountId, int coachId, int pageNumber = 1, int pageSize = 50);
        Task MarkMessagesAsReadAsync(int receiverId, List<int> messageIds);

        // Meeting Features
        Task<int> BookMeetingWithCoachAsync(int accountId, BookMeetingDto bookingDto);
        Task<List<MeetingDto>> GetUserMeetingsAsync(int accountId);

        // Stage Management
        Task<QuitStageProgressDto?> GetCurrentStageProgressAsync(int accountId);
        Task<QuitStageProgressDto> UpdateStageProgressAsync(int accountId, UpdateStageProgressDto updateDto);
        Task<QuitStageProgressDto> AdvanceToNextStageAsync(int accountId, string nextStage);
        Task<List<QuitStageProgressDto>> GetStageHistoryAsync(int accountId);
    }
} 