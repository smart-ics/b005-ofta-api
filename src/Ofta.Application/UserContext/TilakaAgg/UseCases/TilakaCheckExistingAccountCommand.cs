using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Xunit;

namespace Ofta.Application.UserContext.TilakaAgg.UseCases;

public record TilakaCheckExistingAccountCommand(string Email): IRequest<TilakaCheckExistingAccountResponse>;

public record TilakaCheckExistingAccountResponse(
    string NomorIdentitas,
    string Email,
    string TilakaName,
    string TilakaId,
    string Message,
    bool Status
);

public class TilakaCheckExistingAccountHandler: IRequestHandler<TilakaCheckExistingAccountCommand, TilakaCheckExistingAccountResponse>
{
    private readonly IGenerateUuidTilakaService _generateUuid;
    private readonly ICheckExistingAccountService _checkExistingAccount;
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public TilakaCheckExistingAccountHandler(IGenerateUuidTilakaService generateUuid, ICheckExistingAccountService checkExistingAccount, ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
        _generateUuid = generateUuid;
        _checkExistingAccount = checkExistingAccount;
        _builder = builder;
        _writer = writer;
    }

    public Task<TilakaCheckExistingAccountResponse> Handle(TilakaCheckExistingAccountCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        Guard.Argument(request).NotNull()
            .Member(x => x.Email, y => y.NotEmpty());
        
        // BUILD
        var uuid = _generateUuid.Execute();
        if (!uuid.Success)
            throw new ArgumentException(uuid.Message);
        
        var aggregate = _builder
            .Load(request.Email)
            .Build();

        var req = new CheckExistingAccountRequest(uuid.RegistrationId, aggregate.NomorIdentitas);
        var checkExistingAccount = _checkExistingAccount.Execute(req);

        aggregate = _builder
            .Attach(aggregate)
            .TilakaId(checkExistingAccount.TilakaId)
            .Build();
        
        // WRITE
        _ = _writer.Save(aggregate);
        var response = new TilakaCheckExistingAccountResponse(
            aggregate.NomorIdentitas, 
            aggregate.Email,
            aggregate.TilakaName,
            checkExistingAccount.TilakaId,
            checkExistingAccount.Message, 
            checkExistingAccount.Status
        );
        return Task.FromResult(response);
    }
}

public class TilakaCheckExistingAccountHandlerTest
{
    private readonly Mock<IGenerateUuidTilakaService> _generateUuid;
    private readonly Mock<ICheckExistingAccountService> _checkExistingAccount;
    private readonly Mock<ITilakaUserBuilder> _builder;
    private readonly Mock<ITilakaUserWriter> _writer;
    private readonly TilakaCheckExistingAccountHandler _sut;

    public TilakaCheckExistingAccountHandlerTest()
    {
        _generateUuid = new Mock<IGenerateUuidTilakaService>();
        _checkExistingAccount = new Mock<ICheckExistingAccountService>();
        _builder = new Mock<ITilakaUserBuilder>();
        _writer = new Mock<ITilakaUserWriter>();
        _sut = new TilakaCheckExistingAccountHandler(_generateUuid.Object,_checkExistingAccount.Object, _builder.Object, _writer.Object);
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
    public async Task GivenAccountIsExist_ThenReturnExpected()
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