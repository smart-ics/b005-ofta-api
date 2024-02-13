using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.UserContext.TeamAgg;

namespace Ofta.Application.UserContext.TeamAgg.Contracts;

public interface ITeamDal :
    IInsert<TeamModel>,
    IUpdate<TeamModel>,
    IDelete<ITeamKey>,
    IGetData<TeamModel, ITeamKey>,
    IListData<TeamModel>
{
}