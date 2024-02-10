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

public class BlueprintSigneeDal : IBlueprintSigneeDal
{
    private readonly DatabaseOptions _opt;

    public BlueprintSigneeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<BlueprintSigneeModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        conn.Open();
        bcp.AddMap("BlueprintId", "BlueprintId");
        bcp.AddMap("BlueprintDocTypeId","BlueprintDocTypeId");
        bcp.AddMap("BlueprintSigneeId","BlueprintSigneeId");
        bcp.AddMap("NoUrut", "NoUrut");

        bcp.AddMap("UserOftaId", "UserOftaId");
        bcp.AddMap("Email", "Email");
        bcp.AddMap("SignTag", "SignTag");
        bcp.AddMap("SignPosition", "SignPosition");
            
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.DestinationTableName = "OFTA_BlueprintSignee";
        bcp.WriteToServer(fetched.AsDataTable());
        conn.Close();
    }

    public void Delete(IBlueprintKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_BlueprintSignee
            WHERE
                BlueprintId = @BlueprintId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", key.BlueprintId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<BlueprintSigneeModel> ListData(IBlueprintKey filter)
    {
        const string sql = @"
            SELECT
                BlueprintId, BlueprintDocTypeId, BlueprintSigneeId, NoUrut,
                UserOftaId, Email, SignTag, SignPosition
            FROM
                OFTA_BlueprintSignee
            WHERE
                BlueprintId = @BlueprintId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", filter.BlueprintId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<BlueprintSigneeModel>(sql, dp);
    }
}