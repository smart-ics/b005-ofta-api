using FluentValidation;
using MediatR;
using Ofta.Application.UserContext.UserOftaAgg.Workers;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.UserOftaAgg.UseCases;

public record UserOftaAddMappingCommand(
    string UserOftaId,
    string UserMappingId,
    string PegId,
    string UserType
) : IRequest, IUserOftaKey;

public class UserOftaAddMappingHandler : IRequestHandler<UserOftaAddMappingCommand>
{
    private readonly IValidator<UserOftaAddMappingCommand> _guard;
    private readonly IUserBuilder _builder;
    private readonly IUserWriter _writer;

    public UserOftaAddMappingHandler(IValidator<UserOftaAddMappingCommand> guard, IUserBuilder builder,
        IUserWriter writer)
    {
        _guard = guard;
        _builder = builder;
        _writer = writer;
    }

    public Task<Unit> Handle(UserOftaAddMappingCommand request, CancellationToken cancellationToken)
    {
        // GUARD
        var validationResult = _guard.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // BUILD
        var user = _builder
            .Load(request)
            .AddUserMapping(request.UserMappingId, request.PegId, request.UserType)
            .Build();

        // WRITE
        _ = _writer.Save(user);
        return Task.FromResult(Unit.Value);
    }
}

public class UserOftaAddMappingValidator : AbstractValidator<UserOftaAddMappingCommand>
{
    public UserOftaAddMappingValidator()
    {
        RuleFor(x => x.UserOftaId).NotEmpty();
        RuleFor(x => x.UserMappingId).NotEmpty();
    }
}