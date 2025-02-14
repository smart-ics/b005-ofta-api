﻿using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsModel : IKlaimBpjsKey, IOrderKlaimBpjsKey, IUserOftaKey
{
    public KlaimBpjsModel()
    {
    }

    public KlaimBpjsModel(string id) => KlaimBpjsId = id;

    public string KlaimBpjsId { get; set; }
    public DateTime KlaimBpjsDate { get; set; }
    public string OrderKlaimBpjsId { get; set; }
    public string UserOftaId { get; set; }
    public KlaimBpjsStateEnum KlaimBpjsState { get; set; }
    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string NoSep { get; set; }
    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public RajalRanapEnum RajalRanap { get; set; }
    public string MergerDocId { get; set; }
    public string MergerDocUrl { get; set; }
    public List<KlaimBpjsDocTypeModel> ListDocType { get; set; }
    public List<KlaimBpjsEventModel> ListEvent { get; set; }
    
}