using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.CallbackContext.CallbackRegistrationAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackRegistrationAgg;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.CallbackContext.CallbackRegistrationAgg;

public class CallbackRegistrationDal: ICallbackRegistrationDal
{
    private readonly DatabaseOptions _opt;

    public CallbackRegistrationDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(CallbackRegistrationModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_CallbackRegistration (
                RegistrationId,
                TilakaName,
                RegistrationStatus,
                RegistrationReasonCode,
                ManualRegistrationStatus,
                CallbackDate,
                JsonPayload
            )
            VALUES (
                @RegistrationId,
                @TilakaName,
                @RegistrationStatus,
                @RegistrationReasonCode,
                @ManualRegistrationStatus,
                @CallbackDate,
                @JsonPayload
            )";

        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@RegistrationStatus", model.RegistrationStatus, SqlDbType.VarChar);
        dp.AddParam("@RegistrationReasonCode", model.RegistrationReasonCode, SqlDbType.VarChar);
        dp.AddParam("@ManualRegistrationStatus", model.ManualRegistrationStatus, SqlDbType.VarChar);
        dp.AddParam("@CallbackDate", model.CallbackDate.ToString(DateFormatEnum.YMD_HMS), SqlDbType.VarChar);
        dp.AddParam("@JsonPayload", model.JsonPayload, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(CallbackRegistrationModel model)
    {
        const string sql = @"
            UPDATE OFTA_CallbackRegistration
            SET TilakaName = @TilakaName,
                RegistrationStatus = @RegistrationStatus,
                RegistrationReasonCode = @RegistrationReasonCode,
                ManualRegistrationStatus = @ManualRegistrationStatus,
                CallbackDate = @CallbackDate,
                JsonPayload = @JsonPayload
            WHERE
                RegistrationId = @RegistrationId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@RegistrationStatus", model.RegistrationStatus, SqlDbType.VarChar);
        dp.AddParam("@RegistrationReasonCode", model.RegistrationReasonCode, SqlDbType.VarChar);
        dp.AddParam("@ManualRegistrationStatus", model.ManualRegistrationStatus, SqlDbType.VarChar);
        dp.AddParam("@CallbackDate", model.CallbackDate.ToString(DateFormatEnum.YMD_HMS), SqlDbType.VarChar);
        dp.AddParam("@JsonPayload", model.JsonPayload, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public CallbackRegistrationModel GetData(ITilakaRegistrationKey key)
    {
        const string sql = @"
            SELECT
                RegistrationId,
                TilakaName,
                RegistrationStatus,
                RegistrationReasonCode,
                ManualRegistrationStatus,
                CallbackDate,
                JsonPayload
            FROM
                OFTA_CallbackRegistration
            WHERE
                RegistrationId = @RegistrationId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", key.RegistrationId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<CallbackRegistrationModel>(sql, dp);
    }
}

public class CallbackRegistrationDalTest 
{
    private readonly CallbackRegistrationDal _sut;

    public CallbackRegistrationDalTest()
    {
        _sut = new CallbackRegistrationDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackRegistrationModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            RegistrationStatus = "C",
            RegistrationReasonCode = "D",
            ManualRegistrationStatus = "E",
            CallbackDate = DateTime.Now,
            JsonPayload = "F"
        };

        // ACT & ASSERT
        _sut.Insert(expected);
    }
    
    [Fact]
    public void UpdateTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackRegistrationModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            RegistrationStatus = "C",
            RegistrationReasonCode = "D",
            ManualRegistrationStatus = "E",
            CallbackDate = DateTime.Now,
            JsonPayload = "F"
        };

        // ACT & ASSERT
        _sut.Update(expected);
    }
    
    [Fact]
    public void GetDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackRegistrationModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            RegistrationStatus = "C",
            RegistrationReasonCode = "D",
            ManualRegistrationStatus = "E",
            CallbackDate = new DateTime(2024, 11, 23, 08, 00, 00),
            JsonPayload = "F"
        };
        _sut.Insert(expected);

        // ACT
        var actual = _sut.GetData(expected);
        
        // ASSERT
        actual.Should().BeEquivalentTo(expected);
    }
}