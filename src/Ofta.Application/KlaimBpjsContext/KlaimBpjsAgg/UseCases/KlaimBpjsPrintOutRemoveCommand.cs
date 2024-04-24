using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsPrintOutRemoveCommand(string KlaimBpjsId, string PrintOutReffId)
    : IRequest, IKlaimBpjsKey;

public class KlaimBpjsPrintOutRemoveHandler : IRequestHandler<KlaimBpjsPrintOutRemoveCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsPrintOutRemoveCommand> _guard;

    public KlaimBpjsPrintOutRemoveHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<KlaimBpjsPrintOutRemoveCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(KlaimBpjsPrintOutRemoveCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .RemovePrintOut(request.PrintOutReffId)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class RemovePrintOutKlaimBpjsGuard : AbstractValidator<KlaimBpjsPrintOutRemoveCommand>
{
    public RemovePrintOutKlaimBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.PrintOutReffId).NotEmpty();
    }
}