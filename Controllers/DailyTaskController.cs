using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.DailyTask;
using SmokingQuitSupportAPI.Services;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DailyTaskController : ControllerBase
    {
        private readonly DailyTaskService _dailyTaskService;

        public DailyTaskController(DailyTaskService dailyTaskService)
        {
            _dailyTaskService = dailyTaskService;
        }

        [HttpPost]
        public async Task<ActionResult<DailyTaskDto>> CreateTask([FromBody] CreateDailyTaskRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var task = await _dailyTaskService.CreateTaskAsync(userId, request);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DailyTaskDto>>> GetUserTasks()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var tasks = await _dailyTaskService.GetUserTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<DailyTaskDto>>> GetTasksByDate(DateTime date)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var tasks = await _dailyTaskService.GetTasksByDateAsync(userId, date);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<DailyTaskDto>>> GetPendingTasks()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var tasks = await _dailyTaskService.GetPendingTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult<DailyTaskDto>> CompleteTask(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var task = await _dailyTaskService.CompleteTaskAsync(id, userId);
                
                if (task == null)
                    return NotFound(new { message = "Task not found or access denied" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<DailyTaskDto>> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var task = await _dailyTaskService.UpdateTaskStatusAsync(id, userId, request.Status);
                
                if (task == null)
                    return NotFound(new { message = "Task not found or access denied" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class UpdateTaskStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
} 