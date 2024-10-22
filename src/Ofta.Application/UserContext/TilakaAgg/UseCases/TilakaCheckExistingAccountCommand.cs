using Dawn;
using FluentAssertions;
using MediatR;
using Moq;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.UserContext.TilakaAgg;
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
    private readonly ICheckExistingAccountService _checkExistingAccount;
    private readonly ITilakaUserBuilder _builder;
    private readonly ITilakaUserWriter _writer;

    public TilakaCheckExistingAccountHandler(ICheckExistingAccountService checkExistingAccount, ITilakaUserBuilder builder, ITilakaUserWriter writer)
    {
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
        var aggregate = _builder
            .Load(request.Email)
            .Build();

        var req = new CheckExistingAccountRequest(aggregate.RegistrationId, aggregate.NomorIdentitas);
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
    private readonly Mock<ICheckExistingAccountService> _checkExistingAccount;
    private readonly Mock<ITilakaUserBuilder> _builder;
    private readonly Mock<ITilakaUserWriter> _writer;
    private readonly TilakaCheckExistingAccountHandler _sut;

    public TilakaCheckExistingAccountHandlerTest()
    {
        _checkExistingAccount = new Mock<ICheckExistingAccountService>();
        _builder = new Mock<ITilakaUserBuilder>();
        _writer = new Mock<ITilakaUserWriter>();
        _sut = new TilakaCheckExistingAccountHandler(_checkExistingAccount.Object, _builder.Object, _writer.Object);
    }
    
    [Fact]
    public async Task GivenAccountIsExist_ThenReturnExpected()
    {
        // ARRANGE
        var request = new TilakaCheckExistingAccountCommand("A");
        var tilakaUser = new TilakaUserModel
        {
            RegistrationId = "A",
            Email = request.Email,
            TilakaId = "D",
            TilakaName = "E"
        };
        _builder.Setup(x => x.Load(It.IsAny<string>()).Build())
            .Returns(tilakaUser);
        
        var expected = new CheckExistingAccountResponse(true, "A", "D");
        _checkExistingAccount.Setup(x => x.Execute(It.IsAny<CheckExistingAccountRequest>()))
            .Returns(expected);
        
        _builder.Setup(x => x.Attach(It.IsAny<TilakaUserModel>()).TilakaId(It.IsAny<string>()).Build())
            .Returns(tilakaUser);

        TilakaUserModel actual = null;
        _writer.Setup(x => x.Save(It.IsAny<TilakaUserModel>()))
            .Callback((TilakaUserModel k) => actual = k);
        
        // ACT
        await _sut.Handle(request, CancellationToken.None);
        
        // ASSERT
        actual?.TilakaId.Should().BeEquivalentTo("D");
    }
}