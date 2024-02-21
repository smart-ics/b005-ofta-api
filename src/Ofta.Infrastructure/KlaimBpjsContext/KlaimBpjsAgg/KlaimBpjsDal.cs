using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDal : IKlaimBpjsDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(KlaimBpjsModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_KlaimBpjs(
                KlaimBpjsId, KlaimBpjsDate, UserOftaId, BundleState,
                RegId, PasienId, PasienName, NoSep,
                LayananName, DokterName, RajalRanap)
            VALUES(
                @KlaimBpjsId, @KlaimBpjsDate, @UserOftaId, @BundleState,
                @RegId, @PasienId, @PasienName, @NoSep,
                @LayananName, @DokterName, @RajalRanap)";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", model.KlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@KlaimBpjsDate", model.KlaimBpjsDate, SqlDbType.DateTime);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@BundleState", model.BundleState, SqlDbType.Int);
        dp.AddParam("@RegId", model.RegId, SqlDbType.VarChar);
        dp.AddParam("@PasienId", model.PasienId, SqlDbType.VarChar);
        dp.AddParam("@PasienName", model.PasienName, SqlDbType.VarChar);
        dp.AddParam("@NoSep", model.NoSep, SqlDbType.VarChar);
        dp.AddParam("@LayananName", model.LayananName, SqlDbType.VarChar);
        dp.AddParam("@DokterName", model.DokterName, SqlDbType.VarChar);
        dp.AddParam("@RajalRanap", model.RajalRanap, SqlDbType.Int);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(KlaimBpjsModel model)
    {
        const string sql = @"
            UPDATE
                OFTA_KlaimBpjs
            SET
                KlaimBpjsDate = @KlaimBpjsDate,
                UserOftaId = @UserOftaId,
                BundleState = @BundleState,
                RegId = @RegId,
                PasienId = @PasienId,
                PasienName = @PasienName,
                NoSep = @NoSep,
                LayananName = @LayananName,
                DokterName = @DokterName,
                RajalRanap = @RajalRanap
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", model.KlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@KlaimBpjsDate", model.KlaimBpjsDate, SqlDbType.DateTime);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
        dp.AddParam("@BundleState", model.BundleState, SqlDbType.Int);
        dp.AddParam("@RegId", model.RegId, SqlDbType.VarChar);
        dp.AddParam("@PasienId", model.PasienId, SqlDbType.VarChar);
        dp.AddParam("@PasienName", model.PasienName, SqlDbType.VarChar);
        dp.AddParam("@NoSep", model.NoSep, SqlDbType.VarChar);
        dp.AddParam("@LayananName", model.LayananName, SqlDbType.VarChar);
        dp.AddParam("@DokterName", model.DokterName, SqlDbType.VarChar);
        dp.AddParam("@RajalRanap", model.RajalRanap, SqlDbType.Int);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjs
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
    }

    public KlaimBpjsModel GetData(IKlaimBpjsKey key)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsDate, UserOftaId, BundleState,
                RegId, PasienId, PasienName, NoSep,
                LayananName, DokterName, RajalRanap
            FROM
                OFTA_KlaimBpjs
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.QueryFirstOrDefault<KlaimBpjsModel>(sql, dp);
    }

    public IEnumerable<KlaimBpjsModel> ListData(Periode filter)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsDate, UserOftaId, BundleState,
                RegId, PasienId, PasienName, NoSep,
                LayananName, DokterName, RajalRanap
            FROM
                OFTA_KlaimBpjs
            WHERE
                KlaimBpjsDate BETWEEN @StartDate AND @EndDate";
        
        var dp = new DynamicParameters();
        dp.AddParam("@StartDate", filter.Tgl1, SqlDbType.DateTime);
        dp.AddParam("@EndDate", filter.Tgl2, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<KlaimBpjsModel>(sql, dp);
    }
}