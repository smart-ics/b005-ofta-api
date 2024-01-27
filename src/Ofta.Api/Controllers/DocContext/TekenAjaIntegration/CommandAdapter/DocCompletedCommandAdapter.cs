using MediatR;
using Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CallbackData;
using Ofta.Application.DocContext.DocAgg.UseCases;
using System.Text.Json;

namespace Ofta.Api.Controllers.DocContext.TekenAjaIntegration.CommandAdapter;

public class DocCompletedCommandAdapter : ITekenAjaCallbackFactory
{
    public IRequest AdaptCommand(TekenAjaCallbackRequest callback)
    {
        var data = JsonSerializer.Deserialize<DocSignCompletedCallbackData>(callback.data.ToString() ?? string.Empty);
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        var docReffId = data.document_id;
        var downloadUrl = data.download_url;
        var cmd = new PublishDocCommand(docReffId, downloadUrl);
        return cmd;
    }
}