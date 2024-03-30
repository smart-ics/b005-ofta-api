using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.TimelineContext.PostAgg;

public class PostDal : IPostDal
{
    private readonly DatabaseOptions _opt;

    public PostDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(PostModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_Post (
                PostId, PostDate, UserOftaId,
                Msg, DocId, CommentCount, LikeCount) 
            VALUES(
                @PostId, @PostDate, @UserOftaId,
                @Msg, @DocId, @CommentCount, @LikeCount)";

        var dp = new DynamicParameters();
        dp.AddParam("@PostId", model.PostId, SqlDbType.VarChar); 
        dp.AddParam("@PostDate", model.PostDate, SqlDbType.DateTime); 
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Msg", model.Msg, SqlDbType.VarChar);
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar); 
        dp.AddParam("@CommentCount", model.CommentCount, SqlDbType.VarChar); 
        dp.AddParam("@LikeCount", model.LikeCount, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(PostModel model)
    {
        const string sql = @"
            UPDATE
                OFTA_Post
            SET
                PostDate = @PostDate, 
                UserOftaId = @UserOftaId,
                Msg = @Msg, 
                DocId = @DocId, 
                CommentCount = @CommentCount, 
                LikeCount = @LikeCount 
            WHERE
                PostId = @PostId ";

        var dp = new DynamicParameters();
        dp.AddParam("@PostId", model.PostId, SqlDbType.VarChar); 
        dp.AddParam("@PostDate", model.PostDate, SqlDbType.DateTime); 
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Msg", model.Msg, SqlDbType.VarChar);
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar); 
        dp.AddParam("@CommentCount", model.CommentCount, SqlDbType.VarChar); 
        dp.AddParam("@LikeCount", model.LikeCount, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IPostKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_Post
            WHERE
                PostId = @PostId ";

        var dp = new DynamicParameters();
        dp.AddParam("@PostId", key.PostId, SqlDbType.VarChar); 

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public PostModel GetData(IPostKey key)
    {
        const string sql = @"
            SELECT
                PostId, PostDate, UserOftaId,
                Msg, DocId, CommentCount, LikeCount 
            FROM
                OFTA_Post
            WHERE
                PostId = @PostId ";

        var dp = new DynamicParameters();
        dp.AddParam("@PostId", key.PostId, SqlDbType.VarChar); 

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<PostModel>(sql, dp);
    }

    public IEnumerable<PostModel> ListData(Periode filter1, IUserOftaKey filter2)
    {
        const string sql = @"
            SELECT
                PostId, PostDate, UserOftaId,
                Msg, DocId, CommentCount, LikeCount 
            FROM
                OFTA_Post
            WHERE
                PostDate BETWEEN @tgl1 AND @tgl2
                AND UserOftaId = @userOftaId ";

        var dp = new DynamicParameters();
        dp.AddParam("@tgl1", filter1.Tgl1, SqlDbType.DateTime); 
        dp.AddParam("@tgl2", filter1.Tgl2, SqlDbType.DateTime); 
        dp.AddParam("@userOftaId", filter2.UserOftaId, SqlDbType.VarChar); 

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<PostModel>(sql, dp);
    }
}