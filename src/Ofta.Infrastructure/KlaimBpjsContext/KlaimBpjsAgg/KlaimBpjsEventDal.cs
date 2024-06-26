﻿using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsEventDal : IKlaimBpjsEventDal
{
    private readonly DatabaseOptions _opt;

    public KlaimBpjsEventDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }
    
    public void Insert(IEnumerable<KlaimBpjsEventModel> listModel)
    {
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        using var bcp = new SqlBulkCopy(conn);

        conn.Open();
        bcp.AddMap("KlaimBpjsId", "KlaimBpjsId");
        bcp.AddMap("KlaimBpjsJurnalId", "KlaimBpjsJurnalId");
        bcp.AddMap("NoUrut", "NoUrut");
        bcp.AddMap("EventDate", "EventDate");
        bcp.AddMap("Description", "Description");
        
        bcp.DestinationTableName = "OFTA_KlaimBpjsEvent";
        var fetched = listModel.ToList();
        bcp.BatchSize = fetched.Count;
        bcp.WriteToServer(fetched.AsDataTable());
    }

    public void Delete(IKlaimBpjsKey key)
    {
        const string sql = @"
            DELETE FROM
                OFTA_KlaimBpjsEvent
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", key.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }

    public IEnumerable<KlaimBpjsEventModel> ListData(IKlaimBpjsKey filter)
    {
        const string sql = @"
            SELECT
                KlaimBpjsId, KlaimBpjsJurnalId, NoUrut, 
                EventDate, Description
            FROM 
                OFTA_KlaimBpjsEvent
            WHERE
                KlaimBpjsId = @KlaimBpjsId";
        
        var dp = new DynamicParameters();
        dp.AddParam("@KlaimBpjsId", filter.KlaimBpjsId, SqlDbType.VarChar);
        
        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        return conn.Read<KlaimBpjsEventModel>(sql, dp);
    }
}