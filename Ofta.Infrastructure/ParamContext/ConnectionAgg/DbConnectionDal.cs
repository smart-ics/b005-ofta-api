using Microsoft.Extensions.Options;
using Ners.Infrastructure.Helpers;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.ConnectionAgg;

namespace Ofta.Infrastructure.ParamContext.ConnectionAgg;

public class DbConnectionDal : IDbConnectionDal
{
    private readonly DatabaseOptions _opt;

    public DbConnectionDal(IOptions<DatabaseOptions> opt)
    {
        _opt = opt.Value;
    }

    public DataBaseConnModel Get()
    {

        var result = new DataBaseConnModel
        {
            DataSource = _opt.ServerName,
            Catalog = _opt.DbName,
            TglJam = DateTime.Now
        };
        return result;
    }
}
