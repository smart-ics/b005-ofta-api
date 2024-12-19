using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Polly;
using Xunit;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaRegistrationCommand(string UserOftaId, string NamaKTP, string NomorIdentitas, string FotoKtpBase64)
    : IRequest<TilakaRegistrationResponse>, IUserOftaKey;

public record TilakaRegistrationResponse(string RegistrationId);

public class TilakaRegistrationHandler : IRequestHandler<TilakaRegistrationCommand, TilakaRegistrationResponse>
{
    private TilakaUserModel _aggregate = new();
    private readonly IGenerateUuidTilakaService _generateUuid;
    private readonly IRegisterUserTilakaService _registerUser;
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public TilakaRegistrationHandler(IGenerateUuidTilakaService generateUuid, IRegisterUserTilakaService registerUser,
        ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _generateUuid = generateUuid;
        _registerUser = registerUser;
        _builder = builder;
        _writer = writer;
    }

    public Task<TilakaRegistrationResponse> Handle(TilakaRegistrationCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.UserOftaId, y => y.NotEmpty())
            .Member(x => x.NamaKTP, y => y.NotEmpty())
            .Member(x => x.NomorIdentitas, y => y.NotEmpty())
            .Member(x => x.FotoKtpBase64, y => y.NotEmpty());

        // BUILD
        var uuid = _generateUuid.Execute();
        if (!uuid.Success)
            throw new ArgumentException(uuid.Message);
        
        _aggregate = _builder
            .Create()
            .RegistrationId(uuid)
            .UserOfta(request)
            .Identitas(request.NamaKTP, request.NomorIdentitas, request.FotoKtpBase64)
            .Build();
        
        var registerToTilaka = _registerUser.Execute(new RegisterUserTilakaRequest(_aggregate));
        if (!registerToTilaka.Success)
            throw new ArgumentException(registerToTilaka.Message);
        
        // WRITE
        _ = _writer.Save(_aggregate);
        return Task.FromResult(new TilakaRegistrationResponse(registerToTilaka.RegisterId));
    }
}

public class TilakaRegistrationHandlerTest
{
    private readonly Mock<IGenerateUuidTilakaService> _generateUuid;
    private readonly Mock<IRegisterUserTilakaService> _registerUser;
    private readonly Mock<ITilakaUserBuilder> _builder;
    private readonly Mock<ITilakaUserWriter> _writer;
    private readonly TilakaRegistrationHandler _sut;

    public TilakaRegistrationHandlerTest()
    {
        _generateUuid = new Mock<IGenerateUuidTilakaService>();
        _registerUser = new Mock<IRegisterUserTilakaService>();
        _builder = new Mock<ITilakaUserBuilder>();
        _writer = new Mock<ITilakaUserWriter>();
        _sut = new TilakaRegistrationHandler(_generateUuid.Object, _registerUser.Object, _builder.Object, _writer.Object);
    }

    [Fact]
    public async Task GivenGenerateUuidFailed_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaRegistrationCommand("A", "B", "C", "D");
        var expected = new GenerateUuidTilakaResponse(false, "", "");
        _generateUuid.Setup(x => x.Execute()).Returns(expected);

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenRegisterUserFailed_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaRegistrationCommand("A", "B", "C", "D");
        
        var uuid = new GenerateUuidTilakaResponse(true, "", "D");
        _generateUuid.Setup(x => x.Execute()).Returns(uuid);
        
        var newUser = new TilakaUserModel
        {
            ExpiredDate = DateTime.Now.AddYears(1),
            UserState = TilakaUserState.Created,
            CertificateState = TilakaCertificateState.NoCertificate,
            RegistrationId = uuid.RegistrationId,
            UserOftaId = request.UserOftaId,
            NomorIdentitas = request.NomorIdentitas,
            FotoKtpBase64 = request.FotoKtpBase64,
        };
        
        _builder.Setup(x
            => x.Create()
                .RegistrationId(It.IsAny<ITilakaRegistrationKey>())
                .UserOfta(It.IsAny<IUserOftaKey>())
                .Identitas(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                .Build()
        ).Returns(newUser);
        
        var expected = new RegisterUserTilakaResponse(false, "", "");
        _registerUser.Setup(x => x.Execute(It.IsAny<RegisterUserTilakaRequest>())).Returns(expected);

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenGenerateUuidAndRegisterSuccess_ThenCreateUserRegObject()
    {
        // ARRANGE
        var request = new TilakaRegistrationCommand("A", "B", "C", "D");
        
        var uuid = new GenerateUuidTilakaResponse(true, "", "D");
        _generateUuid.Setup(x => x.Execute()).Returns(uuid);
        
        var newUser = new TilakaUserModel
        {
            ExpiredDate = DateTime.Now.AddYears(1),
            UserState = TilakaUserState.Created,
            CertificateState = TilakaCertificateState.NoCertificate,
            RegistrationId = uuid.RegistrationId,
            UserOftaId = request.UserOftaId,
            NomorIdentitas = request.NomorIdentitas,
            FotoKtpBase64 = request.FotoKtpBase64,
        };
        
        _builder.Setup(x
            => x.Create()
                .RegistrationId(It.IsAny<ITilakaRegistrationKey>())
                .UserOfta(It.IsAny<IUserOftaKey>())
                .Identitas(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                .Build()
            ).Returns(newUser);
        
        var expected = new RegisterUserTilakaResponse(true, "", "D");
        _registerUser.Setup(x => x.Execute(It.IsAny<RegisterUserTilakaRequest>())).Returns(expected);

        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);
        
        // ACT
        await _sut.Handle(request, CancellationToken.None);
        
        // ASSERT
        actual?.RegistrationId.Should().BeEquivalentTo(expected.RegisterId);
        actual?.UserState.Should().Be(TilakaUserState.Created);
    }
}