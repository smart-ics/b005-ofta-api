using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Xunit;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaCheckUserCertificateCommand(string Email): IRequest<TilakaCheckUserCertificateResponse>;

public record MessageResponse(string Info);

public record DataResponse(
    string Status,
    string Serialnumber,
    string SubjectDn,
    string StartActiveDate,
    string ExpiryDate,
    string Certificate
);

public record TilakaCheckUserCertificateResponse(
    string RegistrationId,
    string TilakaName,
    string UserState,
    string CertificateStatus,
    int Status,
    MessageResponse Message,
    IEnumerable<DataResponse> Data
);

public class TilakaCheckUserCertificateHandler: IRequestHandler<TilakaCheckUserCertificateCommand, TilakaCheckUserCertificateResponse>
{
    private readonly ICheckUserCertificateService _checkUserCertificate;
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public TilakaCheckUserCertificateHandler(ICheckUserCertificateService checkUserCertificate, ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _checkUserCertificate = checkUserCertificate;
        _builder = builder;
        _writer = writer;
    }

    public Task<TilakaCheckUserCertificateResponse> Handle(TilakaCheckUserCertificateCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var aggregate = _builder
            .Load(request.Email)
            .Build();

        var checkUserCertificate = _checkUserCertificate.Execute(new CheckUserCertificateRequest(aggregate.TilakaName));
        if (!checkUserCertificate.Success)
            throw new ArgumentException(checkUserCertificate.Message.Info);
        
        aggregate = _builder
            .Attach(aggregate)
            .CertificateState((TilakaCertificateState)checkUserCertificate.Status)
            .Build();

        // WRITE
        _ = _writer.Save(aggregate);
        var response = new TilakaCheckUserCertificateResponse(
            aggregate.RegistrationId,
            aggregate.TilakaName,
            aggregate.UserState.ToString(), 
            aggregate.CertificateState.ToString(),
            checkUserCertificate.Status,
            new MessageResponse(checkUserCertificate.Message.Info),
            checkUserCertificate.Data.Select(x => new DataResponse(x.Status, x.Serialnumber, x.SubjectDn, x.StartActiveDate, x.ExpiryDate, x.Certificate))
        );
        return Task.FromResult(response);
    }
}

public class TilakaCheckUserCertificateHandlerTest
{
    private readonly Mock<ICheckUserCertificateService> _checkUserCertificate;
    private readonly Mock<ITilakaUserBuilder> _builder;
    private readonly Mock<ITilakaUserWriter> _writer;
    private readonly TilakaCheckUserCertificateHandler _sut;

    public TilakaCheckUserCertificateHandlerTest()
    {
        _checkUserCertificate = new Mock<ICheckUserCertificateService>();
        _builder = new Mock<ITilakaUserBuilder>();
        _writer = new Mock<ITilakaUserWriter>();
        _sut = new TilakaCheckUserCertificateHandler(_checkUserCertificate.Object, _builder.Object, _writer.Object);
    }

    [Fact]
    public async Task GivenCheckUserCertFailed_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaCheckUserCertificateCommand("A");
        var registrationData = new TilakaUserModel
        {
            RegistrationId = "A",
            TilakaName = "B"
        };

        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(registrationData);
        
        var expectedCheckUser = new CheckUserCertificateResponse(false, 0, new MessageDto(""), new List<DataDto>());
        _checkUserCertificate.Setup(x => x.Execute(It.IsAny<CheckUserCertificateRequest>()))
            .Returns(expectedCheckUser);

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenCheckUserCertSuccess_ThenUpdateCertificateState()
    {
        // ARRANGE
        var request = new TilakaCheckUserCertificateCommand("A");
        var registrationData = new TilakaUserModel
        {
            RegistrationId = "A",
            TilakaName = "B"
        };

        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(registrationData);
        
        var expectedCheckUser = new CheckUserCertificateResponse(true, 3, new MessageDto(""), new List<DataDto>());
        _checkUserCertificate.Setup(x => x.Execute(It.IsAny<CheckUserCertificateRequest>()))
            .Returns(expectedCheckUser);
        
        var updatedRegData = new TilakaUserModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            CertificateState = (TilakaCertificateState)expectedCheckUser.Status
        };
        
        _builder.Setup(x => x.Attach(It.IsAny<TilakaUserModel>()).CertificateState(It.IsAny<TilakaCertificateState>()).Build())
            .Returns(updatedRegData);

        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);

        // ACT
        await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        actual?.CertificateState.Should().Be(TilakaCertificateState.Active);
    }
}