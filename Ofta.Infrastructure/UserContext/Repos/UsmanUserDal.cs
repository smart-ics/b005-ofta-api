using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Ners.Infrastructure.Helpers;
using Nuna.Lib.DataAccessHelper;
using Usman.Lib.NetStandard.Interfaces;
using Usman.Lib.NetStandard.Models;

namespace Ofta.Infrastructure.UserContext.Repos;

public class UsmanUserDal : IUsmanUserDal
{
    private readonly DatabaseOptions _opt;

    public UsmanUserDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }
    public void Insert(UsmanUserModel model)
    {
        const string sql = @"
            INSERT INTO
                USMAN_User ( 
                    UserId, UserName, CreationDate,
                    Email, Pass, IsApproved, ApprovalDate,
                    ExpiredDate, TokenLifeTime)
            VALUES (
                    @UserId, @UserName, @CreationDate,
                    @Email, @Pass, @IsApproved, @ApprovalDate,
                    @ExpiredDate, @TokenLifeTime) ";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", model.UserId, SqlDbType.UniqueIdentifier);
        dp.AddParam("@UserName", model.UserName, SqlDbType.VarChar);
        dp.AddParam("@CreationDate", model.CreationDate, SqlDbType.DateTime);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@Pass", model.Pass, SqlDbType.VarChar);
        dp.AddParam("@IsApproved", model.IsApproved, SqlDbType.Bit);
        dp.AddParam("@ApprovalDate", model.ApprovalDate, SqlDbType.DateTime);
        dp.AddParam("@ExpiredDate", model.ExpiredDate, SqlDbType.DateTime);
        dp.AddParam("@TokenLifeTime", model.TokenLifeTime, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(UsmanUserModel model)
    {
        const string sql = @"
            UPDATE
                USMAN_User 
            SET 
                UserName = @UserName,
                CreationDate = @CreationDate,
                Email = @Email,
                Pass = @Pass,
                IsApproved = @IsApproved, 
                ApprovalDate = @ApprovalDate,
                ExpiredDate = @ExpiredDate,
                TokenLifeTime = @TokenLifeTime
            WHERE
                 UserId = @UserId ";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", model.UserId, SqlDbType.UniqueIdentifier);
        dp.AddParam("@UserName", model.UserName, SqlDbType.VarChar);
        dp.AddParam("@CreationDate", model.CreationDate, SqlDbType.DateTime);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@Pass", model.Pass, SqlDbType.VarChar);
        dp.AddParam("@IsApproved", model.IsApproved, SqlDbType.Bit);
        dp.AddParam("@ApprovalDate", model.ApprovalDate, SqlDbType.DateTime);
        dp.AddParam("@ExpiredDate", model.ExpiredDate, SqlDbType.DateTime);
        dp.AddParam("@TokenLifeTime", model.TokenLifeTime, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IUserKey key)
    {
        const string sql = @"
            DELETE
                USMAN_User 
            WHERE
                 UserId = @UserId ";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", key.UserId, SqlDbType.UniqueIdentifier);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public UsmanUserModel GetData(IUserKey userId)
    {
        const string sql = @"
            SELECT
                UserId, UserName, CreationDate,
                Email, Pass, IsApproved, ApprovalDate,
                ExpiredDate, TokenLifeTime
            FROM
                USMAN_User 
            WHERE
                UserId = @UserId  ";

        var dp = new DynamicParameters();
        dp.AddParam("@UserId", userId.UserId, SqlDbType.UniqueIdentifier);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<UsmanUserModel>(sql, dp);
    }

    public UsmanUserModel GetData(IEmail email)
    {
        const string sql = @"
            SELECT
                UserId, UserName, CreationDate,
                Email, Pass, IsApproved, ApprovalDate,
                ExpiredDate, TokenLifeTime
            FROM
                USMAN_User 
            WHERE
                Email = @Email  ";

        var dp = new DynamicParameters();
        dp.AddParam("@Email", email.Email, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<UsmanUserModel>(sql, dp);
    }

    public IEnumerable<UsmanUserModel> ListData()
    {
        const string sql = @"
            SELECT
                UserId, UserName, CreationDate,
                Email, Pass, IsApproved, ApprovalDate,
                ExpiredDate, TokenLifeTime
            FROM
                USMAN_User ";

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<UsmanUserModel>(sql);
    }
}
