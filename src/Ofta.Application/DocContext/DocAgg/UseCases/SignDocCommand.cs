﻿using Dawn;
using MediatR;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.UseCases;

public interface ISignDocTarget
{
    string DocId { get; init; }
    string Email { get; init; }
}

public record SignDocCommand(string DocId, string Email) : IRequest, ISignDocTarget;

public class SignDocHandler : IRequestHandler<SignDocCommand>
{
    private readonly IDocBuilder _builder;
    private readonly IDocWriter _writer;
    private readonly IDocDal _docDal;
    private readonly ITilakaUserBuilder _tilakaUserBuilder;
    private readonly IExecuteSignToSignProviderService _executeSignToSignProviderService;

    public SignDocHandler(IDocBuilder builder, IDocWriter writer, IDocDal docDal, ITilakaUserBuilder tilakaUserBuilder,
        IExecuteSignToSignProviderService executeSignToSignProviderService)
    {
        _builder = builder;
        _writer = writer;
        _docDal = docDal;
        _tilakaUserBuilder = tilakaUserBuilder;
        _executeSignToSignProviderService = executeSignToSignProviderService;
    }

    public Task<Unit> Handle(SignDocCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.DocId, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());

        //  BUILD
        var doc = _builder
            .Load(new DocModel(request.DocId))
            .Build();

        var tilakaUser = _tilakaUserBuilder
            .Load(request.Email)
            .Build();

        var userProvider = tilakaUser is not null ? tilakaUser.TilakaName : string.Empty;

        var signee = doc.ListSignees.FirstOrDefault(x => x.Email == request.Email) ?? new DocSigneeModel();
        var executeSignToSignProviderRequest = new ExecuteSignToSignProviderRequest(doc, signee, userProvider);
        var executesignToSignProviderResponse =
            _executeSignToSignProviderService.Execute(executeSignToSignProviderRequest);

        if (!executesignToSignProviderResponse.Status)
        {
            throw new KeyNotFoundException(executesignToSignProviderResponse.Message);
        }

        var aggregate = _builder
            .Load(doc)
            .Sign(request.Email)
            .AddJurnal(DocStateEnum.Signed, request.Email)
            .Build();

        if (doc.ListSignees.Count > 1)
        {
            var index = doc.ListSignees.FindIndex(x => x.Email == signee.Email);
            if (index >= 0 && index != doc.ListSignees.Count - 1)
                doc.ListSignees.ElementAt(index + 1).IsHidden = false;
        }

        //  WRITE
        _writer.Save(aggregate);
        return Task.FromResult(Unit.Value);
    }
}