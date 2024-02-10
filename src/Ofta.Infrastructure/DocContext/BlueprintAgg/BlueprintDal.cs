using System.Data;
using System.Data.SqlClient;
using Dapper;
using Ofta.Application.DocContext.BlueprintAgg.Contracts;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.DocContext.BlueprintAgg;
using Ofta.Domain.DocContext.BundleSpecAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.BlueprintAgg;

public class BlueprintDal : IBlueprintDal
{
    private readonly DatabaseOptions _opt;

    public BlueprintDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(BlueprintModel model)
    {
        //  define sql query
        const string sql = @"
            INSERT INTO OFTA_Blueprint 
                (BlueprintId, BlueprintName) 
            VALUES 
                (@BlueprintId, @BlueprintName)";
        
        //  define parameters
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", model.BlueprintId, SqlDbType.VarChar);
        dp.AddParam("@BlueprintName", model.BlueprintName, SqlDbType.VarChar);
        
        //  execute query
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    
    public void Update(BlueprintModel model)
    {
        const string sql = @"
            UPDATE 
                OFTA_Blueprint 
            SET 
                BlueprintName = @BlueprintName 
            WHERE 
                BlueprintId = @BlueprintId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", model.BlueprintId, SqlDbType.VarChar);
        dp.AddParam("@BlueprintName", model.BlueprintName, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IBlueprintKey key)
    {
        const string sql = @"
            DELETE FROM 
                OFTA_Blueprint 
            WHERE 
                BlueprintId = @BlueprintId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", key.BlueprintId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public BlueprintModel GetData(IBlueprintKey key)
    {
        const string sql = @"
            SELECT 
                BlueprintId, 
                BlueprintName 
            FROM 
                OFTA_Blueprint 
            WHERE 
                BlueprintId = @BlueprintId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@BlueprintId", key.BlueprintId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<BlueprintModel>(sql, dp);
    }

    public IEnumerable<BlueprintModel> ListData()
    {
        const string sql = @"
            SELECT 
                BlueprintId, 
                BlueprintName 
            FROM 
                OFTA_Blueprint";
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<BlueprintModel>(sql);
    }
}