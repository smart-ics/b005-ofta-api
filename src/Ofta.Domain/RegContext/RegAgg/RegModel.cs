namespace Ofta.Domain.RegContext.RegAgg;

public class RegModel : IRegKey
{
    public RegModel() {}

    public RegModel(string id) => RegId = id;

    public string RegId { get; set; }
    public string PasienId { get; set; }
    public string PasienName { get; set; }
    public string PesertaJaminanId { get; set; }
    public DateTime RegDate { get; set; }
    public string RegTime { get; set; }
    public DateTime RegOutDate { get; set; }
    public string RegOutTime { get; set; }
    public string LayananName { get; set; }
    public string DokterName { get; set; }
    public string RegJenis { get; set; }
    public string JenisInap { get; set; }
    public string NoSep { get; set; }
}