namespace Ofta.Domain.DocContext.DocAgg;

public class DocSigneeModel : IDocKey
{
    public string DocId { get; set; }
    public string DocSigneeId { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
    public int Level { get; set; }
    public SignStateEnum SignState { get; set; }
    public DateTime SignedDate { get; set; }
    public string SignPositionDesc { get; set; }
    public string SignUrl { get; set; }
    public bool IsHidden { get; set; }
}

public enum SignStateEnum
{
    NotSigned = 0,
    Signed = 1,
    Rejected = 2
}