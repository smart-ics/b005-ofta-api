using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;

public interface IOrderKlaimBpjsBuilder : INunaBuilder<OrderKlaimBpjsModel>
{
    IOrderKlaimBpjsBuilder Create();
    IOrderKlaimBpjsBuilder Load(IOrderKlaimBpjsKey orderKlaimBpjsKey);
    IOrderKlaimBpjsBuilder Attach(OrderKlaimBpjsModel orderKlaimBpjs);
    IOrderKlaimBpjsBuilder Reg(IRegPasien regPasien);
    IOrderKlaimBpjsBuilder Layanan(string  layananName);
    IOrderKlaimBpjsBuilder Dokter(string dokterName);
    IOrderKlaimBpjsBuilder RajalRanap(RajalRanapEnum rajalRanap);
    IOrderKlaimBpjsBuilder User(IUserOftaKey userOftaKey);

}
public class OrderKlaimBpjsBuilder : IOrderKlaimBpjsBuilder
{
    private readonly IOrderKlaimBpjsDal _orderKlaimBpjsDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly ITglJamDal _tglJamDal;
    
    private OrderKlaimBpjsModel _agg = new();

    public OrderKlaimBpjsBuilder(IOrderKlaimBpjsDal orderKlaimBpjsDal, 
        ITglJamDal tglJamDal, 
        IUserOftaDal userOftaDal)
    {
        _orderKlaimBpjsDal = orderKlaimBpjsDal;
        _tglJamDal = tglJamDal;
        _userOftaDal = userOftaDal;
    }

    public OrderKlaimBpjsModel Build()
    {
        _agg.RemoveNull();
        return _agg;
    }

    public IOrderKlaimBpjsBuilder Create()
    {
        _agg = new OrderKlaimBpjsModel
        {
            OrderKlaimBpjsDate = _tglJamDal.Now,
        };
        return this;
    }

    public IOrderKlaimBpjsBuilder Load(IOrderKlaimBpjsKey orderKlaimBpjsKey)
    {
        _agg = _orderKlaimBpjsDal.GetData(orderKlaimBpjsKey)
            ?? throw new KeyNotFoundException($"OrderKlaimBpjsKey {orderKlaimBpjsKey} not found");
        return this;
    }

    public IOrderKlaimBpjsBuilder Attach(OrderKlaimBpjsModel orderKlaimBpjs)
    {
        _agg = orderKlaimBpjs;
        return this;
    }

    public IOrderKlaimBpjsBuilder Reg(IRegPasien regPasien)
    {
        _agg.RegId = regPasien.RegId;
        _agg.PasienId = regPasien.PasienId;
        _agg.PasienName = regPasien.PasienName;
        _agg.NoSep = regPasien.NoSep;
        return this;
    }

    public IOrderKlaimBpjsBuilder Layanan(string layananName)
    {
        _agg.LayananName = layananName;
        return this;
    }

    public IOrderKlaimBpjsBuilder Dokter(string dokterName)
    {
        _agg.DokterName = dokterName;
        return this;
    }

    public IOrderKlaimBpjsBuilder RajalRanap(RajalRanapEnum rajalRanap)
    {
        _agg.RajalRanap = rajalRanap;
        return this;
    }

    public IOrderKlaimBpjsBuilder User(IUserOftaKey userOftaKey)
    {
        var user = _userOftaDal.GetData(userOftaKey)
            ?? throw new KeyNotFoundException($"UserOftaKey {userOftaKey} not found");
        _agg.UserOftaId = user.UserOftaId;
        return this;
    }
}