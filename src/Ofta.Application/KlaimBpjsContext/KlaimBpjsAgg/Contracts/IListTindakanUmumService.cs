using Nuna.Lib.CleanArchHelper;
using System.Runtime.CompilerServices;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class TindakanUmumDto
{
    public string TindakanId { get; set; }
    public string RegId { get; set; }
    public string LayananId { get; set; }
    public string LayananName { get; set; }
    public string LayananTypeDkdId { get; set; }

}
public interface IListTindakanUmumService : INunaService<IEnumerable<TindakanUmumDto>, string>
{
}
