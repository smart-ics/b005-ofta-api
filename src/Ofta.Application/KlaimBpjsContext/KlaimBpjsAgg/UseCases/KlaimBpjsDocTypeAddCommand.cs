using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record KlaimBpjsDocTypeAddCommand(string KlaimBpjsId, string DocTypeId)
    : IRequest, IKlaimBpjsKey, IDocTypeKey;

public class KlaimBpjsDocTypeAddHandler : IRequestHandler<KlaimBpjsDocTypeAddCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<KlaimBpjsDocTypeAddCommand> _guard;
    private readonly IDocTypeBuilder _docTypeBuilder;

    public KlaimBpjsDocTypeAddHandler(IKlaimBpjsBuilder builder,
        IKlaimBpjsWriter writer,
        IValidator<KlaimBpjsDocTypeAddCommand> guard, 
        IDocTypeBuilder docTypeBuilder)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _docTypeBuilder = docTypeBuilder;
    }

    public Task<Unit> Handle(KlaimBpjsDocTypeAddCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  BUILD
        var docType = _docTypeBuilder
            .Load(request)
            .Build();
        var klaimBpjs = _builder
            .Load(request)
            .AddDocType(request, true)
            .AddEvent(KlaimBpjsStateEnum.InProgress, $"Add DocType {docType.DocTypeName}")
            .Build();
        
        //  WRITE
        _writer.Save(klaimBpjs);
        return Task.FromResult(Unit.Value);
    }
}

public class AddDocTypeKlaimBpjsCommandValidator : AbstractValidator<KlaimBpjsDocTypeAddCommand>
{
    public AddDocTypeKlaimBpjsCommandValidator()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
    }
}
