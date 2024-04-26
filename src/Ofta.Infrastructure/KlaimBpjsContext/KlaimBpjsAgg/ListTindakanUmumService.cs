using Dapper;
using Microsoft.Extensions.Options;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using System.Data.SqlClient;
using System.Data;
using Nuna.Lib.DataAccessHelper;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListTindakanUmumService : IListTindakanUmumService
{
    private readonly RemoteCetakOptions _opt;

    public ListTindakanUmumService(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }
    public IEnumerable<TindakanUmumDto> Execute(string req)
    {
        const string sql = @"
            SELECT
	            aa.fs_kd_trs as TindakanId, aa.fs_kd_reg as RegId, 
	            aa.fs_kd_layanan as LayananId,
	            ISNULL(bb.fs_nm_layanan, '') as LayananName,
	            ISNULL(bb.fs_kd_layanan_tipe_dk, '') as LayananTypeDkdId
            FROM 
	            ta_trs_tdk_umum aa
	            LEFT JOIN ta_layanan bb ON aa.fs_kd_layanan = bb.fs_kd_layanan
            WHERE
	            aa.fs_kd_reg = 'RG01789502'
	            AND aa.fd_tgl_void = '3000-01-01'";

        var dp = new DynamicParameters();
        dp.AddParam("@regId", req, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return conn.Read<TindakanUmumDto>(sql, dp);
    }
}
