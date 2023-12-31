using MediatR;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Domain.ParamContext.ConnectionAgg;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Ofta.Application.ParamContext.ConnectionAgg.UseCases;

public record DbConnectionQuery : IRequest<DbConnectionResponse>;

public class DbConnectionResponse
{
    public string DataSource { get; set; }
    public string Catalog { get; set; }
    public string Tanggal { get; set; }
    public string Jam { get; set; }
}

public class DbConnectionHandler : IRequestHandler<DbConnectionQuery, DbConnectionResponse>
{
    private DataBaseConnModel _aggRoot = new();
    private readonly IDbConnectionDal _dbConn;

    public DbConnectionHandler(IDbConnectionDal dbConn)
    {
        _dbConn = dbConn;
    }

    public Task<DbConnectionResponse> Handle(DbConnectionQuery request, CancellationToken cancellationToken)
    {
        var dbConn = _dbConn.Get();
        var result = new DbConnectionResponse
        {
            DataSource = dbConn.DataSource,
            Catalog = dbConn.Catalog,
            Tanggal = dbConn.TglJam.ToString(DateFormatEnum.YMD),
            Jam = dbConn.TglJam.ToString(DateFormatEnum.HMS)
        };
        return Task.FromResult(result);
    }
}

