using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
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
    private readonly ISaveFileService _saveFileService;
    private readonly IMediator _mediator;

    public SubmitDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        ISaveFileService saveFileService, 
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _saveFileService = saveFileService;
        _mediator = mediator;
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
            .DocState(DocStateEnum.Submited, string.Empty)
            .Build();
        
        
        //  WRITE
        _writer.Save(aggregate);
        var response = new SubmitDocResponse
        {
            RequestedDocUrl = aggregate.RequestedDocUrl
        };
        _mediator.Publish(new SubmittedDocEvent
        {
            Aggregate = aggregate,
            Command = request
        }, cancellationToken);
        return Task.FromResult(response);
    }
} 

public class SubmittedDocEvent : INotification
{
    public DocModel Aggregate { get; set; }
    public SubmitDocCommand Command { get; set; }
}