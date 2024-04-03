using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.UserContext.TeamAgg;

namespace Ofta.Application.UserContext.TeamAgg.Contracts;

public interface ITeamUserOftaDal :
    IInsertBulk<TeamUserOftaModel>,
    IDelete<ITeamKey>,
    IListData<TeamUserOftaModel, ITeamKey>,
    IListData<TeamUserOftaModel, string>
{
}