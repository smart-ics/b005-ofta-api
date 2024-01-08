using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.ParamContext.SystemAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public record GenDocCommand(string DocId, string ContentBase64) : IRequest<GenDocResponse>, IDocKey;

public class GenDocResponse
{
    public string RequestedDocUrl { get; set; }
}

public class GenDocHandler : IRequestHandler<GenDocCommand, GenDocResponse>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IParamSistemDal _paramSistemDal;
    private readonly SaveDocFileWorker _saveDocFileWorker;

    public GenDocHandler(IDocBuilder builder, 
        IDocWriter writer, 
        IParamSistemDal paramSistemDal, SaveDocFileWorker saveDocFileWorker)
    {
        _builder = builder;
        _writer = writer;
        _paramSistemDal = paramSistemDal;
        _saveDocFileWorker = saveDocFileWorker;
    }

    public Task<GenDocResponse> Handle(GenDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _builder
            .Load(request)
            .GenRequestedDocUrl()
            .Build();

        var saveDocFileRequest = new SaveDocFileRequest
        {
            FilePathName = aggregate.RequestedDocUrl,
            FileContentBase64 = request.ContentBase64
        };
        _saveDocFileWorker.Execute(saveDocFileRequest);
        
        //  WRITE
        _writer.Save(aggregate);
        var response = new GenDocResponse
        {
            RequestedDocUrl = aggregate.RequestedDocUrl
        };
        return Task.FromResult(response);
    }
} 