using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.UserOftaAgg;
using Xunit;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaUserGetByUserMappingQuery(string UserId, string UserType) : IRequest<TilakaUserGetByUserMappingResponse>;

public record TilakaUserGetByUserMappingResponse(
    string UserOftaId,
    string TilakaName
);

public class TilakaUserGetByUserMappingHandler: IRequestHandler<TilakaUserGetByUserMappingQuery, TilakaUserGetByUserMappingResponse>
{
    private readonly IUserOftaMappingDal _userOftaMappingDal;
    private readonly ITilakaUserDal _tilakaUserDal;

    public TilakaUserGetByUserMappingHandler(IUserOftaMappingDal userOftaMappingDal, ITilakaUserDal tilakaUserDal)
    {
        _userOftaMappingDal = userOftaMappingDal;
        _tilakaUserDal = tilakaUserDal;
    }

    public Task<TilakaUserGetByUserMappingResponse> Handle(TilakaUserGetByUserMappingQuery request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.UserId, y => y.NotEmpty())
            .Member(x => x.UserType, y => y.NotEmpty());

        var successParse = Enum.TryParse<UserTypeEnum>(request.UserType, out var userType);
        if (!successParse)
            throw new ArgumentException($"UserType '{request.UserType}' is not supported");
        
        var userMapping = _userOftaMappingDal.ListData(request.UserId)
            .FirstOrDefault(x => x.UserType == userType);

        if (userMapping is null)
            throw new KeyNotFoundException($"User Emr with id: {request.UserId} not found");

        var tilakaUser = _tilakaUserDal.GetData(userMapping)
            ?? throw new KeyNotFoundException($"Tilaka User mapped to {request.UserId} not found");

        // RESPONSE
        var response = new TilakaUserGetByUserMappingResponse(tilakaUser.UserOftaId, tilakaUser.TilakaName);
        return Task.FromResult(response);
    }
}