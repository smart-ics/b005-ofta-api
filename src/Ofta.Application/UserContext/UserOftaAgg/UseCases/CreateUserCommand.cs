using Dawn;
using MediatR;
using Ofta.Application.UserContext.UserOftaAgg.Workers;

namespace Ofta.Application.UserContext.UserOftaAgg.UseCases;

public record CreateUserCommand(string UserOftaName, string Email) :
    IRequest<CreatUserResponse>;

public class CreatUserResponse
{
    public string UserOftaId { get; set; }
}

public class CreateUserHanler : IRequestHandler<CreateUserCommand, CreatUserResponse>
{
    private readonly IUserBuilder _builder;
    private readonly IUserWriter _writer;

    public CreateUserHanler(IUserBuilder builder, 
        IUserWriter writer)
    {
        _builder = builder;
        _writer = writer;
    }

    public Task<CreatUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        Guard.Argument(() => request).NotNull()
            .Member(x => x.UserOftaName, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _builder
            .Create()
            .UserOftaName(request.UserOftaName)
            .Email(request.Email)
            .Build();

        aggregate =  _writer.Save(aggregate);
        var response = new CreatUserResponse
        {
            UserOftaId = aggregate.UserOftaId
        };
        return Task.FromResult(response);
    }
}