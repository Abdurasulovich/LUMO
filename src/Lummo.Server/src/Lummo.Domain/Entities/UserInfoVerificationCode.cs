using Lummo.Domain.Enums;
namespace Lummo.Domain.Entities;

public class UserInfoVerificationCode : VerificationCode
{
    public Guid UserId { get; set; }
    public UserInfoVerificationCode()
    {
        Type = VerificationType.UserInfoVerificationCode;
    }
}
