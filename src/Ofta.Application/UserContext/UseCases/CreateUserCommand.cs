using Dawn;
using MediatR;
using Ofta.Application.UserContext.Workers;

namespace Ofta.Application.UserContext.UseCases;

public record CreateUserCommand(string UserName, string Email) :
    IRequest<CreatUserResponse>;

public class CreatUserResponse
{
    public string UserId { get; set; }
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
            .Member(x => x.UserName, y => y.NotEmpty())
            .Member(x => x.Email, y => y.NotEmpty());
        
        //  BUILD
        var aggregate = _builder
            .Create()
            .UserName(request.UserName)
            .Email(request.Email)
            .Build();

        aggregate =  _writer.Save(aggregate);
        var response = new CreatUserResponse
        {
            UserId = aggregate.UserId
        };
        return Task.FromResult(response);
    }
}