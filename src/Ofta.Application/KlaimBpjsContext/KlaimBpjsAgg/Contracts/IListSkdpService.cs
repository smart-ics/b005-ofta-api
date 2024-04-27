using Nuna.Lib.CleanArchHelper;
using System.Runtime.CompilerServices;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class SkdpDto
{
    public string NoSuratKontrol { get; set; }
    public string RegId { get; set; }
    public bool IsSpri { get; set; }
}
public interface IListSkdpService : INunaService<IEnumerable<SkdpDto>, string>
{
}
