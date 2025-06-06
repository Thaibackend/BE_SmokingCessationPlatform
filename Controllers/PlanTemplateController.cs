using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.PlanTemplate;
using SmokingQuitSupportAPI.Services;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanTemplateController : ControllerBase
    {
        private readonly PlanTemplateService _planTemplateService;

        public PlanTemplateController(PlanTemplateService planTemplateService)
        {
            _planTemplateService = planTemplateService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<ActionResult<PlanTemplateDto>> CreateTemplate([FromBody] CreatePlanTemplateRequestDto request)
        {
            try
            {
                var template = await _planTemplateService.CreateTemplateAsync(request);
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanTemplateDto>>> GetActiveTemplates()
        {
            try
            {
                var templates = await _planTemplateService.GetActiveTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<PlanTemplateDto>>> GetTemplatesByCategory(string category)
        {
            try
            {
                var templates = await _planTemplateService.GetTemplatesByCategoryAsync(category);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlanTemplateDto>> GetTemplateById(int id)
        {
            try
            {
                var template = await _planTemplateService.GetTemplateByIdAsync(id);
                
                if (template == null)
                    return NotFound(new { message = "Template not found" });

                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<ActionResult<PlanTemplateDto>> UpdateTemplate(int id, [FromBody] CreatePlanTemplateRequestDto request)
        {
            try
            {
                var template = await _planTemplateService.UpdateTemplateAsync(id, request);
                
                if (template == null)
                    return NotFound(new { message = "Template not found" });

                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 