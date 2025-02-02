using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDocTypeDal : IKlaimBpjsDocTypeDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsDocTypeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }


    public void Insert(IEnumerable<KlaimBpjsDocTypeModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("KlaimBpjsId", "KlaimBpjsId");
        bcp.AddMap("KlaimBpjsDocTypeId", "KlaimBpjsDocTypeId");
        bcp.AddMap("NoUrut", "NoUrut");
        bcp.AddMap("DocTypeId", "DocTypeId");
        bcp.AddMap("DocTypeName", "DocTypeName");
        bcp.AddMap("DrafterUserId", "DrafterUserId");
        bcp.AddMap("ToBePrinted", "ToBePrinted");
 
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_KlaimBpjsDocType";
        bcp.WriteToServer(fetched.AsDataTable());
    }
    
    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjsDocType
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<KlaimBpjsDocTypeModel> ListData(IKlaimBpjsKey filter)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsDocTypeId, NoUrut, 
                DocTypeId, DocTypeName, DrafterUserId, ToBePrinted
            FROM
                OFTA_KlaimBpjsDocType
            WHERE 
                KlaimBpjsId = @KlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", filter.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<KlaimBpjsDocTypeModel>(sql, dp);
    }
}