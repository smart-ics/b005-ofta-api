using Nuna.Lib.CleanArchHelper;
using System.Runtime.CompilerServices;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class SepDto
{
    public string TrsSepId { get; set; }
    public string SepId { get; set; }
    public string RegId { get; set; }
}
public interface IGetSepService : INunaService<SepDto, string>
{
}
