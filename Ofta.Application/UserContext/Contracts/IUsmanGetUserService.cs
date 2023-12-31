using Nuna.Lib.CleanArchHelper;
// ReSharper disable InconsistentNaming

namespace Ners.Application.BillingContext.UserAgg.Contracts;

public interface IUsmanGetUserService : INunaService<UsmanGetUserResponse, UsmanGetUserDto>
{
}

public class UsmanGetUserDto
{
    public string Email { get; set; }
    public string Pass { get; set; }
}

public class UsmanGetUserResponse
{
    public string pegId { get; set; }
    public string userName { get; set; }
    public string email { get; set; }
    public string expiredDate { get; set; }
}