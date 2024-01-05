using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.UserContext;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.UserContext;

public class UserDal : IUserDal
{
    private readonly DatabaseOptions _opt;

    public UserDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(UserModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_User(
                UserId, UserName, Email, 
                IsVerified, VerifiedDate, ExpiredDate)
            VALUES (
                @UserId, @UserName, @Email, 
                @IsVerified, @VerifiedDate, @ExpiredDate)";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserId", model.UserId, SqlDbType.VarChar);
        dp.AddParam("@UserName", model.UserName, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@IsVerified", model.IsVerified, SqlDbType.Bit);
        dp.AddParam("@VerifiedDate", model.VerifiedDate, SqlDbType.DateTime);
        dp.AddParam("@ExpiredDate", model.ExpiredDate, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(UserModel model)
    {
        const string sql = @"
            UPDATE
                OFTA_User
            SET 
                UserName = @UserName, 
                Email = @Email, 
                IsVerified = @IsVerified, 
                VerifiedDate = @VerifiedDate, 
                ExpiredDate = @ExpiredDate
            WHERE
                UserId = @UserId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserId", model.UserId, SqlDbType.VarChar);
        dp.AddParam("@UserName", model.UserName, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@IsVerified", model.IsVerified, SqlDbType.Bit);
        dp.AddParam("@VerifiedDate", model.VerifiedDate, SqlDbType.DateTime);
        dp.AddParam("@ExpiredDate", model.ExpiredDate, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IUserKey key)
    {
        const string sql = @"
            UPDATE
                OFTA_User
            WHERE
                UserId = @UserId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserId", key.UserId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public UserModel GetData(IUserKey key)
    {
        const string sql = @"
            SELECT
                UserId, UserName, Email, 
                IsVerified, VerifiedDate, ExpiredDate
            FROM
                OFTA_User
            WHERE
                UserId = @UserId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserId", key.UserId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<UserModel>(sql, dp);
    }
}