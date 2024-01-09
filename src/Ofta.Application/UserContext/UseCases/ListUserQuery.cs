using MediatR;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Application.UserContext.UseCases;

public record ListUserQuery : IRequest<IEnumerable<ListUserResponse>>;

public record ListUserResponse(string UserOftaId, string UserOftaName, string Email, bool IsVerified);

public class ListUserHandler : IRequestHandler<ListUserQuery, IEnumerable<ListUserResponse>>
{
    private readonly IUserOftaDal _userOftaDal;

    public ListUserHandler(IUserOftaDal userOftaDal)
    {
        _userOftaDal = userOftaDal;
    }

    public Task<IEnumerable<ListUserResponse>> Handle(ListUserQuery request, CancellationToken cancellationToken)
    {
        var listUser = _userOftaDal.ListData().ToList() ?? new List<UserOftaModel>();
        var response = listUser.Select(x => new ListUserResponse(x.UserOftaId, x.UserOftaName, x.Email, x.IsVerified));
        return Task.FromResult(response);
    }
} 