using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public class KlaimBpjsDocModel : IKlaimBpjsKey, IDocTypeKey
{
    public string KlaimBpjsId { get; set; }
    public string KlaimBpjsDocId { get; set; }
    public int NoUrut { get; set; }

    public string DocTypeId { get; set; }
    public string DocTypeName { get; set; }
    
    public string DocId { get; set; }
    public string DocUrl { get; set; }
    
    public string PrintOutReffId { get; set; }
    public PrintStateEnum PrintState { get; set; }
    
    public List<KlaimBpjsSigneeModel> ListSign { get; set; }
}

public enum PrintStateEnum
{
    Listed,
    Queued,    //    dokumen sudah di-order cetak
    Printed,    //    dokumen sudah di-print
    Failed
}