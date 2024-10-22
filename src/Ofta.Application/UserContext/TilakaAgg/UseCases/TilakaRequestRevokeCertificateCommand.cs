using Dawn;
using MediatR;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaRequestRevokeCertificateCommand(string Email, string RevokeReason): IRequest<TilakaRequestRevokeCertificateResponse>;

public record TilakaRequestRevokeCertificateResponse(
    string Message,
    string Email,
    string TilakaName,
    string RevokeReason,
    string RevokeToken,
    string UrlWebview
);

public class TilakaRequestRevokeCertificateHandler: IRequestHandler<TilakaRequestRevokeCertificateCommand, TilakaRequestRevokeCertificateResponse>
{
    private readonly IRequestRevokeService _requestRevoke;
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public TilakaRequestRevokeCertificateHandler(IRequestRevokeService requestRevoke, ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _requestRevoke = requestRevoke;
        _builder = builder;
        _writer = writer;
    }

    public Task<TilakaRequestRevokeCertificateResponse> Handle(TilakaRequestRevokeCertificateCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty())
            .Member(x => x.RevokeReason, y => y.NotEmpty());

        // BUILD
        var aggregate = _builder
            .Load(request.Email)
            .Build();

        var revokeRequest = new RequestRevokeRequest(aggregate.TilakaName, aggregate.RevokeReason);
        var requestRevokeUser = _requestRevoke.Execute(revokeRequest);
        if (!requestRevokeUser.Success)
            throw new ArgumentException(requestRevokeUser.Message);

        aggregate = _builder
            .Attach(aggregate)
            .UserState(TilakaUserState.RevokeRequested)
            .RevokeReason(request.RevokeReason)
            .Build();

        // WRITE
        _ = _writer.Save(aggregate);
        var response = new TilakaRequestRevokeCertificateResponse(
            requestRevokeUser.Message,
            aggregate.Email,
            aggregate.TilakaName,
            aggregate.RevokeReason,
            requestRevokeUser.RevokeToken,
            requestRevokeUser.UrlWebview
        );
        return Task.FromResult(response);
    }
}