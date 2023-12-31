using Dawn;
using Mapster;
using MediatR;
using Ners.Application.BillingContext.UserAgg.Contracts;
using Nuna.Lib.ValidationHelper;
using Ofta.Domain.UserAgg;

namespace Ofta.Application.UserContext.UseCases;

public record GetUserUsmanQuery(string UserEmail, string Pass) : IRequest<GetUserUsmanResponse>, IUserKey;

public class GetUserUsmanResponse
{
    public string PegId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string ExpiredDate { get; set; }
    public string Token { get; set; }
}

public class GetUserUsmanHandler : IRequestHandler<GetUserUsmanQuery, GetUserUsmanResponse>
{
    private readonly IUsmanGetUserService _service;

    public GetUserUsmanHandler(IUsmanGetUserService service)
    {
        _service = service;
    }

    public Task<GetUserUsmanResponse> Handle(GetUserUsmanQuery request, CancellationToken cancellationToken)
    {
        // GUARD 
        Guard.Argument(() => request).NotNull()
            .Member(x => x.UserEmail, y => y.NotEmpty())
            .Member(x => x.Pass, y => y.NotEmpty());

        // BUILD
        var req = new UsmanGetUserDto
        {
            Email = request.UserEmail,
            Pass = request.Pass
        };
        var user = _service.Execute(req);
        if (user is null)
            throw new KeyNotFoundException("User not found");
        var result = new UserModel
        {
            PegId = user.pegId,
            UserName = user.userName,
            UserEmail = user.email,
            ExpiredDate = user.expiredDate.ToDate()
        };

        // RESPONSE
        return Task.FromResult(GenResponse(result));
    }

    private GetUserUsmanResponse GenResponse(UserModel user)
    {
        var result = user.Adapt<GetUserUsmanResponse>();
        result.ExpiredDate = user.ExpiredDate.ToString(DateFormatEnum.YMD);
        result.Token = string.Empty;
        return result;
    }
}