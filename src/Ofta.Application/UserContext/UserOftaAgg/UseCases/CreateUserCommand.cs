using Dawn;
using MediatR;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;

namespace Ofta.Application.UserContext.UserOftaAgg.UseCases;

public record CreateUserCommand(string UserOftaName, string Email) :
    IRequest<CreateUserResponse>;

public record CreateUserResponse(string UserOftaId);

public class CreateUserHanler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserBuilder _builder;
    private readonly IUserWriter _writer;

    public CreateUserHanler(IUserBuilder builder, 
        IUserWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.UserOftaName, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var fallback = Policy<UserOftaModel>
            .Handle<KeyNotFoundException>()
            .Fallback(() => _builder
                .Create()
                .UserOftaName(request.UserOftaName)
                .Email(request.Email)
                .Build());
        
        var aggregate = fallback.Execute(() =>
            _builder.Load(request.Email).Build());

        // WRITE
        aggregate = _writer.Save(aggregate);
        var response = new CreateUserResponse(aggregate.UserOftaId);
        return Task.FromResult(response);
    }
}