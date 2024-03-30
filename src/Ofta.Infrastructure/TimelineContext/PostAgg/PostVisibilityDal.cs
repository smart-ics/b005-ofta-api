using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.TimelineContext.PostAgg;

public class PostVisibilityDal : IPostVisibilityDal
{
    private readonly DatabaseOptions _opt;

    public PostVisibilityDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<PostVisibilityModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("PostId", "PostId");
        bcp.AddMap("VisibilityReff", "VisibilityReff");
        

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_PostVisibility";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IPostKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_PostVisibility
            WHERE
                PostId = @PostId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@PostId", key.PostId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<PostVisibilityModel> ListData(IPostKey filter)
    {
        const string sql = @"
            SELECT
                aa.PostId, aa.VisibilityReff
            FROM
                OFTA_PostVisibility aa
            WHERE
                aa.PostId = @PostId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@PostId", filter.PostId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<PostVisibilityModel>(sql, dp);
    }
}