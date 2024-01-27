namespace Ofta.Domain.DocContext.DocAgg;

public class DocSigneeModel : IDocKey
{
    public string DocId { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
    public int Level { get; set; }  
    public bool IsSigned { get; set; }
    public DateTime SignedDate { get; set; }
}