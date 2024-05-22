using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListResepService : IListResepService
{
    private readonly RemoteCetakOptions _opt;

    public ListResepService(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }

    public IEnumerable<ResepDto> Execute(string req)
    {
        const string sql = @"
            SELECT
                aa.fs_kd_trs AS ResepId,
                aa.fd_tgl_trs AS ResepDate,
                ISNULL(bb.fs_nm_layanan, '') Layanan,
                ISNULL(cc.fs_nm_peg, '') Dokter
            FROM
                ta_trs_kartu_periksa aa
                LEFT JOIN ta_layanan bb ON aa.fs_kd_layanan = bb.fs_kd_layanan
                LEFT JOIN td_peg cc ON aa.fs_kd_petugas_medis = cc.fs_kd_peg
            WHERE
                aa.fs_kd_reg = @regId
                AND aa.fd_tgl_void = '3000-01-01'
                AND aa.fb_resep = 1";

        var dp = new DynamicParameters();
        dp.AddParam("@regId", req, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return conn.Read<ResepDto>(sql, dp);
    }
}