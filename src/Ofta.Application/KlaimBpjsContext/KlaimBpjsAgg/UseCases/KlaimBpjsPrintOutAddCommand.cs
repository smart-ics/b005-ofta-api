using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsPrintOutAddCommand(string KlaimBpjsId, string DocTypeId, string PrintOutReffId)
    : IRequest, IKlaimBpjsKey, IDocTypeKey;

public class KlaimBpjsPrintOutAddHandler : IRequestHandler<KlaimBpjsPrintOutAddCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsPrintOutAddCommand> _guard;

    public KlaimBpjsPrintOutAddHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<KlaimBpjsPrintOutAddCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(KlaimBpjsPrintOutAddCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .AddPrintOut(request, request.PrintOutReffId)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class AddPrintOutKlaimBpjsGuard : AbstractValidator<KlaimBpjsPrintOutAddCommand>
{
    public AddPrintOutKlaimBpjsGuard()
    {
        RuleFor(x => x.DocTypeId).NotEmpty();
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.PrintOutReffId).NotEmpty();
    }
}