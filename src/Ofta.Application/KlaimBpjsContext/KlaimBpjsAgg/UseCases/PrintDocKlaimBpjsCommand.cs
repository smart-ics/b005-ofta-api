using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record PrintDocKlaimBpjsCommand(string KlaimBpjsId, int NoUrut) : IRequest, IKlaimBpjsKey;

public class PrintDocKlaimBpjsHandler : IRequestHandler<PrintDocKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<PrintDocKlaimBpjsCommand> _guard;

    public PrintDocKlaimBpjsHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<PrintDocKlaimBpjsCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(PrintDocKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  BUILD
        var klaimBpjs = _builder
            .Load(request)
            //  TODO: Add PrintDoc
            //.PrintDoc()
            .AddJurnal(KlaimBpjsStateEnum.Sorted, "Print Doc")
            .Build();
        
        //  WRITE
        _writer.Save(klaimBpjs);
        return Task.FromResult(Unit.Value);
    }
}

public class PrintDocKlaimBpjsCommandValidator : AbstractValidator<PrintDocKlaimBpjsCommand>
{
    public PrintDocKlaimBpjsCommandValidator()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.NoUrut).GreaterThan(0);
    }
}
