using AutoMapper;
using Lummo.Application.Common.Identity.Models;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Brokers;
using Lummo.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lummo.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IMapper mapper,
    IAuthService authService,
    IRequestUserContextProvider requestuserContextProvider) : ControllerBase
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDetails signUpDetails, CancellationToken cancellationToken)
    {
        var result = await authService.SignUpAsync(signUpDetails, cancellationToken);
        return result ? Ok(result) : BadRequest();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInDetails signInDetails, CancellationToken cancellationToken)
    {
        var result = await authService.SignInAsync(signInDetails, cancellationToken);

        return Ok(mapper.Map<IdentityTokenDto>(result));
    }

    [HttpPut("refresh-token")]
    public async ValueTask<IActionResult> RefreshToken([FromBody] string refreshTokenValue, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(refreshTokenValue, cancellationToken);
        return Ok(mapper.Map<AccessTokenDto>(result));
    }
    
    [Authorize(Roles = "Admin, System")]
    [HttpPost("users/{userId:guid}/roles/{roleType}")]
    public async Task<IActionResult> GrandRole([FromRoute] Guid userId, [FromRoute] string roleType, CancellationToken cancellationToken)
    {
        var result = await authService.GrandRoleAsync(userId, roleType, cancellationToken);
        return result ? Ok(result) : NoContent();
    }

    [Authorize(Roles = "Admin, System")]
    [HttpDelete("users/{userId:guid}/roles/{roleType}")]
    public async Task<IActionResult> RevokeRole([FromRoute] Guid userId, [FromRoute] string roleType, CancellationToken cancellationToken)
    {
        var result = await authService.RevokeRoleAsync(userId, roleType, cancellationToken);
        return result ? Ok(result) : NoContent();
    }
}
