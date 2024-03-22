using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.PrintOutContext.RemoteCetakAgg;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Contracts;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.PrintOutContext.RemoteCetakAgg;

public class RemoteCetakDal : IRemoteCetakDal
{
    private readonly RemoteCetakOptions _opt;

    public RemoteCetakDal(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(RemoteCetakModel model)
    {
        const string sql = @"
            INSERT INTO ta_remote_cetak(
                fs_kd_trs, fs_jenis_dok, fd_tgl_send, fs_jam_send, 
                fs_remote_addr, fn_cetak, fd_tgl_cetak, fs_jam_cetak, 
                fs_json_data, CallbackDataOfta) 
            VALUES (
                @fs_kd_trs, @fs_jenis_dok, @fd_tgl_send, @fs_jam_send, 
                @fs_remote_addr, @fn_cetak, @fd_tgl_cetak, @fs_jam_cetak, 
                @fs_json_data, @CallbackDataOfta)";
        
        //  PARAM
        var dp = new DynamicParameters();
        dp.AddParam("@fs_kd_trs", model.KodeTrs, SqlDbType.VarChar);
        dp.AddParam("@fs_jenis_dok", model.JenisDoc, SqlDbType.VarChar);
        dp.AddParam("@fd_tgl_send", model.TglSend, SqlDbType.VarChar);
        dp.AddParam("@fs_jam_send", model.JamSend, SqlDbType.VarChar);
        dp.AddParam("@fs_remote_addr", model.RemoteAddr, SqlDbType.VarChar);
        dp.AddParam("@fn_cetak", model.PrintState, SqlDbType.Int);
        dp.AddParam("@fd_tgl_cetak", model.TglCetak, SqlDbType.VarChar);
        dp.AddParam("@fs_jam_cetak", model.JamCetak, SqlDbType.VarChar);
        dp.AddParam("@fs_json_data", model.JsonData, SqlDbType.VarChar);
        dp.AddParam("@CallbackDataOfta", model.CallbackDataOfta, SqlDbType.VarChar);
        
        //  EXEC
        using var con = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        con.Execute(sql, dp);
    }

    public void Update(RemoteCetakModel model)
    {
        const string sql = @"
            UPDATE
                ta_remote_cetak
            SET
                fs_jenis_dok = @fs_jenis_dok,
                fd_tgl_send = @fd_tgl_send,
                fs_jam_send = @fs_jam_send,
                fs_remote_addr = @fs_remote_addr,
                fn_cetak = @fn_cetak,
                fd_tgl_cetak = @fd_tgl_cetak,
                fs_jam_cetak = @fs_jam_cetak,
                fs_json_data = @fs_json_data,
                CallbackDataOfta = @CallbackDataOfta
            WHERE
                fs_kd_trs = @fs_kd_trs";
        
        //  PARAM
        var dp = new DynamicParameters();
        dp.AddParam("@fs_kd_trs", model.KodeTrs, SqlDbType.VarChar);
        dp.AddParam("@fs_jenis_dok", model.JenisDoc, SqlDbType.VarChar);
        dp.AddParam("@fd_tgl_send", model.TglSend, SqlDbType.VarChar);
        dp.AddParam("@fs_jam_send", model.JamSend, SqlDbType.VarChar);
        dp.AddParam("@fs_remote_addr", model.RemoteAddr, SqlDbType.VarChar);
        dp.AddParam("@fn_cetak", model.PrintState, SqlDbType.Int);
        dp.AddParam("@fd_tgl_cetak", model.TglCetak, SqlDbType.VarChar);
        dp.AddParam("@fs_jam_cetak", model.JamCetak, SqlDbType.VarChar);
        dp.AddParam("@fs_json_data", model.JsonData, SqlDbType.VarChar);
        dp.AddParam("@CallbackDataOfta", model.CallbackDataOfta, SqlDbType.VarChar);
        
        //  EXEC
        using var con = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        con.Execute(sql, dp);
    }

    public RemoteCetakModel GetData(IRemoteCetakKey key)
    {
        const string sql = @"
            SELECT
                fs_kd_trs AS KodeTrs, 
                fs_jenis_dok AS JenisDoc, 
                fd_tgl_send AS TglSend, 
                fs_jam_send AS JamSend, 
                fs_remote_addr AS RemoteAddr, 
                fn_cetak AS PrintState, 
                fd_tgl_cetak AS TglCetak, 
                fs_jam_cetak AS JamCetak, 
                fs_json_data AS JsonData,
                CallbackDataOfta
            FROM
                ta_remote_cetak
            WHERE
                fs_kd_trs = @fs_kd_trs";
        
        //  PARAM
        var dp = new DynamicParameters();
        dp.AddParam("@fs_kd_trs", key.KodeTrs, SqlDbType.VarChar);
        
        //  EXEC
        using var con = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return con.QueryFirstOrDefault<RemoteCetakModel>(sql, dp);
    }

    public void Delete(IRemoteCetakKey key)
    {
        const string sql = @"
            DELETE FROM
                ta_remote_cetak
            WHERE
                fs_kd_trs = @fs_kd_trs";
        
        //  PARAM
        var dp = new DynamicParameters();
        dp.AddParam("@fs_kd_trs", key.KodeTrs, SqlDbType.VarChar);
        
        //  EXEC
        using var con = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        con.Execute(sql, dp);
    }

    public IEnumerable<RemoteCetakModel> ListData(Periode filter)
    {
        const string sql = @"
            SELECT
                fs_kd_trs AS KodeTrs, 
                fs_jenis_dok AS JenisDoc, 
                fd_tgl_send AS TglSend, 
                fs_jam_send AS JamSend, 
                fs_remote_addr AS RemoteAddr, 
                fn_cetak AS PrintState, 
                fd_tgl_cetak AS TglCetak, 
                fs_jam_cetak AS JamCetak, 
                fs_json_data AS JsonData,
                CallbackDataOfta
            FROM
                ta_remote_cetak
            WHERE
                fd_tgl_send BETWEEN @tgl1 AND @tgl2";
        
        //  PARAM
        var dp = new DynamicParameters();
        dp.AddParam("@tgl1", filter.Tgl1.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), SqlDbType.VarChar);
        dp.AddParam("@tgl2", filter.Tgl2.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), SqlDbType.VarChar);
        
        //  EXEC
        using var con = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return con.Query<RemoteCetakModel>(sql, dp);
    }
}