namespace Ofta.Domain.UserContext.TilakaAgg;

public enum TilakaUserState
{
    Created,
    ManualRegistration,
    Verified,
    RevokeRequested,
    Revoked
}