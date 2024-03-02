namespace Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

public interface IRegPasien
{
    string RegId { get; }
    string PasienId { get; }
    string PasienName { get; }
    string NoSep { get; }
}