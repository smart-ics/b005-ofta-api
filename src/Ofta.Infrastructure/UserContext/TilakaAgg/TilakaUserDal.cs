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
                RegistrationId, UserOftaId, TilakaName, NomorIdentitas, 
                ExpiredDate, UserState, CertificateState, RevokeReason
            )
            VALUES (
                @RegistrationId, @UserOftaId, @TilakaName, @NomorIdentitas,
                @ExpiredDate, @UserState, @CertificateState, @RevokeReason
            )";

        var dp = new DynamicParameters();
        dp.AddParam("@RegistrationId", model.RegistrationId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
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
        const string sql = @"
            SELECT
                RegistrationId,
                UserOftaId,
                TilakaName,
                NomorIdentitas,
                ExpiredDate,
                UserState,
                CertificateState,
                RevokeReason
            FROM
                OFTA_TilakaUserRegistration
            WHERE 
                UserOftaId = @UserOftaId";

        var dp = new DynamicParameters();
        dp.AddParam("@UserOftaId", key.UserOftaId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<TilakaUserModel>(sql, dp);
    }
}