using Dawn;
using MediatR;
using Ofta.Application.UserContext.UserOftaAgg.Workers;

namespace Ofta.Application.UserContext.UserOftaAgg.UseCases;

public record GetUserQuery(string Email) : IRequest<GetUserResponse>;

public record GetUserResponse(
    string UserOftaId, 
    string UserOftaName, 
    string Email,
    bool IsVerified);

public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUserBuilder _builder;

    public GetUserHandler(IUserBuilder builder)
    {
        _builder = builder;
    }

    public Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty());
        
        //  BUILD
        var user = _builder
            .Load(request.Email)
            .Build();
        
        var response = new GetUserResponse(user.UserOftaId, user.UserOftaName, user.Email, user.IsVerified);
        return Task.FromResult(response);
    }
}