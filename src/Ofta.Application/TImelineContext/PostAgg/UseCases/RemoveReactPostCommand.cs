using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record RemoveReactPostCommand(string PostId, string UserOftaId)
    : IRequest, IPostKey, IUserOftaKey;

public class RemoveReachPostHandler : IRequestHandler<RemoveReactPostCommand>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IValidator<RemoveReactPostCommand> _guard;

    public RemoveReachPostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IValidator<RemoveReactPostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(RemoveReactPostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .RemoveReact(new UserOftaModel(request.UserOftaId))
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class RemoveReactPostGuard : AbstractValidator<RemoveReactPostCommand>
{
    public RemoveReactPostGuard()
    {
        RuleFor(x => x.PostId).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}