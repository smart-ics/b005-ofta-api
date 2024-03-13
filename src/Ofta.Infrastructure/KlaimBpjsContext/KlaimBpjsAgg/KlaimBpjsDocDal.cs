using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDocDal : IKlaimBpjsDocDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsDocDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }


    public void Insert(IEnumerable<KlaimBpjsDocModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("KlaimBpjsId", "KlaimBpjsId");
        bcp.AddMap("KlaimBpjsDocId", "KlaimBpjsDocId");
        bcp.AddMap("NoUrut", "NoUrut");
        bcp.AddMap("DocTypeId", "DocTypeId");
        bcp.AddMap("DocId", "DocId");
        bcp.AddMap("DocUrl", "DocUrl");
        bcp.AddMap("PrintReffId", "PrintReffId");
        bcp.AddMap("PrintState", "PrintState");
 
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_KlaimBpjsDoc";
        bcp.WriteToServer(fetched.AsDataTable());
    }
    
    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjsDoc
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<KlaimBpjsDocModel> ListData(IKlaimBpjsKey filter)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsDocId, NoUrut, DocTypeId, 
                DocId, DocUrl, PrintReffId, PrintState
            FROM
                OFTA_KlaimBpjsDoc
            WHERE 
                KlaimBpjsId = @KlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", filter.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<KlaimBpjsDocModel>(sql, dp);
    }
}