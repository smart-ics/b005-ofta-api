using FluentValidation;
using MediatR;
using Ofta.Application.TImelineContext.PostAgg.Workers;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.UseCases;

public record UpdateMsgPostCommand(string PostId, string Msg)
    : IRequest, IPostKey;

public class UpdateMsgPostHandler : IRequestHandler<UpdateMsgPostCommand>
{
    private readonly IPostBuilder _builder;
    private readonly IPostWriter _writer;
    private readonly IValidator<UpdateMsgPostCommand> _guard;

    public UpdateMsgPostHandler(IPostBuilder builder, 
        IPostWriter writer, 
        IValidator<UpdateMsgPostCommand> guard)
    {
        _builder = builder;
        _writer = writer;
        _guard = guard;
    }

    public Task<Unit> Handle(UpdateMsgPostCommand request, CancellationToken cancellationToken)
    {
        //  GUARD
        var guardResult = _guard.Validate(request);
        if (!guardResult.IsValid)
            throw new ValidationException(guardResult.Errors);
        
        //  BUILD
        var agg = _builder
            .Load(request)
            .Msg(request.Msg)
            .Build();
        
        //  WRITE
        _ = _writer.Save(agg);
        return Task.FromResult(Unit.Value);
    }
}

public class UpdateMsgPostGuard : AbstractValidator<UpdateMsgPostCommand>
{
    public UpdateMsgPostGuard()
    {
        RuleFor(x => x.Msg).NotEmpty();
        RuleFor(x => x.PostId).NotEmpty();
    }
}