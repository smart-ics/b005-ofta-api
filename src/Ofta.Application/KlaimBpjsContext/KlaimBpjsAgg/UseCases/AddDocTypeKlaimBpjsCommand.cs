﻿using FluentValidation;
using MediatR;
using Ofta.Application.DocContext.DocTypeAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record AddDocTypeKlaimBpjsCommand(string KlaimBpjsId, string DocTypeId)
    : IRequest, IKlaimBpjsKey, IDocTypeKey;

public class AddDocTypeKlaimBpjsHandler : IRequestHandler<AddDocTypeKlaimBpjsCommand>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<AddDocTypeKlaimBpjsCommand> _guard;
    private readonly IDocTypeBuilder _docTypeBuilder;

    public AddDocTypeKlaimBpjsHandler(IKlaimBpjsBuilder builder,
        IKlaimBpjsWriter writer,
        IValidator<AddDocTypeKlaimBpjsCommand> guard, 
        IDocTypeBuilder docTypeBuilder)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
        _docTypeBuilder = docTypeBuilder;
    }

    public Task<Unit> Handle(AddDocTypeKlaimBpjsCommand request, CancellationToken cancellationToken)
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
            .AddDocType(request)
            .AddJurnal(KlaimBpjsStateEnum.Listed, $"Add DocType {docType.DocTypeName}")
            .Build();
        
        //  WRITE
        _writer.Save(klaimBpjs);
        return Task.FromResult(Unit.Value);
    }
}

public class AddDocTypeKlaimBpjsCommandValidator : AbstractValidator<AddDocTypeKlaimBpjsCommand>
{
    public AddDocTypeKlaimBpjsCommandValidator()
    {
        RuleFor(x => x.KlaimBpjsId).NotEmpty();
        RuleFor(x => x.DocTypeId).NotEmpty();
    }
}
