using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Infrastructure.Helpers;

namespace Ofta.Infrastructure.CallbackContext.CallbackSignStatusAgg;

public class CallbackSignStatusDal: ICallbackSignStatusDal
{
    private readonly DatabaseOptions _opt;

    public CallbackSignStatusDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public void Insert(CallbackSignStatusModel model)
    {
        const string sql = @"
            INSERT INTO OFTA_CallbackSignStatus (CallbackDate, Payload)
            VALUES (@CallbackDate, @Payload)";

        var dp = new DynamicParameters();
        dp.AddParam("@CallbackDate", model.CallbackDate, SqlDbType.DateTime);
        dp.AddParam("@Payload", model.Payload, SqlDbType.VarChar);

        using var conn = new SqlConnection(ConnStringHelper.Get(_opt));
        conn.Execute(sql, dp);
    }
}