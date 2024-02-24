using FluentValidation;
using MediatR;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;

public record CreateKlaimBpjsCommand(string OrderKlaimBpjsId, string UserOftaId) 
    : IRequest<CreateKlaimBpjsResponse>, IUserOftaKey, IOrderKlaimBpjsKey;

public record CreateKlaimBpjsResponse(string KlaimBpjsId);

public class CreateKlaimBpjsCommandHandler : IRequestHandler<CreateKlaimBpjsCommand, CreateKlaimBpjsResponse>
{
    private readonly IKlaimBpjsBuilder _builder;
    private readonly IKlaimBpjsWriter _writer;
    private readonly IValidator<CreateKlaimBpjsCommand> _guard;

    public CreateKlaimBpjsCommandHandler(IKlaimBpjsBuilder builder, 
        IKlaimBpjsWriter writer, 
        IValidator<CreateKlaimBpjsCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<CreateKlaimBpjsResponse> Handle(CreateKlaimBpjsCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  BUILD
        var klaimBpjs = _builder
            .Create()
            .UserOfta(request)
            .GenListBlueprint()
            .Build();
        
        //  WRITE
        var klaimBpjsModel = _writer.Save(klaimBpjs);
        return Task.FromResult(new CreateKlaimBpjsResponse(klaimBpjsModel.KlaimBpjsId));
    }
}

public class CreateKlaimBpjsCommandGuard : AbstractValidator<CreateKlaimBpjsCommand>
{
    public CreateKlaimBpjsCommandGuard()
    {
        RuleFor(x => x.OrderKlaimBpjsId).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}
