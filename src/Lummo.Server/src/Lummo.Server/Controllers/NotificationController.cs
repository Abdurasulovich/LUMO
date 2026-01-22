using AutoMapper;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Server.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        return result.Any() ? Ok(mapper.Map<EmailTemplateDto>(result)) : NotFound();
    }


}
