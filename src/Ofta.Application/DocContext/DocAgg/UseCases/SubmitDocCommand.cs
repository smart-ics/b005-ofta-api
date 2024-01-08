using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.ParamContext.SystemAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record SubmitDocCommand(string DocId, string ContentBase64) : IRequest<SubmitDocResponse>, IDocKey;

public class SubmitDocResponse
{
    public string RequestedDocUrl { get; set; }
}

public class SubmitDocHandler : IRequestHandler<SubmitDocCommand, SubmitDocResponse>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly SaveDocFileWorker _saveDocFileWorker;

    public SubmitDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        SaveDocFileWorker saveDocFileWorker)
    {
        _builder = builder;
        _writer = writer;
        _saveDocFileWorker = saveDocFileWorker;
    }

    public Task<SubmitDocResponse> Handle(SubmitDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());
        var aggregate = _builder
            .Load(request)
            .Build();
        if (aggregate.DocState != DocStateEnum.Created)
            throw new Exception($"Submit failed: DocState has been {aggregate.DocState.ToString()}");
        
        
        //  BUILD
        aggregate = _builder
            .Attach(aggregate)
            .GenRequestedDocUrl()
            .DocState(DocStateEnum.Submited)
            .Build();
        
        var saveDocFileRequest = new SaveDocFileRequest
        {
            FilePathName = aggregate.RequestedDocUrl,
            FileContentBase64 = request.ContentBase64
        };
        _saveDocFileWorker.Execute(saveDocFileRequest);
        
        //  WRITE
        _writer.Save(aggregate);
        var response = new SubmitDocResponse
        {
            RequestedDocUrl = aggregate.RequestedDocUrl
        };
        return Task.FromResult(response);
    }
} 