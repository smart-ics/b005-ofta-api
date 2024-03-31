using Dawn;
using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record CreatePostCommand(string UserOftaId, string Msg)
    : IRequest<CreatePostResponse>, IUserOftaKey;

public record CreatePostResponse(string PostId);


public class CreatePostHandler : IRequestHandler<CreatePostCommand, CreatePostResponse>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IValidator<CreatePostCommand> _guard;

    public CreatePostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IValidator<CreatePostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<CreatePostResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Create()
            .User(request)
            .Msg(request.Msg)
            .AddVisibility(request.UserOftaId)
            .Build();
        
        //  WRITE
        agg = _writer.Save(agg);
        return Task.FromResult(new CreatePostResponse(agg.PostId));
    }
}

public class CreatePostGuard : AbstractValidator<CreatePostCommand>
{
    public CreatePostGuard()
    {
        RuleFor(x => x.Msg).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}