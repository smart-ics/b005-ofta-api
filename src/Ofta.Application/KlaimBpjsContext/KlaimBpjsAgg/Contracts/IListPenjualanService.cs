using Nuna.Lib.CleanArchHelper;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class PenjualanDto
{
    public string PenjualanId { get; set; }
    public DateTime PenjualanDate { get; set; }
    public string RegId { get; set; }
    public string ResepId { get; set; }
}

public interface IListPenjualanService : INunaService<IEnumerable<PenjualanDto>, string>
{
}
