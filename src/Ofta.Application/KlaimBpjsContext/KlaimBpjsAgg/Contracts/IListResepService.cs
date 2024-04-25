using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;


public class ResepDto
{
    public string ResepId {get; set;} 
    public DateTime ResepDate {get; set;} 
    public string Layanan {get; set;} 
    public string Dokter {get; set;}    
};
public interface IListResepService : INunaService<IEnumerable<ResepDto>, string>
{
}