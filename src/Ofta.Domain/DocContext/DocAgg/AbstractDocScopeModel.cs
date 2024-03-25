using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.DocContext.DocAgg;

public abstract class AbstractDocScopeModel : IDocKey
{
    public string DocId { get; set; }
    public int ScopeType { get; set; }
}

public class DocScopeUserModel : AbstractDocScopeModel, IUserOftaKey
{
    public string UserOftaId { get; set; }
}

public class DocScopeTeamModel : AbstractDocScopeModel, ITeamKey
{
    public string TeamId { get;  set;}
}
