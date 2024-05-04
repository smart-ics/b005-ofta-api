using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.DocTypeAgg;

public class DocTypeTagDal : IDocTypeTagDal
{
    private readonly DatabaseOptions _opt;

    public DocTypeTagDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<DocTypeTagModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);

        conn.Open();
        bcp.AddMap("DocTypeId", "DocTypeId");
        bcp.AddMap("Tag", "Tag");
        
        bcp.DestinationTableName = "dbo.OFTA_DocTypeTag";
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.WriteToServer(fetched.AsDataTable());
    }

    public void Delete(IDocTypeKey key)
    {
        //  create query delete
        const string sql = @"
            DELETE FROM
                OFTA_DocTypeTag
            WHERE
                DocTypeId = @DocTypeId";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", key.DocTypeId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<DocTypeTagModel> ListData(IDocTypeKey filter)
    {
        //  create select query table OFTA_DocTypeTag
        const string sql = @"
            SELECT
                DocTypeId, Tag
            FROM 
                OFTA_DocTypeTag
            WHERE
                DocTypeId = @DocTypeId ";

        var dp = new DynamicParameters();
        dp.AddParam("@DocTypeId", filter.DocTypeId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocTypeTagModel>(sql, dp);
    }

    public IEnumerable<DocTypeTagModel> ListData(List<ITag> filter1)
    {
        const string sql = @"
            SELECT
                DocTypeId, Tag
            FROM 
                OFTA_DocTypeTag
            WHERE
                Tag in @Tags ";

        var dp = new DynamicParameters();
        dp.Add("@Tags", filter1.Select(x => x.Tag).ToArray());
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocTypeTagModel>(sql, dp);
    }

    public IEnumerable<DocTypeTagModel> ListData()
    {
        const string sql = @"
            SELECT
                Tag
            FROM 
                OFTA_DocTypeTag
            Group By Tag ";


        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<DocTypeTagModel>(sql);
    }
}