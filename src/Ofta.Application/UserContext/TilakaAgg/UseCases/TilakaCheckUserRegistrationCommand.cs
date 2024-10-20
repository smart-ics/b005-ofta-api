using Dawn;
using FluentAssertions;
using Mapster;
using MediatR;
using Moq;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;
using Xunit;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaCheckUserRegistrationCommand(string Email): IRequest<TilakaCheckUserRegistrationResponse>;

public record TilakaCheckUserRegistrationResponse(
    string RegistrationId,
    string RegistrationStatus,
    string ManualRegistrationStatus,
    string ReasonCode,
    string PhotoSelfie,
    string TilakaName,
    string UserState
);

public class TilakaCheckUserRegistrationHandler: IRequestHandler<TilakaCheckUserRegistrationCommand, TilakaCheckUserRegistrationResponse>
{
    private readonly ICheckUserRegistrationService _checkUserRegistration;
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public TilakaCheckUserRegistrationHandler(ICheckUserRegistrationService checkUserRegistration, ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _checkUserRegistration = checkUserRegistration;
        _builder = builder;
        _writer = writer;
    }

    public Task<TilakaCheckUserRegistrationResponse> Handle(TilakaCheckUserRegistrationCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty());

        // BUILD
        var aggregate = _builder
            .Load(request.Email)
            .Build();
        
        if (aggregate.RegistrationId == string.Empty)
            throw new ArgumentException($"Registration id from email: {request.Email} not found");

        var checkUserReg = _checkUserRegistration.Execute(new CheckUserRegistrationRequest(aggregate.RegistrationId));
        if (!checkUserReg.Success)
            throw new ArgumentException(checkUserReg.Message);

        if (checkUserReg.ManualRegistrationStatus != string.Empty)
            aggregate = _builder
                .Attach(aggregate)
                .UserState(TilakaUserState.ManualRegistration)
                .Build();
        
        if (checkUserReg.RegistrationStatus == "S" || checkUserReg.ManualRegistrationStatus == "S")
            aggregate = _builder
                .Attach(aggregate)
                .UserState(TilakaUserState.Verified)
                .TilakaName(checkUserReg.TilakaName)
                .Build();

        // WRITE
        _ = _writer.Save(aggregate);
        var response = new TilakaCheckUserRegistrationResponse(
            aggregate.RegistrationId,
            checkUserReg.RegistrationStatus,
            checkUserReg.ManualRegistrationStatus,
            checkUserReg.ReasonCode,
            checkUserReg.PhotoSelfie,
            aggregate.TilakaName,
            aggregate.UserState.ToString()
        );
        return Task.FromResult(response);
    }
}

public class TilakaCheckUserRegistrationHandlerTest
{
    private readonly Mock<ICheckUserRegistrationService> _checkUserRegistration;
    private readonly Mock<ITilakaUserBuilder> _builder;
    private readonly Mock<ITilakaUserWriter> _writer;
    private readonly TilakaCheckUserRegistrationHandler _sut;

    public TilakaCheckUserRegistrationHandlerTest()
    {
        _checkUserRegistration = new Mock<ICheckUserRegistrationService>();
        _builder = new Mock<ITilakaUserBuilder>();
        _writer = new Mock<ITilakaUserWriter>();
        _sut = new TilakaCheckUserRegistrationHandler(_checkUserRegistration.Object, _builder.Object, _writer.Object);
    }
    
    [Fact]
    public async Task GivenCheckUserRegFailed_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaCheckUserRegistrationCommand("A");
        var registrationData = new TilakaUserModel
        {
            RegistrationId = "A",
        };

        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(registrationData);
        
        var expectedCheckUser = new CheckUserRegistrationResponse(false, "", "", "", "", "", "");
        _checkUserRegistration.Setup(x => x.Execute(It.IsAny<CheckUserRegistrationRequest>()))
            .Returns(expectedCheckUser);

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenManualRegStatusNotEmpty_ThenUpdateUserStateToManualRegistration()
    {
        // ARRANGE
        var request = new TilakaCheckUserRegistrationCommand("A");
        var registrationData = new TilakaUserModel
        {
            RegistrationId = "A",
        };

        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(registrationData);
    
        var expectedCheckUser = new CheckUserRegistrationResponse(true, "", "", "", "A", "", "");
        _checkUserRegistration.Setup(x => x.Execute(It.IsAny<CheckUserRegistrationRequest>()))
            .Returns(expectedCheckUser);
        
        var updatedRegData = new TilakaUserModel
        {
            RegistrationId = "A",
            UserState = TilakaUserState.ManualRegistration,
        };
        
        _builder.Setup(x => x.Attach(It.IsAny<TilakaUserModel>()).UserState(It.IsAny<TilakaUserState>()).Build())
            .Returns(updatedRegData);
    
        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);
    
        // ACT
        await _sut.Handle(request, CancellationToken.None);
    
        // ASSERT
        actual?.UserState.Should().Be(TilakaUserState.ManualRegistration);
    }
    
    [Fact]
    public async Task GivenManualRegStatusSuccess_ThenUpdateUserStateToVerifiedAndTilakaName()
    {
        // ARRANGE
        var request = new TilakaCheckUserRegistrationCommand("A");
        var registrationData = new TilakaUserModel
        {
            RegistrationId = "A",
        };

        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(registrationData);
    
        var expectedCheckUser = new CheckUserRegistrationResponse(true, "", "A", "", "S", "", "");
        _checkUserRegistration.Setup(x => x.Execute(It.IsAny<CheckUserRegistrationRequest>()))
            .Returns(expectedCheckUser);
        
        var updatedRegData = new TilakaUserModel
        {
            RegistrationId = "A",
            UserState = TilakaUserState.Verified,
            TilakaName = expectedCheckUser.TilakaName
        };
        
        _builder.Setup(x => x.Attach(It.IsAny<TilakaUserModel>()).UserState(It.IsAny<TilakaUserState>()).TilakaName(It.IsAny<string>()).Build())
            .Returns(updatedRegData);
    
        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);
    
        // ACT
        await _sut.Handle(request, CancellationToken.None);
    
        // ASSERT
        actual?.UserState.Should().Be(TilakaUserState.Verified);
        actual?.TilakaName.Should().BeEquivalentTo(updatedRegData.TilakaName);
    }
    
    [Fact]
    public async Task GivenRegStatusSuccess_ThenUpdateUserStateToVerifiedAndTilakaName()
    {
        // ARRANGE
        var request = new TilakaCheckUserRegistrationCommand("A");
        var registrationData = new TilakaUserModel
        {
            RegistrationId = "A",
        };

        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(registrationData);
    
        var expectedCheckUser = new CheckUserRegistrationResponse(true, "", "A", "S", "", "", "");
        _checkUserRegistration.Setup(x => x.Execute(It.IsAny<CheckUserRegistrationRequest>()))
            .Returns(expectedCheckUser);
        
        var updatedRegData = new TilakaUserModel
        {
            RegistrationId = "A",
            UserState = TilakaUserState.Verified,
            TilakaName = expectedCheckUser.TilakaName
        };
        
        _builder.Setup(x => x.Attach(It.IsAny<TilakaUserModel>()).UserState(It.IsAny<TilakaUserState>()).TilakaName(It.IsAny<string>()).Build())
            .Returns(updatedRegData);
    
        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);
    
        // ACT
        await _sut.Handle(request, CancellationToken.None);
    
        // ASSERT
        actual?.UserState.Should().Be(TilakaUserState.Verified);
        actual?.TilakaName.Should().BeEquivalentTo(updatedRegData.TilakaName);
    }
}