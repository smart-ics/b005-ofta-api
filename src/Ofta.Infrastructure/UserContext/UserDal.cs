﻿using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.UserOftaContext;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.UserContext;

public class UserDal : IUserDal
{
    private readonly DatabaseOptions _opt;

    public UserDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(UserOftaModel oftaModel)
    {
        const string sql = @"
            INSERT INTO OFTA_UserOfta(
                UserOftaId, UserOftaName, Email, 
                IsVerified, VerifiedDate, ExpiredDate)
            VALUES (
                @UserId, @UserOftaName, @Email, 
                @IsVerified, @VerifiedDate, @ExpiredDate)";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserId", oftaModel.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaName", oftaModel.UserOftaName, SqlDbType.VarChar);
        dp.AddParam("@Email", oftaModel.Email, SqlDbType.VarChar);
        dp.AddParam("@IsVerified", oftaModel.IsVerified, SqlDbType.Bit);
        dp.AddParam("@VerifiedDate", oftaModel.VerifiedDate, SqlDbType.DateTime);
        dp.AddParam("@ExpiredDate", oftaModel.ExpiredDate, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(UserOftaModel oftaModel)
    {
        const string sql = @"
            UPDATE
                OFTA_UserOfta
            SET 
                UserOftaName = @UserOftaName, 
                Email = @Email, 
                IsVerified = @IsVerified, 
                VerifiedDate = @VerifiedDate, 
                ExpiredDate = @ExpiredDate
            WHERE
                UserOftaId = @UserOftaId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", oftaModel.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaName", oftaModel.UserOftaName, SqlDbType.VarChar);
        dp.AddParam("@Email", oftaModel.Email, SqlDbType.VarChar);
        dp.AddParam("@IsVerified", oftaModel.IsVerified, SqlDbType.Bit);
        dp.AddParam("@VerifiedDate", oftaModel.VerifiedDate, SqlDbType.DateTime);
        dp.AddParam("@ExpiredDate", oftaModel.ExpiredDate, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IUserOftaKey oftaKey)
    {
        const string sql = @"
            UPDATE
                OFTA_UserOfta
            WHERE
                UserOftaId = @UserOftaId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", oftaKey.UserOftaId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public UserOftaModel GetData(IUserOftaKey oftaKey)
    {
        const string sql = @"
            SELECT
                UserOftaId, UserOftaName, Email, 
                IsVerified, VerifiedDate, ExpiredDate
            FROM
                OFTA_UserOfta
            WHERE
                UserOftaId  = @UserOftaId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", oftaKey.UserOftaId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<UserOftaModel>(sql, dp);
    }
}