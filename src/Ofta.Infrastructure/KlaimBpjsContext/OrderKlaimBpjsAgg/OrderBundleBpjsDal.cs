using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.OrderKlaimBpjsAgg;

public class OrderKlaimBpjsDal : IOrderKlaimBpjsDal
{
    private readonly  DatabaseOptions _opt;

    public OrderKlaimBpjsDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(OrderKlaimBpjsModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_OrderKlaimBpjs (
                OrderKlaimBpjsId, OrderKlaimBpjsDate, KlaimBpjsId, 
                UserOftaId, RegId, PasienId, PasienName, NoSep,
                LayananName, DokterName, RajalRanap)
            VALUES(
                @OrderKlaimBpjsId, @OrderKlaimBpjsDate, @KlaimBpjsId, 
                @UserOftaId, @RegId, @PasienId, @PasienName, @NoSep,
                @LayananName, @DokterName, @RajalRanap)";
        
        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", model.OrderKlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@OrderKlaimBpjsDate", model.OrderKlaimBpjsDate, SqlDbType.DateTime);
        dp.AddParam("@KlaimBpjsId", model.KlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
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

    public void Update(OrderKlaimBpjsModel model)
    {
        const string sql = @"
            UPDATE 
                OFTA_OrderKlaimBpjs
            SET
                OrderKlaimBpjsDate = @OrderKlaimBpjsDate,
                KlaimBpjsId = @KlaimBpjsId,
                UserOftaId = @UserOftaId,
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
        dp.AddParam("@UserOftaId", model.UserOftaId, SqlDbType.VarChar);
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
                OFTA_OrderKlaimBpjs 
            WHERE 
                OrderKlaimBpjsId = @OrderKlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", key.OrderKlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public OrderKlaimBpjsModel GetData(IOrderKlaimBpjsKey key)
    {
        const string sql = @"
            SELECT 
                OrderKlaimBpjsId, OrderKlaimBpjsDate, KlaimBpjsId, 
                UserOftaId, RegId,  PasienId, PasienName, 
                NoSep, LayananName, DokterName, RajalRanap 
            FROM 
                OFTA_OrderKlaimBpjs 
            WHERE 
                OrderKlaimBpjsId = @OrderKlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@OrderKlaimBpjsId", key.OrderKlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<OrderKlaimBpjsModel>(sql, dp);
    }

    public IEnumerable<OrderKlaimBpjsModel> ListData(Periode filter)
    {
        const string sql = @"
            SELECT 
                OrderKlaimBpjsId, OrderKlaimBpjsDate, KlaimBpjsId, 
                UserOftaId, RegId,  PasienId, PasienName, 
                NoSep, LayananName, DokterName, RajalRanap 
            FROM 
                OFTA_OrderKlaimBpjs 
            WHERE 
                OrderKlaimBpjsDate BETWEEN @tgl1 AND @tgl2";
        
        var dp = new DynamicParameters();
        dp.AddParam("@tgl1", filter.Tgl1, SqlDbType.DateTime);
        dp.AddParam("@tgl2", filter.Tgl2, SqlDbType.DateTime);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<OrderKlaimBpjsModel>(sql, dp);
    }
}