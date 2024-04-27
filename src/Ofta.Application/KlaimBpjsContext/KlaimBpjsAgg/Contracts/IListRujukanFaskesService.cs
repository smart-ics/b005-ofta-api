using Nuna.Lib.CleanArchHelper;
using Ofta.Application.DocContext.DocAgg.UseCases;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class RujukanFaskesDto
{
    public string TrsRujukanFaskesId { get; set; }
    public string RegId { get; set; }
    public string PpkPerujukId { get; set; }
    public string PpkPerujukName { get; set; }
    public string PpkDirujukId { get; set; }
    public string PpkDirujukName { get; set; }
}
public interface IListRujukanFaskesService : INunaService<IEnumerable<RujukanFaskesDto>, string>
{
 
}
