using Lummo.Domain.Common.Exceptions;

namespace Lummo.Domain.Extensions;

public static class ExceptionExtensions
{
    public static async ValueTask<FuncResult<T>> GetValueAsync<T>(this Func<Task<T>> func) where T : struct
    {
        FuncResult<T> result;
        try
        {
            result = new FuncResult<T>(await func());
        }
        catch (Exception ex)
        {
            result = new FuncResult<T>(ex);
        }

        return result;
    }

    public static async ValueTask<FuncResult<T>> GetValueAsync<T>(this Func<ValueTask<T>> func) where T : struct
    {
        FuncResult<T> result;
        try
        {
            result = new FuncResult<T>(await func());
        }
        catch (Exception ex)
        {
            result = new FuncResult<T>(ex);
        }

        return result;
    }

    public static async ValueTask<FuncResult<bool>> GetValueAsync(this Func<ValueTask> func)
    {
        FuncResult<bool> result;
        try
        {
            await func();
            result = new FuncResult<bool>(true);
        }catch (Exception ex)
        {
            result = new FuncResult<bool>(ex);
        }
        return result;
    }

    public static FuncResult<T> GetValue<T>(this Func<T> func)
    {
        FuncResult<T> funcResult;
        try
        {
            funcResult = new FuncResult<T>(func());
        }catch(Exception ex)
        {
            funcResult = new FuncResult<T>(ex);
        }

        return funcResult;
    }
}
