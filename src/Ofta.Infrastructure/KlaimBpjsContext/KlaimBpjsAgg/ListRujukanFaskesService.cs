using Dapper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using System.Data.SqlClient;
using System.Data;
using Nuna.Lib.DataAccessHelper;
using Microsoft.Extensions.Options;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListRujukanFaskesService : IListRujukanFaskesService
{
    private readonly RemoteCetakOptions _opt;

    public ListRujukanFaskesService(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }
    public IEnumerable<RujukanFaskesDto> Execute(string req)
    {
        const string sql = @"
            SELECT 
	            aa.fs_kd_trs as TrsRujukanId, aa.fs_kd_reg as RegId,
	            aa.fs_kd_ppk_perujuk as PpkPerujukId, aa.fs_nm_ppk_perujuk as PpkPerujukName,
	            aa.fs_kd_ppk_dirujuk as PpkDirujukId, aa.fs_nm_ppk_dirujuk as PpkDirujukName
            FROM
	            ta_trs_rujukan_faskes aa
            WHERE
	            aa.fs_kd_reg = 'RG01789502'
	            AND aa.fd_tgl_void = '3000-01-01'";

        var dp = new DynamicParameters();
        dp.AddParam("@regId", req, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return conn.Read<RujukanFaskesDto>(sql, dp);
    }
}
