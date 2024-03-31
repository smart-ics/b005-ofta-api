using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.TimelineContext.CommentAgg;

public class CommentDal : ICommentDal
{
    private readonly DatabaseOptions _opt;

    public CommentDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(CommentModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_Comment (
                CommentId, CommentDate, PostId, 
                UserOftaId, Msg, ReactCount) 
            VALUES(
                @CommentId, @CommentDate, @PostId, 
                @UserOftaId, @Msg, @ReactCount)";

        var dp = new DynamicParameters();
        dp.AddParam("@CommentId", model.CommentId, SqlDbType.VarChar); 
        dp.AddParam("@CommentDate", model.CommentDate, SqlDbType.DateTime);
        dp.AddParam("@PostId", model.PostId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Msg", model.Msg, SqlDbType.VarChar);
        dp.AddParam("@ReactCount", model.ReactCount, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(CommentModel model)
    {
        const string sql = @"
            UPDATE
                OFTA_Comment
            SET
                CommentDate = @CommentDate, 
                PostId = @PostId,
                UserOftaId = @UserOftaId,
                Msg = @Msg, 
                ReactCount = @ReactCount 
            WHERE
                CommentId = @CommentId ";

        var dp = new DynamicParameters();
        dp.AddParam("@CommentId", model.CommentId, SqlDbType.VarChar); 
        dp.AddParam("@CommentDate", model.CommentDate, SqlDbType.DateTime);
        dp.AddParam("@PostId", model.PostId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Msg", model.Msg, SqlDbType.VarChar);
        dp.AddParam("@ReactCount", model.ReactCount, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(ICommentKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_Comment
            WHERE
                CommentId = @CommentId ";

        var dp = new DynamicParameters();
        dp.AddParam("@CommentId", key.CommentId, SqlDbType.VarChar); 

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public CommentModel GetData(ICommentKey key)
    {
        const string sql = @"
            SELECT
                CommentId, CommentDate, PostId, 
                UserOftaId, Msg, ReactCount 
            FROM
                OFTA_Comment
            WHERE
                CommentId = @CommentId ";

        var dp = new DynamicParameters();
        dp.AddParam("@CommentId", key.CommentId, SqlDbType.VarChar); 

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<CommentModel>(sql, dp);
    }

    public IEnumerable<CommentModel> ListData(IPostKey post)
    {
        const string sql = @"
            SELECT
                CommentId, CommentDate, PostId, 
                UserOftaId, Msg, ReactCount 
            FROM
                OFTA_Comment
            WHERE
                PostId = @PostId";

        var dp = new DynamicParameters();
        dp.AddParam("@PostId", post.PostId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<CommentModel>(sql, dp);
    }
}