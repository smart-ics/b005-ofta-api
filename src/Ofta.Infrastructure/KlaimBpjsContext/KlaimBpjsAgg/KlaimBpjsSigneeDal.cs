using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsSigneeDal : IKlaimBpjsSigneeDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsSigneeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<KlaimBpjsSigneeModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("KlaimBpjsId", "KlaimBpjsId");
        bcp.AddMap("KlaimBpjsDocId", "KlaimBpjsDocId");
        bcp.AddMap("KlaimBpjsSigneeId", "KlaimBpjsSigneeId");
        bcp.AddMap("NoUrut", "NoUrut");
        bcp.AddMap("UserOftaId", "UserOftaId");
        bcp.AddMap("Email", "Email");
        bcp.AddMap("SignTag", "SignTag");
        
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_KlaimBpjsSignee";
        bcp.WriteToServer(fetched.AsDataTable());
    }

    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjsSignee
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<KlaimBpjsSigneeModel> ListData(IKlaimBpjsKey filter)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsDocId, KlaimBpjsSigneeId, NoUrut, UserOftaId, Email, SignTag
            FROM
                OFTA_KlaimBpjsSignee
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", filter.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<KlaimBpjsSigneeModel>(sql, dp);
    }
}