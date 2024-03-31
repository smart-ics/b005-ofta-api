using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record AddReactPostCommand(string PostId, string UserOftaId)
    : IRequest, IPostKey, IUserOftaKey;

public class AddReactPostHandler : IRequestHandler<AddReactPostCommand>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IValidator<AddReactPostCommand> _guard;

    public AddReactPostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IValidator<AddReactPostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(AddReactPostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .AddReact(request)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class AddReactPostGuard : AbstractValidator<AddReactPostCommand>
{
    public AddReactPostGuard()
    {
        RuleFor(x => x.PostId).NotEmpty();
        RuleFor(x => x.UserOftaId).NotEmpty();
    }
}