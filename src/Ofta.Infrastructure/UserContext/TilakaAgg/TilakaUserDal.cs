using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg;

public class TilakaUserDal: ITilakaUserDal
{
    private readonly DatabaseOptions _opt;

    public TilakaUserDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(TilakaUserModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_TilakaUserRegistration(
                RegistrationId, UserOftaId, TilakaId, TilakaName, NomorIdentitas, 
                ExpiredDate, UserState, CertificateState, RevokeReason
            )
            VALUES (
                @RegistrationId, @UserOftaId, @TilakaId, @TilakaName, @NomorIdentitas,
                @ExpiredDate, @UserState, @CertificateState, @RevokeReason
            )";

        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@TilakaId", model.TilakaId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@NomorIdentitas", model.NomorIdentitas, SqlDbType.VarChar);
        dp.AddParam("@ExpiredDate", model.ExpiredDate, SqlDbType.DateTime);
        dp.AddParam("@UserState", model.UserState, SqlDbType.Int);
        dp.AddParam("@CertificateState", model.CertificateState, SqlDbType.Int);
        dp.AddParam("@RevokeReason", model.RevokeReason, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(TilakaUserModel model)
    {
        const string sql = @"
            UPDATE OFTA_TilakaUserRegistration
                SET
                    UserOftaId = @UserOftaId,
                    TilakaId = @TilakaId,
                    TilakaName = @TilakaName,
                    NomorIdentitas = @NomorIdentitas, 
                    ExpiredDate = @ExpiredDate,
                    UserState = @UserState,
                    CertificateState = @CertificateState,
                    RevokeReason = @RevokeReason
                WHERE 
                    RegistrationId = @RegistrationId";

        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@TilakaId", model.TilakaId, SqlDbType.VarChar);
        dp.AddParam("@TilakaName", model.TilakaName, SqlDbType.VarChar);
        dp.AddParam("@NomorIdentitas", model.NomorIdentitas, SqlDbType.VarChar);
        dp.AddParam("@ExpiredDate", model.ExpiredDate, SqlDbType.DateTime);
        dp.AddParam("@UserState", model.UserState, SqlDbType.Int);
        dp.AddParam("@CertificateState", model.CertificateState, SqlDbType.Int);
        dp.AddParam("@RevokeReason", model.RevokeReason, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public TilakaUserModel GetData(IUserOftaKey key)
    {
        var sql = $@"{SelectFromClause()} WHERE aa.UserOftaId = @UserOftaId";
        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", key.UserOftaId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<TilakaUserModel>(sql, dp);
    }

    public TilakaUserModel GetData(ITilakaRegistrationKey key)
    {
        var sql = $@"{SelectFromClause()} WHERE aa.RegistrationId = @RegistrationId";
        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", key.RegistrationId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<TilakaUserModel>(sql, dp);
    }

    public TilakaUserModel GetData(ITilakaNameKey key)
    {
        var sql = $@"{SelectFromClause()} WHERE aa.TilakaName = @TilakaName";
        var dp = new DynamicParameters();
        dp.AddParam("@TilakaName", key.TilakaName, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<TilakaUserModel>(sql, dp);
    }

    private static string SelectFromClause()
    {
        return @"SELECT
                aa.RegistrationId,
                aa.UserOftaId,
                aa.TilakaId,
                aa.TilakaName,
                aa.NomorIdentitas,
                aa.ExpiredDate,
                aa.UserState,
                aa.CertificateState,
                aa.RevokeReason,
                ISNULL(bb.UserOftaName, '') AS UserOftaName,
                ISNULL(bb.Email, '') AS Email
            FROM
                OFTA_TilakaUserRegistration aa
            LEFT JOIN
                OFTA_UserOfta bb ON aa.UserOftaId = bb.UserOftaId";
    }
}