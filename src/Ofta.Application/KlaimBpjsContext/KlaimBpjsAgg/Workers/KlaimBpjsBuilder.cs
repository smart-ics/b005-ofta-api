using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IKlaimBpjsBuilder : INunaBuilder<KlaimBpjsModel>
{
    IKlaimBpjsBuilder Create();
    IKlaimBpjsBuilder Load(IKlaimBpjsKey klaimBpjsKey);
    IKlaimBpjsBuilder Attach(KlaimBpjsModel klaimBpjs);
    IKlaimBpjsBuilder UserOfta(IUserOftaKey userOftaKey);
    IKlaimBpjsBuilder OrderKlaimBpjs(IOrderKlaimBpjsKey orderKlaimBpjsKey);
    IKlaimBpjsBuilder PrintReffId(int noUrut, string printReffId);
    IKlaimBpjsBuilder AttachDoc(IDocTypeKey docTypeKey, IDocKey docKey);
    IKlaimBpjsBuilder DetachDoc(IDocKey docKey);
    IKlaimBpjsBuilder AddDocType(IDocTypeKey docTypeKey);
    IKlaimBpjsBuilder RemoveDocType(IDocTypeKey docTypeKey);

    IKlaimBpjsBuilder AddSignee(IDocTypeKey docTypeKey, string email,
        string signTag, SignPositionEnum signPos);
    IKlaimBpjsBuilder RemoveSignee(IDocTypeKey docTypeKey, string email);
    IKlaimBpjsBuilder AddEvent(KlaimBpjsStateEnum docStateEnum, string description);
}

public class KlaimBpjsBuilder : IKlaimBpjsBuilder
{
    private readonly IKlaimBpjsDal _klaimBpjsDal;
    private readonly IKlaimBpjsDocDal _klaimBpjsDocDal;
    private readonly IKlaimBpjsSigneeDal _klaimBpjsSigneeDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly IBlueprintBuilder _blueprintBuilder;
    private readonly IDocBuilder _docBuilder;
    private readonly IDocTypeDal _docTypeDal;
    private readonly IOrderKlaimBpjsBuilder _orderKlaimBpjsBuilder;
    private readonly IKlaimBpjsEventDal _klaimBpjJurnalDal;
    
    private readonly  ITglJamDal _tglJamDal;
    private KlaimBpjsModel _agg = new KlaimBpjsModel();

    public KlaimBpjsBuilder(IKlaimBpjsDal klaimBpjsDal, 
        IKlaimBpjsDocDal klaimBpjsDocDal, 
        IKlaimBpjsSigneeDal klaimBpjsSigneeDal, 
        ITglJamDal tglJamDal, 
        IUserOftaDal userOftaDal, 
        IBlueprintBuilder blueprintBuilder, 
        IDocBuilder docBuilder, 
        IDocTypeDal docTypeDal, 
        IOrderKlaimBpjsBuilder orderKlaimBpjsBuilder, 
        IKlaimBpjsEventDal klaimBpjJurnalDal)
    {
        _klaimBpjsDal = klaimBpjsDal;
        _klaimBpjsDocDal = klaimBpjsDocDal;
        _klaimBpjsSigneeDal = klaimBpjsSigneeDal;
        _tglJamDal = tglJamDal;
        _userOftaDal = userOftaDal;
        _blueprintBuilder = blueprintBuilder;
        _docBuilder = docBuilder;
        _docTypeDal = docTypeDal;
        _orderKlaimBpjsBuilder = orderKlaimBpjsBuilder;
        _klaimBpjJurnalDal = klaimBpjJurnalDal;
    }

    public KlaimBpjsModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public IKlaimBpjsBuilder Create()
    {
        _agg = new KlaimBpjsModel
        {
            KlaimBpjsDate = _tglJamDal.Now,
            ListDoc = new List<KlaimBpjsDocModel>(),
            ListEvent = new List<KlaimBpjsEventModel>()
        };
        return this;
    }

    public IKlaimBpjsBuilder Load(IKlaimBpjsKey klaimBpjsKey)
    {
        _agg = _klaimBpjsDal.GetData(klaimBpjsKey)
            ?? throw new KeyNotFoundException($"KlaimBpjsKey '{klaimBpjsKey.KlaimBpjsId}' not found");
        var listDoc = _klaimBpjsDocDal.ListData(klaimBpjsKey)?.ToList()
            ?? new List<KlaimBpjsDocModel>();
        var listSignee = _klaimBpjsSigneeDal.ListData(klaimBpjsKey)?.ToList()
                      ?? new List<KlaimBpjsSigneeModel>();
        var listJurnal = _klaimBpjJurnalDal.ListData(klaimBpjsKey)?.ToList()
                      ?? new List<KlaimBpjsEventModel>();

        
        // assign listDoc to _agg and listSignee to _agg using LINQ
        _agg.ListDoc = ( 
            from c in listDoc
            join s in listSignee on c.KlaimBpjsDocId equals s.KlaimBpjsDocId into g
            select new KlaimBpjsDocModel
            {
                KlaimBpjsId = c.KlaimBpjsId,
                KlaimBpjsDocId = c.KlaimBpjsDocId,
                NoUrut = c.NoUrut,
                DocTypeId = c.DocTypeId,
                DocTypeName = c.DocTypeName,
                DocId = c.DocId,
                DocUrl = c.DocUrl,
                ListSign = g.ToList()
            }).ToList();
        _agg.ListEvent = listJurnal;
        return this;
    }

    public IKlaimBpjsBuilder Attach(KlaimBpjsModel klaimBpjs)
    {
        _agg = klaimBpjs;
        return this;
    }

    public IKlaimBpjsBuilder UserOfta(IUserOftaKey userOftaKey)
    {
        var userOfta = _userOftaDal.GetData(userOftaKey)
            ?? throw new KeyNotFoundException($"UserOftaKey '{userOftaKey.UserOftaId}' not found");
        _agg.UserOftaId = userOfta.UserOftaId;
        return this;
    }

    public IKlaimBpjsBuilder OrderKlaimBpjs(IOrderKlaimBpjsKey orderKlaimBpjsKey)
    {
        var order = _orderKlaimBpjsBuilder.Load(orderKlaimBpjsKey).Build();
        _agg.OrderKlaimBpjsId = order.OrderKlaimBpjsId;
        _agg.RegId = order.RegId;
        _agg.PasienId = order.PasienId;
        _agg.PasienName = order.PasienName;
        _agg.DokterName = order.DokterName;
        _agg.LayananName = order.LayananName;
        return this;
    }

    public IKlaimBpjsBuilder PrintReffId(int noUrut, string printReffId)
    {
        var klaimDoc = _agg.ListDoc.FirstOrDefault(c => c.NoUrut == noUrut);
        if (klaimDoc is null)
            throw new KeyNotFoundException($"NoUrut '{noUrut}' not found");
        
        klaimDoc.PrintOutReffId = printReffId;
        return this;
    }

    public IKlaimBpjsBuilder AttachDoc(IDocTypeKey docTypeKey, IDocKey docKey)
    {
        var klaimDoc = _agg.ListDoc.FirstOrDefault(c => c.DocTypeId == docTypeKey.DocTypeId)
            ?? throw new KeyNotFoundException($"DocTypeKey '{docTypeKey.DocTypeId}' not found");
        var doc = _docBuilder.Load(docKey).Build();
        klaimDoc.DocId = doc.DocId;
        klaimDoc.DocUrl = doc.PublishedDocUrl;
        return this;
    }

    public IKlaimBpjsBuilder DetachDoc(IDocKey docKey)
    {
        var klaimDoc = _agg.ListDoc.FirstOrDefault(c => c.DocId == docKey.DocId);
        if (klaimDoc is null)
            return this;
        
        klaimDoc.DocId = string.Empty;
        klaimDoc.DocUrl = string.Empty;
        return this;
    }

    public IKlaimBpjsBuilder AddDocType(IDocTypeKey docTypeKey)
    {
        var klaimDoc = _agg.ListDoc.FirstOrDefault(c => c.DocTypeId == docTypeKey.DocTypeId);
        if (klaimDoc is not null)
            throw new ArgumentException($"DocTypeKey '{docTypeKey.DocTypeId}' already exists");
        
        var docType = _docTypeDal.GetData(docTypeKey)
            ?? throw new KeyNotFoundException($"DocTypeKey '{docTypeKey.DocTypeId}' not found");
        var noUrut = _agg.ListDoc
            .DefaultIfEmpty(new KlaimBpjsDocModel{NoUrut = 0})
            .Max(c => c.NoUrut) + 1;

        _agg.ListDoc.Add(new KlaimBpjsDocModel
        {
            KlaimBpjsId = string.Empty,
            KlaimBpjsDocId = string.Empty,
            NoUrut = noUrut,
            DocTypeId = docType.DocTypeId,
            DocTypeName = docType.DocTypeName,
            DocId = string.Empty,
            DocUrl = string.Empty,
            PrintOutReffId = string.Empty,
            ListSign = new List<KlaimBpjsSigneeModel>()
        });
        return this;
    }

    public IKlaimBpjsBuilder RemoveDocType(IDocTypeKey docTypeKey)
    {
        _agg.ListDoc.RemoveAll(x => x.DocTypeId == docTypeKey.DocTypeId);
        return this;
    }

    public IKlaimBpjsBuilder AddSignee(IDocTypeKey docTypeKey, string email, 
        string signTag, SignPositionEnum signPos)
    {
        var klaimDoc = _agg.ListDoc.FirstOrDefault(c => c.DocTypeId == docTypeKey.DocTypeId);
        if (klaimDoc is null)
            throw new KeyNotFoundException($"DocTypeKey '{docTypeKey.DocTypeId}' not found");
        var noUrut = klaimDoc.ListSign
            .DefaultIfEmpty(new KlaimBpjsSigneeModel{NoUrut = 0})
            .Max(c => c.NoUrut) + 1;
        
        klaimDoc.ListSign.Add(new KlaimBpjsSigneeModel
        {
            KlaimBpjsId = _agg.KlaimBpjsId,
            KlaimBpjsDocId = string.Empty,
            NoUrut = noUrut,
            UserOftaId = _agg.UserOftaId,
            Email = email,
            SignTag = signTag,
            SignPosition = signPos
        });
        return this;
    }

    public IKlaimBpjsBuilder RemoveSignee(IDocTypeKey docTypeKey, string email)
    {
        var klaimDoc = _agg.ListDoc.FirstOrDefault(c => c.DocTypeId == docTypeKey.DocTypeId);
        if (klaimDoc is null)
            return this;
        
        klaimDoc.ListSign.RemoveAll(x => x.Email == email);
        return this;
    }

    public IKlaimBpjsBuilder AddEvent(KlaimBpjsStateEnum docStateEnum, string description)
    {
        var noUrut = _agg.ListEvent.DefaultIfEmpty(new KlaimBpjsEventModel{NoUrut = 0})
            .Max(c => c.NoUrut) + 1;
        var desc = docStateEnum.ToString();
        desc += description.Length != 0 ? description : string.Empty;
        _agg.ListEvent.Add(new KlaimBpjsEventModel
        {
            KlaimBpjsId = _agg.KlaimBpjsId,
            NoUrut = noUrut,
            EventDate = _tglJamDal.Now,
            Description = desc
        });
        return this;
    }
}