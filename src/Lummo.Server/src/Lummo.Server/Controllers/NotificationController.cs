using AutoMapper;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Server.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(IEmailTemplateService emailTemplateService, 
    IMapper mapper) : ControllerBase
{
    [HttpGet("templates/email")]
    public async ValueTask<IActionResult> GetEmailTemplates([FromQuery] FilterPagination filterPagination,
        CancellationToken cancellationToken)
    {
        var result = emailTemplateService.Get();

        // IQueryable'ni List'ga o'tkazish
        var templates = await result.ToListAsync(cancellationToken);

        if (!templates.Any())
            return NotFound();

        // List<EmailTemplate> -> List<EmailTemplateDto>
        var mappedResult = mapper.Map<List<EmailTemplateDto>>(templates);

        return Ok(mappedResult);
    }
}
