using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IKlaimBpjsWriter : INunaWriterWithReturn<KlaimBpjsModel>
{
}
public class KlaimBpjsWriter : IKlaimBpjsWriter
{
    public KlaimBpjsModel Save(KlaimBpjsModel model)
    {
        throw new NotImplementedException();
    }
}