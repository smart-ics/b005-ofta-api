using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
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
                aa.PostId, aa.PostDate, aa.UserOftaId,
                aa.Msg, aa.DocId, aa.CommentCount, aa.LikeCount,
                ISNULL(bb.UserOftaName, '') AS UserOftaName
            FROM
                OFTA_Post aa
                LEFT JOIN OFTA_UserOfta bb ON aa.UserOftaId = bb.UserOftaId
            WHERE
                PostId = @PostId ";

        var dp = new DynamicParameters();
        dp.AddParam("@PostId", key.PostId, SqlDbType.VarChar); 

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<PostModel>(sql, dp);
    }

    public IEnumerable<PostModel> ListData(IUserOftaKey filter1, int pageNo)
    {
        const string sql = @"
            SELECT
                aa.PostId, aa.PostDate, aa.UserOftaId,
                aa.Msg, aa.DocId, aa.CommentCount, aa.LikeCount,
                ISNULL(cc.UserOftaName, '') AS UserOftaName
            FROM
                OFTA_Post aa
                LEFT JOIN OFTA_PostVisibility bb ON aa.PostId = bb.PostId
                LEFT JOIN OFTA_UserOfta cc on bb.VisibilityReff = cc.UserOftaId
            WHERE
                bb.VisibilityReff = @userOftaId
            ORDER BY
                aa.PostId DESC
            OFFSET 
                (@pageNumber - 1) * 50 ROWS
                FETCH NEXT 50 ROWS ONLY";

        var dp = new DynamicParameters();
        dp.AddParam("@userOftaId", filter1.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@pageNumber", pageNo, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<PostModel>(sql, dp);
    }



}