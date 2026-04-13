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
    IAccountService accountService) : ControllerBase
{
    [HttpPost("sign-up")]
    public async ValueTask<ActionResult<bool>> SignUp([FromBody] SignUpDetails signUpDetails, CancellationToken cancellationToken)
    {
        var result = await authService.SignUpAsync(signUpDetails, cancellationToken);

        return result ? Ok(result) : BadRequest();
    }

    [HttpPost("sign-up-with-google")]
    public async ValueTask<ActionResult<IdentityTokenDto>> SignUpWithGoogle([FromBody] GoogleSignInRequest idToken, CancellationToken cancellationToken)
    {
        var result = await authService.SignUpWithGoogleAsync(idToken, cancellationToken);
        return Ok(mapper.Map<IdentityTokenDto>(result));
    }

    [HttpPost("sign-in")]
    public async ValueTask<ActionResult<IdentityTokenDto>> SignIn([FromBody] SignInDetails signInDetails, CancellationToken cancellationToken)
    {
        var result = await authService.SignInAsync(signInDetails, cancellationToken);

        return Ok(mapper.Map<IdentityTokenDto>(result));
    }
    [HttpPost("sign-in-with-google")]
    public async ValueTask<ActionResult<IdentityTokenDto>> SignInWithGoogle([FromBody] GoogleSignInRequest googleSignInRequest, CancellationToken cancellationToken)
    {
        var result = await authService.SignInWithGoogleAsync(googleSignInRequest, cancellationToken);
        return Ok(mapper.Map<IdentityTokenDto>(result));
    }
    [HttpPut("refresh-token")]
    public async ValueTask<ActionResult<AccessTokenDto>> RefreshToken([FromBody] string refreshTokenValue, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(refreshTokenValue, cancellationToken);
        return Ok(mapper.Map<AccessTokenDto>(result));
    }

    [HttpPost("verify-email")]
    public async ValueTask<ActionResult<bool>> VerifyEmail([FromBody] EmailVerificationDetails verificationCode, CancellationToken cancellationToken)
    {
        var result = await accountService.VerifyUserAsync(verificationCode, cancellationToken);
        return result ? Ok(new { message = "Email verified successfully" }) : BadRequest(new { message = "Invalid or expired verification code" });
    }

    [HttpPost("forgot-password-verify-email")]
    public async ValueTask<ActionResult<bool>> ForgotPasswordVerifyEmail([FromBody] ForgotPasswordEmailVerificationDetails verificationEmail, CancellationToken cancellationToken)
    {
        var result = await accountService.ForgotPasswordVerifyEmailAsync(verificationEmail, cancellationToken);
        return result ? Ok(new { message = "Verification code sent successfully" }) : BadRequest(new { message = "Failed to send verification code. User may not exist." });
    }
    [HttpPost("forgot-password-verify-code")]
    public async ValueTask<ActionResult<bool>> ForgotPasswordVerifyCode([FromBody] EmailVerificationDetails verificationCode, CancellationToken cancellationToken)
    {
        var result = await accountService.ForgotPasswordConfirmEmailAsync(verificationCode, cancellationToken);
        return result ? Ok(new { message = "Verification code is valid" }) : BadRequest(new { message = "Invalid or expired verification code" });
    }
    [HttpPost("reset-password")]
    public async ValueTask<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordDetails resetPasswordDetails, CancellationToken cancellationToken)
    {
        var result = await accountService.ResetPasswordAsync(resetPasswordDetails, cancellationToken);
        return result ? Ok(new { message = "Password reset successfully" }) : BadRequest(new { message = "Invalid or expired verification code" });
    }

    [HttpPost("resend-verification-code")]
    public async ValueTask<ActionResult<bool>> ResendVerificationCode([FromBody] ResendVerificationCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await accountService.ResendVerificationCodeAsync(request, cancellationToken);
        return result ? Ok(new { message = "Verification code sent successfully" }) : BadRequest(new { message = "Failed to send verification code. User may not exist or already verified." });
    }

    [Authorize(Roles = "Admin, System")]
    [HttpPost("users/{userId:guid}/roles/{roleType}")]
    public async ValueTask<ActionResult<bool>> GrandRole([FromRoute] Guid userId, [FromRoute] string roleType, CancellationToken cancellationToken)
    {
        var result = await authService.GrandRoleAsync(userId, roleType, cancellationToken);
        return result ? Ok(result) : NoContent();
    }

    [Authorize(Roles = "Admin, System")]
    [HttpDelete("users/{userId:guid}/roles/{roleType}")]
    public async ValueTask<ActionResult<bool>> RevokeRole([FromRoute] Guid userId, [FromRoute] string roleType, CancellationToken cancellationToken)
    {
        var result = await authService.RevokeRoleAsync(userId, roleType, cancellationToken);
        return result ? Ok(result) : NoContent();
    }
}
