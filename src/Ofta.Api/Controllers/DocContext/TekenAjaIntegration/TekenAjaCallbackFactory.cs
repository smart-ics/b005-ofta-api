// ReSharper disable InconsistentNaming

using MediatR;
using Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CommandAdapter;

namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration;


public class TekenAjaCallbackRequest
{
    public bool status { get; set; }
    public string code { get; set; } = string.Empty;
    public DateTime timestamp { get; set; }
    public object? message { get; set; }
    public object data { get; set; }
}

public interface ITekenAjaCallbackFactory
{
    IRequest AdaptCommand(TekenAjaCallbackRequest callback);
}

public class TekenAjaCallbackFactory
{
    public static ITekenAjaCallbackFactory Factory(TekenAjaCallbackRequest callbackRequest)
        => callbackRequest.code switch
        {
            "DOCUMENT_SIGNED" => new DocSignedCommandAdapter(),
            "DOCUMENT_SIGN_COMPLETED" => new DocCompletedCommandAdapter(),
            _ => throw new ArgumentOutOfRangeException()
        };
}

