using System.Data;
using System.Data.SqlClient;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.TransactionHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.CallbackContext.CallbackCertificateStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackCertificateStatusAgg;
using Ofta.Domain.CallbackContext.CallbackRegistrationAgg;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Infrastructure.Helpers;
using Xunit;

namespace Ofta.Infrastructure.CallbackContext.CallbackCertificateStatusAgg;

public class CallbackCertificateStatusDal: ICallbackCertificateStatusDal
{
    private readonly DatabaseOptions _opt;

    public CallbackCertificateStatusDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(CallbackCertificateStatusModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_CallbackCertificateStatus (
                RegistrationId,
                TilakaName,
                CertificateStatus,
                CallbackDate,
                JsonPayload
            )
            VALUES (
                @RegistrationId,
                @TilakaName,
                @CertificateStatus,
                @CallbackDate,
                @JsonPayload
            )";

        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@CertificateStatus", model.CertificateStatus, SqlDbType.VarChar);
        dp.AddParam("@CallbackDate", model.CallbackDate.ToString(DateFormatEnum.YMD_HMS), SqlDbType.VarChar);
        dp.AddParam("@JsonPayload", model.JsonPayload, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(CallbackCertificateStatusModel model)
    {
        const string sql = @"
            UPDATE OFTA_CallbackCertificateStatus
            SET TilakaName = @TilakaName,
                CertificateStatus = @CertificateStatus,
                CallbackDate = @CallbackDate,
                JsonPayload = @JsonPayload
            WHERE
                RegistrationId = @RegistrationId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@CertificateStatus", model.CertificateStatus, SqlDbType.VarChar);
        dp.AddParam("@CallbackDate", model.CallbackDate.ToString(DateFormatEnum.YMD_HMS), SqlDbType.VarChar);
        dp.AddParam("@JsonPayload", model.JsonPayload, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public CallbackCertificateStatusModel GetData(ITilakaRegistrationKey key)
    {
        const string sql = @"
            SELECT
                RegistrationId,
                TilakaName,
                CertificateStatus,
                CallbackDate,
                JsonPayload
            FROM
                OFTA_CallbackCertificateStatus
            WHERE
                RegistrationId = @RegistrationId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", key.RegistrationId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<CallbackCertificateStatusModel>(sql, dp);
    }
}

public class CallbackCertificateStatusDalTest 
{
    private readonly CallbackCertificateStatusDal _sut;

    public CallbackCertificateStatusDalTest()
    {
        _sut = new CallbackCertificateStatusDal(ConnStringHelper.GetTestEnv());
    }

    [Fact]
    public void InsertTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackCertificateStatusModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            CertificateStatus = "C",
            CallbackDate = DateTime.Now,
            JsonPayload = "D"
        };

        // ACT & ASSERT
        _sut.Insert(expected);
    }
    
    [Fact]
    public void UpdateTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackCertificateStatusModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            CertificateStatus = "C",
            CallbackDate = DateTime.Now,
            JsonPayload = "D"
        };

        // ACT & ASSERT
        _sut.Update(expected);
    }
    
    [Fact]
    public void GetDataTest()
    {
        // ARRANGE
        using var trans = TransHelper.NewScope();
        var expected = new CallbackCertificateStatusModel
        {
            RegistrationId = "A",
            TilakaName = "B",
            CertificateStatus = "C",
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