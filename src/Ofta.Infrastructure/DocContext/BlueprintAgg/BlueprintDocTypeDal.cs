using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.BlueprintAgg.Contracts;
using Ofta.Domain.DocContext.BlueprintAgg;
using Ofta.Domain.DocContext.BundleSpecAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.BlueprintAgg;

public class BlueprintDocTypeDal : IBlueprintDocTypeDal
{
    private readonly DatabaseOptions _opt;

    public BlueprintDocTypeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<BlueprintDocTypeModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        conn.Open();
        
        bcp.AddMap("BlueprintId", "BlueprintId");
        bcp.AddMap("BlueprintDocTypeId", "BlueprintDocTypeId");
        bcp.AddMap("NoUrut", "NoUrut");
        bcp.AddMap("DocTypeId", "DocTypeId");
        

        var fetched = listModel.ToList();   
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_BlueprintDocType";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IBlueprintKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_BlueprintDocType
            WHERE
                BlueprintId = @BlueprintId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", key.BlueprintId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<BlueprintDocTypeModel> ListData(IBlueprintKey filter)
    {
        const string sql = @"
            SELECT
                aa.BlueprintId, aa.BlueprintDocTypeId,
                aa.NoUrut, aa.DocTypeId, 
                ISNULL(bb.DocTypeName, '') AS DocTypeName
            FROM
                OFTA_BlueprintDocType aa
                LEFT JOIN OFTA_DocType bb ON aa.DocTypeId = bb.DocTypeId
            WHERE
                aa.BlueprintId = @BlueprintId ";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", filter.BlueprintId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<BlueprintDocTypeModel>(sql, dp);
    }
}