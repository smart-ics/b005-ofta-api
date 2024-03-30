using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.TimelineContext.CommentAgg;

public class CommentReactDal : ICommentReactDal
{
    private readonly DatabaseOptions _opt;

    public CommentReactDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<CommentReactModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("CommentId", "CommentId");
        bcp.AddMap("CommentReactDate", "CommentReactDate");
        bcp.AddMap("UserOftaId", "UserOftaId");
        

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_CommentReact";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(ICommentKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_CommentReact
            WHERE
                CommentId = @CommentId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@CommentId", key.CommentId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<CommentReactModel> ListData(ICommentKey filter)
    {
        const string sql = @"
            SELECT
                aa.CommentId, aa.CommentReactDate, aa.UserOftaId
            FROM
                OFTA_CommentReact aa
            WHERE
                aa.CommentId = @CommentId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@CommentId", filter.CommentId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<CommentReactModel>(sql, dp);
    }
}