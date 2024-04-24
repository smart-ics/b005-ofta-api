using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsPrintOutDal : IKlaimBpjsPrintOutDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsPrintOutDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }


    public void Insert(IEnumerable<KlaimBpjsPrintOutModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("KlaimBpjsId", "KlaimBpjsId");
        bcp.AddMap("KlaimBpjsDocTypeId", "KlaimBpjsDocTypeId");
        bcp.AddMap("KlaimBpjsPrintOutId", "KlaimBpjsPrintOutId");
        bcp.AddMap("NoUrut", "NoUrut");
        bcp.AddMap("PrintOutReffId", "PrintOutReffId");
        bcp.AddMap("PrintState", "PrintState");
        bcp.AddMap("DocId", "DocId");
        bcp.AddMap("DocUrl", "DocUrl");
        
 
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_KlaimBpjsPrintOut";
        bcp.WriteToServer(fetched.AsDataTable());
    }
    
    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjsPrintOut
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<KlaimBpjsPrintOutModel> ListData(IKlaimBpjsKey filter)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsDocTypeId, KlaimBpjsPrintOutId, 
                NoUrut, PrintOutReffId, DocId, DocUrl
            FROM
                OFTA_KlaimBpjsPrintOut
            WHERE 
                KlaimBpjsId = @KlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", filter.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<KlaimBpjsPrintOutModel>(sql, dp);
    }

}