using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;
using System.Data.SqlClient;
using System.Data;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsMergerFileDal : IKlaimBpjsMergerFileDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsMergerFileDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(KlaimBpjsMergerFileModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_KlaimBpjsMergerFile(
                KlaimBpjsId, DocId, DocUrl )
            VALUES(
                @KlaimBpjsId, @DocId, @DocUrl )";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", model.KlaimBpjsId, SqlDbType.VarChar);
        dp.AddParam("@DocId", model.DocId, SqlDbType.VarChar);
        dp.AddParam("@DocUrl", model.DocUrl, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjsMergerFile
            WHERE
                KlaimBpjsId = @KlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public KlaimBpjsMergerFileModel GetData(IKlaimBpjsKey key)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, DocId, DocUrl
            FROM
                OFTA_KlaimBpjsMergerFile
            WHERE
                KlaimBpjsId = @KlaimBpjsId";

        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.QueryFirstOrDefault<KlaimBpjsMergerFileModel>(sql, dp);
    }
}