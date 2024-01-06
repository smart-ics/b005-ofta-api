using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class DocSigneeDal : IDocSigneeDal
{
    private readonly DatabaseOptions _opt;

    public DocSigneeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<DocSigneeModel> listModel)
    {
        var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        var bcp = new SqlBulkCopy(conn);

        conn.Open();
        bcp.AddMap("DocId", "DocId");
        bcp.AddMap("UserOftaId", "UserOftaId");
        bcp.AddMap("Email", "Email");
        bcp.AddMap("SignTag", "SignTag");
        bcp.AddMap("SignPosition", "SignPosition");
        bcp.AddMap("Level", "Level");
        bcp.AddMap("IsSigned", "IsSigned");
        bcp.AddMap("SignedDate", "SignedDate");

        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_DocSignee";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IDocKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_DocSignee
            WHERE
                DocId = @DocId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocId", key.DocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<DocSigneeModel> ListData(IDocKey filter)
    {
        const string sql = @"
            SELECT
                DocId, UserOftaId, Email, SignTag, 
                SignPosition, Level, IsSigned, SignedDate
            FROM
                OFTA_DocSignee
            WHERE
                DocId = @DocId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocId", filter.DocId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocSigneeModel>(sql, dp);
    }
}