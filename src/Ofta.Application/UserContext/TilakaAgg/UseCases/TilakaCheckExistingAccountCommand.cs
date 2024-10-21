using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Xunit;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaCheckExistingAccountCommand(string NomorIdentitas): IRequest<TilakaCheckExistingAccountResponse>;

public record TilakaCheckExistingAccountResponse(
    string NomorIdentitas,
    string TilakaId,
    string Message,
    bool Status
);

public class TilakaCheckExistingAccountHandler: IRequestHandler<TilakaCheckExistingAccountCommand, TilakaCheckExistingAccountResponse>
{
    private readonly IGenerateUuidTilakaService _generateUuid;
    private readonly ICheckExistingAccountService _checkExistingAccount;

    public TilakaCheckExistingAccountHandler(IGenerateUuidTilakaService generateUuid, ICheckExistingAccountService checkExistingAccount)
    {
        _generateUuid = generateUuid;
        _checkExistingAccount = checkExistingAccount;
    }

    public Task<TilakaCheckExistingAccountResponse> Handle(TilakaCheckExistingAccountCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.NomorIdentitas, y => y.NotEmpty())
            .Member(x => x.NomorIdentitas, y => y.MinLength(16));
        
        // EXECUTE
        var uuid = _generateUuid.Execute();
        if (!uuid.Success)
            throw new ArgumentException(uuid.Message);

        var req = new CheckExistingAccountRequest(uuid.RegistrationId, request.NomorIdentitas);
        var checkExistingAccount = _checkExistingAccount.Execute(req);
        
        // RESPONSE
        var response = new TilakaCheckExistingAccountResponse(request.NomorIdentitas, checkExistingAccount.TilakaId,
            checkExistingAccount.Message, checkExistingAccount.Status);
        return Task.FromResult(response);
    }
}

public class TilakaCheckExistingAccountHandlerTest
{
    private readonly Mock<IGenerateUuidTilakaService> _generateUuid;
    private readonly Mock<ICheckExistingAccountService> _checkExistingAccount;
    private readonly TilakaCheckExistingAccountHandler _sut;

    public TilakaCheckExistingAccountHandlerTest()
    {
        _generateUuid = new Mock<IGenerateUuidTilakaService>();
        _checkExistingAccount = new Mock<ICheckExistingAccountService>();
        _sut = new TilakaCheckExistingAccountHandler(_generateUuid.Object, _checkExistingAccount.Object);
    }

    [Fact]
    public async Task GivenNomorIdentitasLengthLessThan16_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaCheckExistingAccountCommand("A");

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenGenerateUuidFailed_ThenThrowArgumentException()
    {
        // ARRANGE
        var request = new TilakaCheckExistingAccountCommand("1122334455667788");
        var expected = new GenerateUuidTilakaResponse(false, "", "");
        _generateUuid.Setup(x => x.Execute()).Returns(expected);

        // ACT
        var actual = async () => await _sut.Handle(request, CancellationToken.None);

        // ASSERT
        await actual.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GivenGenerateUuidAndAccountIsExist_ThenReturnExpected()
    {
        // ARRANGE
        var request = new TilakaCheckExistingAccountCommand("1122334455667788");
        
        var uuid = new GenerateUuidTilakaResponse(true, "", "D");
        _generateUuid.Setup(x => x.Execute()).Returns(uuid);
        
        var expected = new CheckExistingAccountResponse(true, "A", "D");
        _checkExistingAccount.Setup(x => x.Execute(It.IsAny<CheckExistingAccountRequest>())).Returns(expected);
        
        // ACT
        var actual = await _sut.Handle(request, CancellationToken.None);
        
        // ASSERT
        actual.Status.Should().BeTrue();
        actual.Message.Should().BeEquivalentTo("A");
        actual.TilakaId.Should().BeEquivalentTo("D");
    }
}