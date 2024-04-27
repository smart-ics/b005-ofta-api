using Dapper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using System.Data.SqlClient;
using System.Data;
using Nuna.Lib.DataAccessHelper;
using Microsoft.Extensions.Options;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class GetSepService : IGetSepService
{
    private readonly RemoteCetakOptions _opt;

    public GetSepService(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }
    public SepDto Execute(string req)
    {
        const string sql = @"
            SELECT 
	            aa.fs_kd_trs as TrsSepId, aa.fs_no_sep as NoSep, 
                aa.fs_kd_reg as RegId
            FROM
	            VCLAIM_sep aa
            WHERE
                aa.fs_kd_reg = @regId
                AND aa.fd_tgl_void = '3000-01-01'";

        var dp = new DynamicParameters();
        dp.AddParam("@regId", req, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return conn.ReadSingle<SepDto>(sql, dp);
    }
}
