using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.DocContext.DocAgg;

public class DocScopeDal : IDocScopeDal
{
    private readonly DatabaseOptions _opt;

    public DocScopeDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(IEnumerable<AbstractDocScopeModel> model)
    {
        var listDto = new List<DocScopeDto>();
        foreach (var item in model)
        {
            switch (item)
            {
                case DocScopeUserModel scopeUser:
                    listDto.Add(new DocScopeDto(item.DocId, 0,  scopeUser.UserOftaId, string.Empty));
                    break;
                case DocScopeTeamModel scopeTeam:
                    listDto.Add(new DocScopeDto(item.DocId, 0,  string.Empty, scopeTeam.TeamId));
                    break;
            }
        }

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);
        
        bcp.AddMap("DocId", "DocId");
        bcp.AddMap("ScopeType", "ScopeType");
        bcp.AddMap("UserOftaId", "UserOftaId");
        bcp.AddMap("TeamId", "TeamId");

        bcp.BatchSize = listDto.Count;
        bcp.DestinationTableName = "OFTA_DocScope";
        conn.Open();
        bcp.WriteToServer(listDto.AsDataTable());
    }
    private class DocScopeDto
    {
        public DocScopeDto(string docId, int scopeType, string userOftaId, string teamId)
        {
            DocId = docId;
            ScopeType = scopeType;
            UserOftaId = userOftaId;
            TeamId = teamId;
        }

        public string DocId { get; set; }
        public int ScopeType { get; set; }
        public string UserOftaId { get; set; }
        public string TeamId { get; set; }
    }

    public void Delete(IDocKey key)
    {
        const string sql = @"
            DELETE FROM OFTA_DocScope
            WHERE DocId = @DocId ";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", key.DocId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<AbstractDocScopeModel> ListData(IDocKey filter)
    {
        const string sql = @"
            SELECT  
                DocId, ScopeType, UserOftaId, TeamId
            FROM
                OFTA_DocScope
            WHERE
                DocId = @DocId ";

        var dp = new DynamicParameters();
        dp.AddParam("@DocId", filter.DocId, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        var listScope = conn.Read<DocScopeDto>(sql, dp);
        if (listScope is null)
            return null!;

        var result = new List<AbstractDocScopeModel>();
        foreach (var item in listScope)
        {
            switch (item.ScopeType)
            {
                case 0:
                    result.Add(new DocScopeUserModel
                    {
                        DocId = item.DocId,
                        ScopeType = item.ScopeType,
                        UserOftaId = item.UserOftaId,
                    });
                    break;
                case 1:
                    result.Add(new DocScopeTeamModel
                    {
                        DocId = item.DocId,
                        ScopeType = item.ScopeType,
                        TeamId = item.TeamId,
                    });
                    break;
            }
        }
        return result;
    }
}