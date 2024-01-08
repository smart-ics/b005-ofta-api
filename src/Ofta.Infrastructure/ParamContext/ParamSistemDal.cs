using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.SystemAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.ParamContext;

public class ParamSistemDal : IParamSistemDal
{
    private readonly DatabaseOptions _opt;

    public ParamSistemDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(ParamSistemModel model)
    {
        const string sql = @"
            INSERT INTO ParamSistem (
                ParamSistemId, ParamSistemValue )
            VALUES (
                @ParamSistemId, @ParamSistemValue )";

        var dp = new DynamicParameters();
        dp.AddParam("@ParamSistemId", model.ParamSistemId, SqlDbType.VarChar);
        dp.AddParam("@ParamSistemValue", model.ParamSistemValue, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(ParamSistemModel model)
    {
        const string sql = @"
            UPDATE
                ParamSistem
            SET 
                ParamSistemValue = @ParamSistemValue
            WHERE
                ParamSistemId = @ParamSistemId";

        var dp = new DynamicParameters();
        dp.AddParam("@ParamSistemId", model.ParamSistemId, SqlDbType.VarChar);
        dp.AddParam("@ParamSistemValue", model.ParamSistemValue, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IParamSistemKey key)
    {
        const string sql = @"
            DELETE FROM
                ParamSistem
            WHERE
                ParamSistemId = @ParamSistemId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@ParamSistemId", key.ParamSistemId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public ParamSistemModel GetData(IParamSistemKey key)
    {
        const string sql = @"
            SELECT
                ParamSistemId, ParamSistemValue
            FROM
                ParamSistem
            WHERE
                ParamSistemId = @ParamSistemId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@ParamSistemId", key.ParamSistemId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<ParamSistemModel>(sql, dp);
    }

    public IEnumerable<ParamSistemModel> ListData()
    {
        const string sql = @"
            SELECT
                ParamSistemId, ParamSistemValue
            FROM
                ParamSistem";
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<ParamSistemModel>(sql);
    }
}