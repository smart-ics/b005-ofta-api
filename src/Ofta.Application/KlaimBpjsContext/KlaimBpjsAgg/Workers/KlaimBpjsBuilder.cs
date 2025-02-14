﻿using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.DocContext.DocAgg.Workers;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.BlueprintAgg.Workers;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.UseCases;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;
using System.Reflection.Metadata.Ecma335;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IKlaimBpjsBuilder : INunaBuilder<KlaimBpjsModel>
{
    IKlaimBpjsBuilder Create();
    IKlaimBpjsBuilder UpdateState(KlaimBpjsStateEnum klaimBpjsStateEnum);
    IKlaimBpjsBuilder UpdateStateCompleted();
    IKlaimBpjsBuilder Load(IKlaimBpjsKey klaimBpjsKey);
    IKlaimBpjsBuilder Attach(KlaimBpjsModel klaimBpjs);
    IKlaimBpjsBuilder UserOfta(IUserOftaKey userOftaKey);
    IKlaimBpjsBuilder OrderKlaimBpjs(IOrderKlaimBpjsKey orderKlaimBpjsKey);
    
    IKlaimBpjsBuilder AddDocType(IDocTypeKey docTypeKey, bool toBePrinted);
    IKlaimBpjsBuilder RemoveDocType(int noUrut);
    IKlaimBpjsBuilder SetDocTypeDrafter(IDocTypeKey docTypeKey, IUserOftaKey userOftaKey);
    
    IKlaimBpjsBuilder AddPrintOut(IDocTypeKey docTypeKey, string printReffId);
    IKlaimBpjsBuilder RemovePrintOut(string printOutReffId);
    IKlaimBpjsBuilder FinishPrintOut(string printOutReffId);
    IKlaimBpjsBuilder AddSignedDocToPrintOut(string printOutReffId, IDocKey doc);

    IKlaimBpjsBuilder PrintDoc(IDocTypeKey docTypeKey, string reffId);
    
    IKlaimBpjsBuilder AddSignee(IDocKey docKey, string email,
        string signTag, SignPositionEnum signPos);
    IKlaimBpjsBuilder RemoveSignee(IDocKey docKey, string email);
    IKlaimBpjsBuilder AddEvent(KlaimBpjsStateEnum docStateEnum, string description);
}

public class KlaimBpjsBuilder : IKlaimBpjsBuilder
{
    private readonly IKlaimBpjsDal _klaimBpjsDal;
    private readonly IKlaimBpjsDocTypeDal _klaimBpjsDocTypeDal;
    private readonly IKlaimBpjsPrintOutDal _klaimBpjsPrintOutDal;
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
        IKlaimBpjsDocTypeDal klaimBpjsDocTypeDal, 
        IKlaimBpjsPrintOutDal klaimBpjsPrintOutDal, 
        IKlaimBpjsSigneeDal klaimBpjsSigneeDal, 
        ITglJamDal tglJamDal, 
        IUserOftaDal userOftaDal, 
        IBlueprintBuilder blueprintBuilder, 
        IDocBuilder docBuilder, 
        IDocTypeDal docTypeDal, 
        IOrderKlaimBpjsBuilder orderKlaimBpjsBuilder, IKlaimBpjsEventDal klaimBpjJurnalDal)
    {
        _klaimBpjsDal = klaimBpjsDal;
        _klaimBpjsDocTypeDal = klaimBpjsDocTypeDal;
        _klaimBpjsPrintOutDal = klaimBpjsPrintOutDal;
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
            ListDocType = new List<KlaimBpjsDocTypeModel>(),
            ListEvent = new List<KlaimBpjsEventModel>()
        };
        return this;
    }

    public IKlaimBpjsBuilder UpdateState(KlaimBpjsStateEnum klaimBpjsStateEnum)
    {        
        _agg.KlaimBpjsState = klaimBpjsStateEnum;
        return this;

    }

    public IKlaimBpjsBuilder UpdateStateCompleted()
    {

        if (_agg.KlaimBpjsState == KlaimBpjsStateEnum.Merged)
            return this;

        var thisPrintOutListed =
            _agg.ListDocType
                .SelectMany(x => x.ListPrintOut)
                .FirstOrDefault(y => y.PrintState == PrintStateEnum.Listed);

        var thisPrintOutFailed =
             _agg.ListDocType
                 .SelectMany(x => x.ListPrintOut)
                 .FirstOrDefault(y => y.PrintState == PrintStateEnum.Failed);

        if (thisPrintOutListed == null && thisPrintOutFailed == null)
        { 
            _agg.KlaimBpjsState = KlaimBpjsStateEnum.Completed;
        }
        else
        {
            _agg.KlaimBpjsState = KlaimBpjsStateEnum.InProgress;
        }

        return this;

    }

    public IKlaimBpjsBuilder Load(IKlaimBpjsKey klaimBpjsKey)
    {
        _agg = _klaimBpjsDal.GetData(klaimBpjsKey)
            ?? throw new KeyNotFoundException($"KlaimBpjsKey '{klaimBpjsKey.KlaimBpjsId}' not found");
        var listDocType = _klaimBpjsDocTypeDal.ListData(klaimBpjsKey)?.ToList()
                    ?? new List<KlaimBpjsDocTypeModel>();
        var listPrintOut = _klaimBpjsPrintOutDal.ListData(klaimBpjsKey)?.ToList()
                           ?? new List<KlaimBpjsPrintOutModel>();
        var listSignee = _klaimBpjsSigneeDal.ListData(klaimBpjsKey)?.ToList()
                      ?? new List<KlaimBpjsSigneeModel>();
        var listJurnal = _klaimBpjJurnalDal.ListData(klaimBpjsKey)?.ToList()
                      ?? new List<KlaimBpjsEventModel>();

        
        // assign listDoc to _agg and listSignee to _agg using LINQ
        _agg.ListDocType = ( 
            from c in listDocType
            join s in listSignee on c.KlaimBpjsDocTypeId equals s.KlaimBpjsDocTypeId into g
            select new KlaimBpjsDocTypeModel
            {
                KlaimBpjsId = c.KlaimBpjsId,
                KlaimBpjsDocTypeId = c.KlaimBpjsDocTypeId,
                NoUrut = c.NoUrut,
                DocTypeId = c.DocTypeId,
                DocTypeName = c.DocTypeName,
                DrafterUserId = c.DrafterUserId,
                ToBePrinted = c.ToBePrinted,
                ListPrintOut = listPrintOut
                    .Where(x => x.KlaimBpjsDocTypeId == c.KlaimBpjsDocTypeId)
                    .Select(y => new KlaimBpjsPrintOutModel
                    {
                        NoUrut = y.NoUrut,
                        PrintOutReffId = y.PrintOutReffId,
                        PrintState = y.PrintState,
                        DocId = y.DocId,
                        DocUrl = y.DocUrl,
                        ListSign = listSignee.Where(x => x.KlaimBpjsPrintOutId == y.KlaimBpjsPrintOutId).ToList()
                    })
                    .ToList()
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
        _agg.NoSep = order.NoSep;
        _agg.DokterName = order.DokterName;
        _agg.LayananName = order.LayananName;
        return this;
    }

    public IKlaimBpjsBuilder AddDocType(IDocTypeKey docTypeKey, bool toBePrinted)
    {
        var klaimDoc = _agg.ListDocType.FirstOrDefault(c => c.DocTypeId == docTypeKey.DocTypeId);
        if (klaimDoc is not null)
            throw new ArgumentException($"DocTypeKey '{docTypeKey.DocTypeId}' already exists");
        
        var docType = _docTypeDal.GetData(docTypeKey)
            ?? throw new KeyNotFoundException($"DocTypeKey '{docTypeKey.DocTypeId}' not found");
        var noUrut = _agg.ListDocType
            .DefaultIfEmpty(new KlaimBpjsDocTypeModel{NoUrut = 0})
            .Max(c => c.NoUrut) + 1;

        _agg.ListDocType.Add(new KlaimBpjsDocTypeModel
        {
            KlaimBpjsId = string.Empty,
            KlaimBpjsDocTypeId = string.Empty,
            NoUrut = noUrut,
            DocTypeId = docType.DocTypeId,
            DocTypeName = docType.DocTypeName,
            DrafterUserId = docType.DefaultDrafterUserId,
            ToBePrinted = toBePrinted,
            ListPrintOut = new List<KlaimBpjsPrintOutModel>()
        });
        return this;
    }

    public IKlaimBpjsBuilder RemoveDocType(int noUrut)
    {
        _agg.ListDocType.RemoveAll(x => x.NoUrut == noUrut);
        var i = 1;
        _agg.ListDocType.OrderBy(y => y.NoUrut)
            .ForEach(x =>
            {
                x.NoUrut = i;
                i++;
            });
        return this;
    }

    public IKlaimBpjsBuilder SetDocTypeDrafter(IDocTypeKey docTypeKey, IUserOftaKey userOftaKey)
    {
        _agg.ListDocType
            .Where(x => x.DocTypeId == docTypeKey.DocTypeId)
            .ForEach(x => x.DrafterUserId = userOftaKey.UserOftaId);
        
        return this;
    }

    public IKlaimBpjsBuilder AddPrintOut(IDocTypeKey docTypeKey, string printOutReffId)
    {
        var errMsg1 = $"Document Type not found in Klaim BPJS: '{docTypeKey.DocTypeId}'";
        var thisDocType = _agg.ListDocType.FirstOrDefault(x => x.DocTypeId == docTypeKey.DocTypeId)
            ?? throw new KeyNotFoundException(errMsg1);

        var errMsg2 = $"PrintOut already exist: '{printOutReffId}'";
        if (thisDocType.ListPrintOut.Any(x => x.PrintOutReffId == printOutReffId))
            return this;

        var noUrut = thisDocType.ListPrintOut
            .DefaultIfEmpty(new KlaimBpjsPrintOutModel { NoUrut = 0 })
            .Max(x => x.NoUrut);
        noUrut++;

        var newPrintOut = new KlaimBpjsPrintOutModel
        {
            NoUrut = noUrut,
            PrintOutReffId = printOutReffId,
            ListSign = new List<KlaimBpjsSigneeModel>()
        };
        newPrintOut.RemoveNull();
        thisDocType.ListPrintOut.Add(newPrintOut);
        
        return this;
    }

    public IKlaimBpjsBuilder RemovePrintOut(string printOutReffId)
    {
        foreach (var docType in _agg.ListDocType)
            docType.ListPrintOut.RemoveAll(x => x.PrintOutReffId == printOutReffId);
        return this;
    }

    public IKlaimBpjsBuilder FinishPrintOut(string printOutReffId)
    {
        var thisPrintOut = 
            _agg.ListDocType
                .SelectMany(x => x.ListPrintOut)
                .FirstOrDefault(y => y.PrintOutReffId == printOutReffId)
            ?? throw new KeyNotFoundException($"Reff ID not found: '{printOutReffId}'");
        
        thisPrintOut.PrintState = PrintStateEnum.Printed;
        return this;
    }

    public IKlaimBpjsBuilder AddSignedDocToPrintOut(string printOutReffId, IDocKey doc)
    {
        var thisPrintOut = 
            _agg.ListDocType
                .SelectMany(x => x.ListPrintOut)
                .FirstOrDefault(y => y.PrintOutReffId == printOutReffId)
            ?? throw new KeyNotFoundException($"Reff ID not found: '{printOutReffId}'");

        var signedDoc = _docBuilder
            .Load(doc)
            .Build();
        
        thisPrintOut.DocId = signedDoc.DocId;
        thisPrintOut.DocUrl = signedDoc.RequestedDocUrl;
        
        return this;
    }

    public IKlaimBpjsBuilder PrintDoc(IDocTypeKey docTypeKey, string reffId)
    {
        
        var thisDocType = _agg.ListDocType.FirstOrDefault(x => x.DocTypeId == docTypeKey.DocTypeId);
        if (thisDocType is null)
            throw new ArgumentException("Document Type not found");

        var thisPrintOut = thisDocType.ListPrintOut.FirstOrDefault(x => x.PrintOutReffId == reffId);
        if (thisPrintOut is null)
            throw new ArgumentException("PrintOut ReffID not found");

        thisPrintOut.PrintState = PrintStateEnum.Queued;
        _ = AddEvent(KlaimBpjsStateEnum.InProgress, $"Print {thisDocType.DocTypeName}: {thisPrintOut.PrintOutReffId}");
        
        return this;
    }

    public IKlaimBpjsBuilder AddSignee(IDocKey docKey, string email, string signTag, SignPositionEnum signPos)
    {
        var doc = _agg.ListDocType.SelectMany(x => x.ListPrintOut)
            .FirstOrDefault(x => x.DocId == docKey.DocId);
        if (doc is null)
            throw new KeyNotFoundException($"DocID not found: '{docKey.DocId}");

        doc.ListSign.RemoveAll(x => x.Email == email);
        doc.ListSign.Add(new KlaimBpjsSigneeModel
        {
            Email = email,
            SignTag = signTag,
            SignPosition = signPos
        });
        return this;
    }

    public IKlaimBpjsBuilder RemoveSignee(IDocKey docKey, string email)
    {
        var doc = _agg.ListDocType.SelectMany(x => x.ListPrintOut)
            .FirstOrDefault(x => x.DocId == docKey.DocId);
        if (doc is null)
            throw new KeyNotFoundException($"DocID not found: '{docKey.DocId}");

        doc.ListSign.RemoveAll(x => x.Email == email);

        return this;
    }

    public IKlaimBpjsBuilder AddEvent(KlaimBpjsStateEnum docStateEnum, string description)
    {
        var noUrut = _agg.ListEvent.DefaultIfEmpty(new KlaimBpjsEventModel{NoUrut = 0})
            .Max(c => c.NoUrut) + 1;
        var desc = docStateEnum.ToString();
        desc += description.Length != 0 ? $" {description}" : string.Empty;
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