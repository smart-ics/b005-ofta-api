using Dapper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class ListSkdpService : IListSkdpService
{
    private readonly RemoteCetakOptions _opt;

    public ListSkdpService(IOptions<RemoteCetakOptions> opt)
    {
        _opt = opt.Value;
    }
    public IEnumerable<SkdpDto> Execute(string req)
    {
        const string sql = @"
            SELECT 
                NoSuratKontrol, KodeReg AS RegId, IsSPRI
            FROM 
                vclaim_skdp
            WHERE  
                KodeReg  = @regId 
                AND tglvoid = '3000-01-01' 
                AND isSKDPHIDOK = '0'";

        var dp = new DynamicParameters();
        dp.AddParam("@regId", req, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelperRemoteCetak.Get(_opt));
        return conn.Read<SkdpDto>(sql, dp);
    }
}
