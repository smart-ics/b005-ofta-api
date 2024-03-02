using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record SubmitDocCommand(string DocId, string ContentBase64) : IRequest<SubmitDocResponse>, IDocKey;

public record SubmitDocResponse(string RequestedDocUrl);

public class SubmitDocHandler : IRequestHandler<SubmitDocCommand, SubmitDocResponse>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IMediator _mediator;
    private readonly IWriteFileService _writeFileService;

    public SubmitDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        IMediator mediator, IWriteFileService writeFileService)
    {
        _builder = builder;
        _writer = writer;
        _mediator = mediator;
        _writeFileService = writeFileService;
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
            throw new ArgumentException($"Submit failed: DocState has been {aggregate.DocState.ToString()}");
        
        //  BUILD
        aggregate = _builder
            .Attach(aggregate)
            .GenRequestedDocUrl()
            .AddJurnal(DocStateEnum.Submited, string.Empty)
            .Build();
        var writeFileRequest = new WriteFileRequest(aggregate.RequestedDocUrl, request.ContentBase64);
        
        //  WRITE
        _ = _writeFileService.Execute(writeFileRequest);
        _writer.Save(aggregate);
        var submitEvent = new SubmittedDocEvent(aggregate, request);
        _mediator.Publish(submitEvent, cancellationToken);
        
        var response = new SubmitDocResponse(aggregate.RequestedDocUrl);
        return Task.FromResult(response);
    }
}

public record SubmittedDocEvent(
    DocModel Aggregate,
    SubmitDocCommand Command) : INotification;
