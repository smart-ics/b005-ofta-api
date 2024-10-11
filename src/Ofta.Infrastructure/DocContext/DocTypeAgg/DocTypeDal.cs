using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.DocTypeAgg;

public class DocTypeDal : IDocTypeDal
{
    private readonly DatabaseOptions _opt;

    public DocTypeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(DocTypeModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_DocType(
                DocTypeId, DocTypeName, DocTypeCode,
                IsStandard, IsActive, FileUrl, 
                JenisDokRemoteCetak, DefaultDrafterUserId)
            VALUES (
                @DocTypeId, @DocTypeName,@DocTypeCode, @IsStandard, 
                @IsActive, @FileUrl, @JenisDokRemoteCetak, @DefaultDrafterUserId)";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@DocTypeName", model.DocTypeName, SqlDbType.VarChar);
        dp.AddParam("@DocTypeCode", model.DocTypeCode, SqlDbType.VarChar);
        dp.AddParam("@IsStandard", model.IsStandard, SqlDbType.Bit);
        dp.AddParam("@IsActive", model.IsActive, SqlDbType.Bit);
        dp.AddParam("@FileUrl", model.FileUrl, SqlDbType.VarChar);
        dp.AddParam("@JenisDokRemoteCetak", model.JenisDokRemoteCetak, SqlDbType.VarChar);
        dp.AddParam("@DefaultDrafterUserId", model.DefaultDrafterUserId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Update(DocTypeModel model)
    {
        const string sql = @"
            UPDATE 
                OFTA_DocType
            SET
                DocTypeName = @DocTypeName, 
                DocTypeCode = @DocTypeCode,
                IsStandard = @IsStandard,
                IsActive = @IsActive,
                FileUrl = @FileUrl,
                JenisDokRemoteCetak = @JenisDokRemoteCetak,
                DefaultDrafterUserId = @DefaultDrafterUserId
            WHERE
                DocTypeId = @DocTypeId
";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", model.DocTypeId, SqlDbType.VarChar);
        dp.AddParam("@DocTypeName", model.DocTypeName, SqlDbType.VarChar);
        dp.AddParam("@DocTypeCode", model.DocTypeCode, SqlDbType.VarChar);
        dp.AddParam("@IsStandard", model.IsStandard, SqlDbType.Bit);
        dp.AddParam("@IsActive", model.IsActive, SqlDbType.Bit);
        dp.AddParam("@FileUrl", model.FileUrl, SqlDbType.VarChar);
        dp.AddParam("@JenisDokRemoteCetak", model.JenisDokRemoteCetak, SqlDbType.VarChar);
        dp.AddParam("@DefaultDrafterUserId", model.DefaultDrafterUserId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IDocTypeKey key)
    {
        const string sql = @"
            DELETE FROM 
                OFTA_DocType
            WHERE
                DocTypeId = @DocTypeId ";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public DocTypeModel GetData(IDocTypeKey key)
    {
        const string sql = @"
            SELECT
                DocTypeId, DocTypeName, DocTypeCode,
                IsStandard, IsActive, FileUrl,
                JenisDokRemoteCetak, DefaultDrafterUserId
            FROM
                OFTA_DocType
            WHERE
                DocTypeId = @DocTypeId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.ReadSingle<DocTypeModel>(sql, dp);
    }

    public IEnumerable<DocTypeModel> ListData()
    {
        const string sql = @"
            SELECT
                DocTypeId, DocTypeName,DocTypeCode,
                IsStandard, IsActive, FileUrl, 
                JenisDokRemoteCetak, DefaultDrafterUserId
            FROM
                OFTA_DocType ";
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocTypeModel>(sql);
    }
}