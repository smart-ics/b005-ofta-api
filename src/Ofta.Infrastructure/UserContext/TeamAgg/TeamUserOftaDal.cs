using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.UserContext.TeamAgg;

public class TeamUserOftaDal : ITeamUserOftaDal
{
    private readonly DatabaseOptions _opt;

    public TeamUserOftaDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<TeamUserOftaModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("TeamId", "TeamId");
        bcp.AddMap("UserOftaId", "UserOftaId");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_TeamUserOfta";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(ITeamKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_TeamUserOfta
            WHERE
                TeamId = @TeamId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@TeamId", key.TeamId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<TeamUserOftaModel> ListData(ITeamKey filter)
    {
        const string sql = @"
            SELECT
                aa.TeamId, aa.UserOftaId, 
                ISNULL(bb.UserOftaName,'') UserOftaName
            FROM
                OFTA_TeamUserOfta aa
            LEFT JOIN
                OFTA_UserOfta bb on aa.UserOftaId = bb.UserOftaId
            WHERE
                aa.TeamId = @TeamId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@TeamId", filter.TeamId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<TeamUserOftaModel>(sql, dp);
    }

    public IEnumerable<TeamUserOftaModel> ListData(string filter)
    {
        const string sql = @"
            SELECT
                aa.TeamId, aa.UserOftaId, 
                ISNULL(bb.UserOftaName,'') UserOftaName
            FROM
                OFTA_TeamUserOfta aa
            LEFT JOIN
                OFTA_UserOfta bb on aa.UserOftaId = bb.UserOftaId
            WHERE
                aa.UserOftaId = @UserOftaId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", filter, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<TeamUserOftaModel>(sql, dp);
    }
}