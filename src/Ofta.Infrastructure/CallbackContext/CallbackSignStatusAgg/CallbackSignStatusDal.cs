using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.CallbackContext.CallbackSignStatusAgg;

public class CallbackSignStatusDal: ICallbackSignStatusDal
{
    private readonly DatabaseOptions _opt;

    public CallbackSignStatusDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(CallbackSignStatusModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_CallbackSignStatus (
                RequestId,
                UserOftaId,
                Email,
                TilakaName,
                CallbackDate,
                JsonPayload
            )
            VALUES (
                @RequestId,
                @UserOftaId,
                @Email,
                @TilakaName,
                @CallbackDate,
                @JsonPayload
            )";

        var dp = new DynamicParameters();
        dp.AddParam("@RequestId", model.RequestId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@CallbackDate", model.CallbackDate.ToString(DateFormatEnum.YMD_HMS), SqlDbType.VarChar);
        dp.AddParam("@JsonPayload", model.JsonPayload, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(CallbackSignStatusModel model)
    {
        const string sql = @"
            UPDATE OFTA_CallbackSignStatus
            SET UserOftaId = @UserOftaId,
                Email = @Email,
                CallbackDate = @CallbackDate,
                JsonPayload = @JsonPayload
            WHERE
                RequestId = @RequestId
            AND
                TilakaName = @TilakaName";
        
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@Email", model.Email, SqlDbType.VarChar);
        dp.AddParam("@CallbackDate", model.CallbackDate.ToString(DateFormatEnum.YMD_HMS), SqlDbType.VarChar);
        dp.AddParam("@RequestId", model.RequestId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@JsonPayload", model.JsonPayload, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public CallbackSignStatusModel GetData(ICallbackSignStatusKey key)
    {
        const string sql = @"
            SELECT
                RequestId,
                UserOftaId,
                Email,
                TilakaName,
                CallbackDate,
                JsonPayload
            FROM
                OFTA_CallbackSignStatus
            WHERE
                RequestId = @RequestId
            AND
                TilakaName = @TilakaName";
        
        var dp = new DynamicParameters();
        dp.AddParam("@RequestId", key.RequestId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", key.TilakaName, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<CallbackSignStatusModel>(sql, dp);
    }
}

public class CallbackSignStatusDalTest 
{
    private readonly CallbackSignStatusDal _sut;

    public CallbackSignStatusDalTest()
    {
        _sut = new CallbackSignStatusDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackSignStatusModel
        {
            RequestId = "A",
            UserOftaId = "B",
            Email = "C",
            TilakaName = "D",
            CallbackDate = DateTime.Now
        };

        // ACT & ASSERT
        _sut.Insert(expected);
    }
    
    [Fact]
    public void UpdateTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackSignStatusModel
        {
            RequestId = "A",
            UserOftaId = "B",
            Email = "C",
            TilakaName = "D",
            CallbackDate = DateTime.Now
        };

        // ACT & ASSERT
        _sut.Update(expected);
    }
    
    [Fact]
    public void GetDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackSignStatusModel
        {
            RequestId = "A",
            UserOftaId = "B",
            Email = "C",
            TilakaName = "D",
            CallbackDate = new DateTime(2024, 11, 23, 08, 00, 00)
        };
        _sut.Insert(expected);

        // ACT
        var actual = _sut.GetData(expected);
        
        // ASSERT
        actual.Should().BeEquivalentTo(expected);
    }
}