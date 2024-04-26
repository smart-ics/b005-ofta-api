using Dapper;
using Microsoft.Extensions.Options;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using System.Data.SqlClient;
using System.Data;
using Nuna.Lib.DataAccessHelper;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListPenjualanService : IListPenjualanService
{
    private readonly RemoteCetakOptions _opt;

    public ListPenjualanService(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }

    public IEnumerable<PenjualanDto> Execute(string req)
    {
        const string sql = @"
            SELECT 
	            aa.fs_kd_trs as PenjualanId, aa.fd_tgl_jam_trs as PenjualanDate, 
	            aa.fs_kd_reg as RegId, aa.fs_kd_resep as ResepId
            FROM 
	            tb_trs_Dobill_umum aa
            WHERE 
	            aa.fs_kd_reg = @regId
	            AND aa.fd_tgl_void = '3000-01-01'";

        var dp = new DynamicParameters();
        dp.AddParam("@regId", req, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return conn.Read<PenjualanDto>(sql, dp);
    }
}
