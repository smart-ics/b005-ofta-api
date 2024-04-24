namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

public enum PrintStateEnum
{
    Listed,
    Queued,    //    dokumen sudah di-order cetak
    Printed,    //    dokumen sudah di-print
    Failed
}