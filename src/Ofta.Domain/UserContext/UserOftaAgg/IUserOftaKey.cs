namespace Ofta.Domain.UserContext.UserOftaAgg;

public interface IScope
{
}

public interface IUserOftaKey : IScope
{
    string UserOftaId { get; }
}