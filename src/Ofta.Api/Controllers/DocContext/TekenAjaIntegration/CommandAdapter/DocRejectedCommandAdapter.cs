using System.Text.Json;
using MediatR;
using Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CallbackData;
using Ofta.Application.DocContext.DocAgg.UseCases;

namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CommandAdapter;

public class DocRejectedCommandAdapter : ITekenAjaCallbackFactory
{
    public IRequest AdaptCommand(TekenAjaCallbackRequest callback)
    {
        var data = JsonSerializer.Deserialize<DocRejectedCallbackData>(callback.data.ToString() ?? string.Empty);
        if (data is null)
            throw new ArgumentNullException(nameof(callback),nameof(data));

        var docReffId = data.document_id;
        var downloadUrl = data.signer_rejected.email;
        var cmd = new RejectSignDocCommand(docReffId, downloadUrl);
        return cmd;
    }
}