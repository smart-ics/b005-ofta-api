using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;

public interface IWorkListBpjsBuilder : INunaBuilder<WorkListBpjsModel>
{
    IWorkListBpjsBuilder Create();
    IWorkListBpjsBuilder Delete();
    IWorkListBpjsBuilder Load(IOrderKlaimBpjsKey orderKlaimBpjsKey);
    IWorkListBpjsBuilder Reg(IRegPasien regPasien);
    IWorkListBpjsBuilder Layanan(string layananName);
    IWorkListBpjsBuilder Dokter(string dokterName);
    IWorkListBpjsBuilder RajalRanap(RajalRanapEnum rajalRanap);
    IWorkListBpjsBuilder KlaimBpjs(IKlaimBpjsKey klaimBpjsKey);
    IWorkListBpjsBuilder WorkState(KlaimBpjsStateEnum workState);

}
public class WorkListBpjsBuilder : IWorkListBpjsBuilder
{
    private readonly IWorkListBpjsDal _workListBpjsDal;
    private readonly ITglJamDal _tglJamDal;

    private WorkListBpjsModel _agg = new();

    public WorkListBpjsBuilder(IWorkListBpjsDal workListBpjsDal,
        ITglJamDal tglJamDal)
    {
        _workListBpjsDal = workListBpjsDal;
        _tglJamDal = tglJamDal;
    }

    public WorkListBpjsModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public IWorkListBpjsBuilder Create()
    {
        _agg = new WorkListBpjsModel
        {
            OrderKlaimBpjsDate = _tglJamDal.Now
        };
        return this;
    }

    public IWorkListBpjsBuilder Delete()
    {
        _agg = new WorkListBpjsModel
        {
            OrderKlaimBpjsDate = _tglJamDal.Now
        };
        return this;
    }

    public IWorkListBpjsBuilder Load(IOrderKlaimBpjsKey orderKlaimBpjsKey)
    {
        _agg = _workListBpjsDal.GetData(orderKlaimBpjsKey)
            ?? throw new KeyNotFoundException($"OrderKlaimBpjsKey {orderKlaimBpjsKey} not found");
        return this;
    }

    public IWorkListBpjsBuilder Reg(IRegPasien regPasien)
    {
        _agg.RegId = regPasien.RegId;
        _agg.PasienId = regPasien.PasienId;
        _agg.PasienName = regPasien.PasienName;
        _agg.NoSep = regPasien.NoSep;
        return this;
    }

    public IWorkListBpjsBuilder Layanan(string layananName)
    {
        _agg.LayananName = layananName;
        return this;
    }

    public IWorkListBpjsBuilder Dokter(string dokterName)
    {
        _agg.DokterName = dokterName;
        return this;
    }

    public IWorkListBpjsBuilder RajalRanap(RajalRanapEnum rajalRanap)
    {
        _agg.RajalRanap = rajalRanap;
        return this;
    }

    public IWorkListBpjsBuilder KlaimBpjs(IKlaimBpjsKey klaimBpjsKey)
    {
        _agg.KlaimBpjsId = klaimBpjsKey.KlaimBpjsId;
        return this;
    }

    public IWorkListBpjsBuilder WorkState(KlaimBpjsStateEnum workState)
    {
        _agg.WorkState = workState;
        return this;
    }
}