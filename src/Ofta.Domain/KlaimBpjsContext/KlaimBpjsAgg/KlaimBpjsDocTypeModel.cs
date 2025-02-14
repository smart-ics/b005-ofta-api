﻿using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDocTypeModel : IKlaimBpjsKey, IDocTypeKey
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsDocTypeId { get; set; }
    public int NoUrut { get; set; }

    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    
    public string DrafterUserId { get; set; }
    public bool ToBePrinted { get; set; }
    
    public List<KlaimBpjsPrintOutModel> ListPrintOut { get; set; }    
}