using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.DocTypeAgg;

public class DocTypeFileUrlDal : IDocTypeFileUrlDal
{
    private readonly DatabaseOptions _opt;

    public DocTypeFileUrlDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IDocTypeFileUrl model)
    {
        const string sql = @"
            INSERT INTO OFTA_DocTypeFileUrl(
                DocTypeId, FileUrl)
            VALUES (
                @DocTypeId, @FileUrl)";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@FileUrl", model.FileUrl, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(IDocTypeFileUrl model)
    {
        const string sql = @"
            UPDATE 
                OFTA_DocTypeFileUrl
            SET
                FileUrl = @FileUrl
            WHERE
                DocTypeId = @DocTypeId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@FileUrl", model.DocTypeId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IDocTypeKey key)
    {
        const string sql = @"
            DELETE FROM 
                OFTA_DocTypeFileUrl
            WHERE
                DocTypeId = @DocTypeId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IDocTypeFileUrl GetData(IDocTypeKey key)
    {
        const string sql = @"
            SELECT 
                DocTypeId, FileUrl
            FROM 
                OFTA_DocTypeFileUrl
            WHERE
                DocTypeId = @DocTypeId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.QueryFirstOrDefault<DocTypeModel>(sql, dp);
    }

    public IEnumerable<IDocTypeFileUrl> ListData()
    {
        const string sql = @"
            SELECT 
                DocTypeId, FileUrl
            FROM 
                OFTA_DocTypeFileUrl";
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Query<DocTypeModel>(sql);
    }
}