using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;
using Ofta.Infrastructure.Helpers;


namespace Ofta.Infrastructure.KlaimBpjsContext.WorkListBpjsAgg;

public class WorkListBpjsDal : IWorkListBpjsDal
{
    private readonly DatabaseOptions _opt;

    public WorkListBpjsDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(WorkListBpjsModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_WorkListBpjs (
                OrderKlaimBpjsId, OrderKlaimBpjsDate, KlaimBpjsId, 
                WorkState, RegId, PasienId, PasienName, NoSep,
                LayananName, DokterName, RajalRanap)
            VALUES(
                @OrderKlaimBpjsId, @OrderKlaimBpjsDate, @KlaimBpjsId, 
                @WorkState, @RegId, @PasienId, @PasienName, @NoSep,
                @LayananName, @DokterName, @RajalRanap)";

        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", model.OrderKlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@OrderKlaimBpjsDate", model.OrderKlaimBpjsDate, SqlDbType.DateTime);
        dp.AddParam("@KlaimBpjsId", model.KlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@WorkState", model.WorkState, SqlDbType.VarChar);
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

    public void Update(WorkListBpjsModel model)
    {
        const string sql = @"
            UPDATE 
                OFTA_WorkListBpjs
            SET
                OrderKlaimBpjsDate = @OrderKlaimBpjsDate,
                KlaimBpjsId = @KlaimBpjsId,
                WorkState = @WorkState,
                RegId = @RegId,
                PasienId = @PasienId,
                PasienName = @PasienName,
                NoSep = @NoSep,
                LayananName = @LayananName,
                DokterName = @DokterName,
                RajalRanap = @RajalRanap
            WHERE
                OrderKlaimBpjsId = @OrderKlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", model.OrderKlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@OrderKlaimBpjsDate", model.OrderKlaimBpjsDate, SqlDbType.DateTime);
        dp.AddParam("@KlaimBpjsId", model.KlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@WorkState", model.WorkState, SqlDbType.VarChar);
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

    public void Delete(IOrderKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM 
                OFTA_WorkListBpjs 
            WHERE 
                OrderKlaimBpjsId = @OrderKlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", key.OrderKlaimBpjsId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public WorkListBpjsModel GetData(IOrderKlaimBpjsKey key)
    {
        const string sql = @"
            SELECT 
                OrderKlaimBpjsId, OrderKlaimBpjsDate, KlaimBpjsId, 
                WorkState, RegId,  PasienId, PasienName, 
                NoSep, LayananName, DokterName, RajalRanap 
            FROM 
                OFTA_WorkListBpjs 
            WHERE 
                OrderKlaimBpjsId = @OrderKlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", key.OrderKlaimBpjsId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<WorkListBpjsModel>(sql, dp);
    }

    public IEnumerable<WorkListBpjsModel> ListData()
    {
        const string sql = @"
            SELECT 
                OrderKlaimBpjsId, OrderKlaimBpjsDate, KlaimBpjsId, 
                UserOftaId, RegId,  PasienId, PasienName, 
                NoSep, LayananName, DokterName, RajalRanap 
            FROM 
                OFTA_WorkListBpjs";


        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<WorkListBpjsModel>(sql);
    }
}