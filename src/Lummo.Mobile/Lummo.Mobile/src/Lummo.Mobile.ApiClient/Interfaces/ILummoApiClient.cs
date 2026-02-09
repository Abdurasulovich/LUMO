using Lummo.Mobile.ApiClient.Models;

namespace Lummo.Mobile.ApiClient.Interfaces;

public interface ILummoApiClient
{
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<UserDto>> AccountsAllAsync(int? pageSize, int? pageToken);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<UserDto>> AccountsAllAsync(int? pageSize, int? pageToken, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserDto> AccountsPOSTAsync(UserDto body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserDto> AccountsPOSTAsync(UserDto body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserDto> AccountsPUTAsync(UserDto body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserDto> AccountsPUTAsync(UserDto body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserDto> AccountsGETAsync(System.Guid userId);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserDto> AccountsGETAsync(System.Guid userId, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task AccountsDELETEAsync(System.Guid userId);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task AccountsDELETEAsync(System.Guid userId, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> SignUpAsync(SignUpDetails body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> SignUpAsync(SignUpDetails body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<IdentityTokenDto> SignInAsync(SignInDetails body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<IdentityTokenDto> SignInAsync(SignInDetails body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<AccessTokenDto> RefreshTokenAsync(string body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<AccessTokenDto> RefreshTokenAsync(string body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> VerifyEmailAsync(EmailVerificationDetails body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> VerifyEmailAsync(EmailVerificationDetails body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> ResendVerificationCodeAsync(string body);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> ResendVerificationCodeAsync(string body, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> RolesPOSTAsync(System.Guid userId, string roleType);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> RolesPOSTAsync(System.Guid userId, string roleType, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> RolesDELETEAsync(System.Guid userId, string roleType);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<bool> RolesDELETEAsync(System.Guid userId, string roleType, System.Threading.CancellationToken cancellationToken);

    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task EmailAsync(int? pageSize, int? pageToken);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Success</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task EmailAsync(int? pageSize, int? pageToken, System.Threading.CancellationToken cancellationToken);

}
