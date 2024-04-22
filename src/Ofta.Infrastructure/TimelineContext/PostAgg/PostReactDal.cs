using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.TeamAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.TimelineContext.PostAgg;

public class PostReactDal : IPostReactDal
{
    private readonly DatabaseOptions _opt;

    public PostReactDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<PostReactModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("PostId", "PostId");
        bcp.AddMap("PostReactDate", "PostReactDate");
        bcp.AddMap("UserOftaId", "UserOftaId");
        

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_PostReact";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IPostKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_PostReact
            WHERE
                PostId = @PostId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@PostId", key.PostId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<PostReactModel> ListData(IPostKey filter)
    {
        const string sql = @"
            SELECT
                aa.PostId, aa.PostReactDate, aa.UserOftaId, isnull(bb.UserOftaName,'??') UserOftaName
            FROM
                OFTA_PostReact aa
            LEFT JOIN 
                OFTA_UserOfta bb on aa.UserOftaId = bb.UserOftaId
            WHERE
                aa.PostId = @PostId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@PostId", filter.PostId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<PostReactModel>(sql, dp);
    }

    public IEnumerable<PostReactModel> ListData()
    {
        const string sql = @"
            SELECT
                aa.PostId, aa.PostReactDate, aa.UserOftaId, isnull(bb.UserOftaName,'??') UserOftaName
            FROM
                OFTA_PostReact aa
            LEFT JOIN 
                OFTA_UserOfta bb on aa.UserOftaId = bb.UserOftaId";

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<PostReactModel>(sql);
    }
}