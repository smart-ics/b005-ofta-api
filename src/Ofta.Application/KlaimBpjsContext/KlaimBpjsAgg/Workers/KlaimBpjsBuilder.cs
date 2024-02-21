using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IKlaimBpjsBuilder : INunaBuilder<KlaimBpjsModel>
{
    IKlaimBpjsBuilder Create();
    IKlaimBpjsBuilder Load(IKlaimBpjsKey klaimBpjsKey);
    IKlaimBpjsBuilder Attach(KlaimBpjsModel klaimBpjs);
    IKlaimBpjsBuilder GenListBlueprint();
    IKlaimBpjsBuilder AddDoc(IDocTypeKey docTypeKey, IDocKey docKey);
    IKlaimBpjsBuilder RemoveDoc(IDocKey docKey);
    IKlaimBpjsBuilder AddSignee(IDocTypeKey docTypeKey, string email);
    IKlaimBpjsBuilder RemoveSignee(IDocTypeKey docTypeKey, string email);
}

public class KlaimBpjsBuilder : IKlaimBpjsBuilder
{
    public KlaimBpjsModel Build()
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder Create()
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder Load(IKlaimBpjsKey klaimBpjsKey)
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder Attach(KlaimBpjsModel klaimBpjs)
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder GenListBlueprint()
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder AddDoc(IDocTypeKey docTypeKey, IDocKey docKey)
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder RemoveDoc(IDocKey docKey)
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder AddSignee(IDocTypeKey docTypeKey, string email)
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder RemoveSignee(IDocTypeKey docTypeKey, string email)
    {
        throw new NotImplementedException();
    }

    public IKlaimBpjsBuilder ListBlueprint()
    {
        throw new NotImplementedException();
    }
}