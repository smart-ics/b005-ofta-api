using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;
using Xunit;

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

        var revokeRequest = new RequestRevokeRequest(aggregate.TilakaName, request.RevokeReason);
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

public class TilakaRequestRevokeCertificateHandlerTest
{
    private readonly Mock<IRequestRevokeService> _requestRevoke;
    private readonly Mock<ITilakaUserBuilder> _builder;
    private readonly Mock<ITilakaUserWriter> _writer;
    private readonly TilakaRequestRevokeCertificateHandler _sut;

    public TilakaRequestRevokeCertificateHandlerTest()
    {
        _requestRevoke = new Mock<IRequestRevokeService>();
        _builder = new Mock<ITilakaUserBuilder>();
        _writer = new Mock<ITilakaUserWriter>();
        _sut = new TilakaRequestRevokeCertificateHandler(_requestRevoke.Object, _builder.Object, _writer.Object);
    }

    [Fact]
    public async Task GivenRequestRevokeFailed_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaRequestRevokeCertificateCommand("A", "B");
        var tilakaUser = new TilakaUserModel
        {
            Email = "A",
            TilakaName = "C"
        };
        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(tilakaUser);

        var revokeRequest = new RequestRevokeResponse(false, "", "", "");
        _requestRevoke.Setup(x => x.Execute(It.IsAny<RequestRevokeRequest>()))
            .Returns(revokeRequest);

        // ACT
        var act = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenRequestRevokeSuccess_ThenReturnExpected()
    {
        // ARRANGE
        var request = new TilakaRequestRevokeCertificateCommand("A", "B");
        var tilakaUser = new TilakaUserModel
        {
            Email = "A",
            RevokeReason = "B",
            TilakaName = "C",
            UserState = TilakaUserState.RevokeRequested
        };
        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(tilakaUser);

        var revokeRequest = new RequestRevokeResponse(true, "A", "B", "C");
        _requestRevoke.Setup(x => x.Execute(It.IsAny<RequestRevokeRequest>()))
            .Returns(revokeRequest);

        _builder.Setup(x =>
                x.Attach(It.IsAny<TilakaUserModel>()).UserState(It.IsAny<TilakaUserState>())
                    .RevokeReason(It.IsAny<string>()).Build())
            .Returns(tilakaUser);

        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);

        // ACT
        await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        actual?.UserState.Should().Be(TilakaUserState.RevokeRequested);
        actual?.RevokeReason.Should().BeEquivalentTo(tilakaUser.RevokeReason);
    }
}