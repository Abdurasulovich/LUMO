using Lummo.Mobile.ApiClient.Models;
using Newtonsoft.Json;

namespace Lummo.Mobile.ApiClient.Exceptions;

public partial class ApiException
{
    public ApiErrorResponse? GetApiError()
    {
        if (string.IsNullOrWhiteSpace(Response))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<ApiErrorResponse>(Response);
        }
        catch
        {
            return null;
        }
    }

    public string GetUserMessage()
    {
        var apiError = GetApiError();

        if (apiError is not null)
        {
            if (apiError.Errors is not null && apiError.Errors.Any())
                return string.Join("\n", apiError.Errors);

            return apiError.Message;
        }

        return StatusCode switch
        {
            401 => "Authentication failed. Please check your credentials.",
            403 => "You don't have permission for this action.",
            404 => "Requested resource not found.",
            _ => "An unexpected error occurred. Please try again."
        };
    }

    public string? GetErrorCode()
    {
        return GetApiError()?.ErrorCode;
    }
}
