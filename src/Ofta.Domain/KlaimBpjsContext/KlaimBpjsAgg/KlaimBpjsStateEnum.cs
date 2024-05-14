
namespace Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;


public enum KlaimBpjsStateEnum
{
    Created,    //    pertama kali dibuat
    InProgress, //    add-remove or sign or print
    Completed,  //    siap di-merge
    Merged,     //    sudah di-merge 
    Downloaded //    dokumen sudah di-merge  
}