using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;


namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;


public record KlaimBpjsMergerFileCommand(string KlaimBpjsId)
    : IRequest, IKlaimBpjsKey;

public class KlaimBpjsMergerFileHandler : IRequestHandler<KlaimBpjsMergerFileCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsMergerFileCommand> _guard;
    private readonly IMediator _mediator;

    public KlaimBpjsMergerFileHandler(IKlaimBpjsBuilder builder,
        IKlaimBpjsWriter writer,
        IValidator<KlaimBpjsMergerFileCommand> guard,
        IMediator mediator)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _mediator = mediator;
    }

    public Task<Unit> Handle(KlaimBpjsMergerFileCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);

        //  BUILD
        var agg = _builder
            .Load(request)
            .Build();

        //  WRITE
        _writer.Save(agg);
        _mediator.Publish(new MergerFileBpjsEvent(agg, request), cancellationToken);
        return Task.FromResult(Unit.Value);
    }
}

public record MergerFileBpjsEvent(
    KlaimBpjsModel Agg,
    KlaimBpjsMergerFileCommand Command) : INotification;

public class MergerFileBpjsGuard : AbstractValidator<KlaimBpjsMergerFileCommand>
{
    public MergerFileBpjsGuard()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
    }
}
