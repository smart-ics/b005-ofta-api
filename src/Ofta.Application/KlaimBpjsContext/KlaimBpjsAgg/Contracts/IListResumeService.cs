using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class ResumeDto
{
    public string ResumeId { get; set; }
    public DateTime Tgljam { get; set; }
    public string LayananId { get; set; }
    public string LayananName { get; set; }
    public string DokterId { get; set; }
    public string DokterName { get; set; }
}
public interface IListResumeAdministratifService : INunaService<IEnumerable<ResumeDto>, string>
{
}

public interface IListResumeService : INunaService<IEnumerable<ResumeDto>, string>
{
}
