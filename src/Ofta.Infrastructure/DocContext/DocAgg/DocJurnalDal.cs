using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class DocJurnalDal : IDocJurnalDal
{
    private readonly DatabaseOptions _opt;

    public DocJurnalDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<DocJurnalModel> listModel)
    {
        var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("DocId","DocId");
        bcp.AddMap("NoUrut","NoUrut");
        bcp.AddMap("JurnalDate","JurnalDate");
        bcp.AddMap("JurnalDesc","JurnalDesc");
        bcp.AddMap("DocState","DocState");
        
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_DocJurnal";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IDocKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_DocJurnal
            WHERE
                DocId = @DocId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", key.DocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<DocJurnalModel> ListData(IDocKey filter)
    {
        const string sql = @"
            SELECT
                DocId, NoUrut, JurnalDate, JurnalDesc, DocState
            FROM
                OFTA_DocJurnal
            WHERE
                DocId = @DocId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocId", filter.DocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocJurnalModel>(sql, dp);
    }
}