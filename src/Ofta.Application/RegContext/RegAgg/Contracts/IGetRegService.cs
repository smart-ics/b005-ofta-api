using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.RegContext.RegAgg;

namespace Ofta.Application.RegContext.RegAgg.Contracts;

public interface IGetRegService : INunaService<RegModel?, IRegKey>
{
}