using System.Text.Json;
using MediatR;
using Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CallbackData;
using Ofta.Application.DocContext.DocAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CommandAdapter;

public class DocSignedCommandAdapter : ITekenAjaCallbackFactory
{
    public IRequest AdaptCommand(TekenAjaCallbackRequest callback)
    {
        var data = JsonSerializer.Deserialize<DocSignedCallbackData>(callback.data.ToString() ?? string.Empty);
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        var docReffId = data.document_id;
        var cmd = new SignDocCommand(docReffId, data.sign[0].email);
        return cmd;
    }
}