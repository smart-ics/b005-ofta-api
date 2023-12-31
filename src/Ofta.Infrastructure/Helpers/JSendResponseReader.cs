using Dawn;
using Newtonsoft.Json;
using RestSharp;

namespace Ofta.Infrastructure.Helpers;

public static class JSendResponse
{
    public static T Read<T>(RestResponse<JSend<T>> response)
    {
        Guard.Argument(() => response).NotNull();
        if (response.Data is null)
            ReadAndThrowError(response);
        return response.Data!.Data;
    }
    private static void ReadAndThrowError(RestResponseBase response)
    {
        Guard.Argument(() => response).NotNull();
        
        if (response.Content is null)
            throw new ArgumentException($"Error Remote: ({(int)response.StatusCode}) {response.ErrorException!.Message}");
            
        var resultFailed = JsonConvert.DeserializeObject<JSend<string>>(response.Content!);
        if (resultFailed != null)
            throw new ArgumentException(resultFailed.Data);
        else
            throw new ArgumentException($"Error Remote: ({(int)response.StatusCode}) {response.ErrorException!.Message}");
    }
}