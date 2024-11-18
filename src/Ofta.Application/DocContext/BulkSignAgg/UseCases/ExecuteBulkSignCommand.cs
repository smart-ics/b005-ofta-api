using Dawn;
using MediatR;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Application.DocContext.BulkSignAgg.Workers;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.UseCases;

public record ExecuteBulkSignCommand (string BulkSignId, string Email): IRequest, IBulkSignKey;

public class ExecuteBulkSignHandler: IRequestHandler<ExecuteBulkSignCommand>
{
    private readonly IBulkSignBuilder _builder;
    private readonly ITilakaUserBuilder _tilakaUserBuilder;
    private readonly IBulkSignWriter _writer;
    private readonly IExecuteBulkSignService _service;

    public ExecuteBulkSignHandler(IBulkSignBuilder builder, ITilakaUserBuilder tilakaUserBuilder, IBulkSignWriter writer, IExecuteBulkSignService service)
    {
        _builder = builder;
        _tilakaUserBuilder = tilakaUserBuilder;
        _writer = writer;
        _service = service;
    }

    public Task<Unit> Handle(ExecuteBulkSignCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.BulkSignId, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var aggregate = _builder
            .Load(request)
            .Build();

        var tilakaUser = _tilakaUserBuilder
            .Load(request.Email)
            .Build();

        var userProvider = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;
        var executeBulkSignRequest = new ExecuteBulkSignRequest(aggregate, userProvider);
        
        var executeSign = _service.Execute(executeBulkSignRequest);
        if (!executeSign.Success)
            throw new ArgumentException(executeSign.Message);
        
        aggregate = _builder
            .Attach(aggregate)
            .UpdateState(BulkSignStateEnum.SuccessSign)
            .Build();
        
        // WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}