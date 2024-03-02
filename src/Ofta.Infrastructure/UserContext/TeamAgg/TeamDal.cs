using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.UserContext.TeamAgg;

public class TeamDal : ITeamDal
{
    private readonly DatabaseOptions _opt;

    public TeamDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(TeamModel model)
    {
        //  QUERY
        const string sql = @"
            INSERT INTO OFTA_Team (
                TeamId, TeamName)
            VALUES (
                @TeamId, @TeamName)";
        
        //  PARAMETER
        var dp = new DynamicParameters();
        dp.AddParam("TeamId", model.TeamId, SqlDbType.VarChar);
        dp.AddParam("TeamName", model.TeamName, SqlDbType.VarChar);
        
        //  EXECUTE
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(TeamModel model)
    {
        //  QUERY
        const string sql = @"
            UPDATE 
                OFTA_Team
            SET 
                TeamName = @TeamName
            WHERE 
                TeamId = @TeamId";
        
        //  PARAMETER
        var dp = new DynamicParameters();
        dp.AddParam("TeamId", model.TeamId, SqlDbType.VarChar);
        dp.AddParam("TeamName", model.TeamName, SqlDbType.VarChar);
        
        //  EXECUTE
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(ITeamKey key)
    {
        //  QUERY
        const string sql = @"
            DELETE FROM 
                OFTA_Team
            WHERE 
                TeamId = @TeamId";
        
        //  PARAMETER
        var dp = new DynamicParameters();
        dp.AddParam("TeamId", key.TeamId, SqlDbType.VarChar);
        
        //  EXECUTE
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public TeamModel GetData(ITeamKey key)
    {
        //  QUERY
        const string sql = @"
            SELECT 
                TeamId, TeamName
            FROM 
                OFTA_Team
            WHERE 
                TeamId = @TeamId";
        
        //  PARAMETER
        var dp = new DynamicParameters();
        dp.AddParam("TeamId", key.TeamId, SqlDbType.VarChar);
        
        //  EXECUTE
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.QueryFirstOrDefault<TeamModel>(sql, dp);
    }

    public IEnumerable<TeamModel> ListData()
    {
        //  QUERY
        const string sql = @"
            SELECT 
                TeamId, TeamName
            FROM 
                OFTA_Team";
        
        //  EXECUTE
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<TeamModel>(sql);
    }
}