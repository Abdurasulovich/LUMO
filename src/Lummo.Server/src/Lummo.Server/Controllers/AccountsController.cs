using AutoMapper;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Persistence.Extensions;
using Lummo.Server.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async ValueTask<ActionResult<List<UserDto>>> Get([FromQuery] FilterPagination filterPagination, CancellationToken cancellationToken)
    {
        var users = await userService
            .Get(asNoTracking: true)
            .ApplySpecification(filterPagination)
            .ToListAsync(cancellationToken);

        if (!users.Any())
            return NotFound();

        var userDtos = mapper.Map<List<UserDto>>(users);
        return Ok(userDtos);
    }

    [HttpGet("{userId:guid}")]
    public async ValueTask<ActionResult<UserDto>> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, true, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpPost]
    public async ValueTask<ActionResult<UserDto>> Create([FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        var user = mapper.Map<User>(userDto);
        var createdUser = await userService.CreateAsync(user, cancellationToken: cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, mapper.Map<UserDto>(createdUser));
    }

    [HttpPut]
    public async ValueTask<ActionResult<UserDto>> Update([FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        var user = mapper.Map<User>(userDto);
        var updatedUser = await userService.UpdateAsync(user, cancellationToken: cancellationToken);

        return Ok(mapper.Map<UserDto>(updatedUser));
    }

    [HttpDelete("{userId:guid}")]
    public async ValueTask<IActionResult> DeleteByIdAsync([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var isDeleted = await userService.DeleteByIdAsync(userId, cancellationToken: cancellationToken);

        return isDeleted ? NoContent() : NotFound();
    }
}
