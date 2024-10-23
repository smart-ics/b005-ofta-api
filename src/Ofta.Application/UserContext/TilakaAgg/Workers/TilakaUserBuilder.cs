using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.TilakaAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Workers;

public interface ITilakaUserBuilder : INunaBuilder<TilakaUserModel>
{
    ITilakaUserBuilder Attach(TilakaUserModel model);
    ITilakaUserBuilder Create();
    ITilakaUserBuilder Load(ITilakaRegistrationKey key);
    ITilakaUserBuilder Load(string email);
    ITilakaUserBuilder RegistrationId(ITilakaRegistrationKey key);
    ITilakaUserBuilder UserOfta(IUserOftaKey key);
    ITilakaUserBuilder Identitas(string nomorIdentitas, string fotoKtpBase64);
    ITilakaUserBuilder TilakaId(string id);
    ITilakaUserBuilder TilakaName(string name);
    ITilakaUserBuilder UserState(TilakaUserState state);
    ITilakaUserBuilder CertificateState(TilakaCertificateState state);
    ITilakaUserBuilder RevokeReason(string reason);
}

public class TilakaUserBuilder: ITilakaUserBuilder
{
    private TilakaUserModel _aggregate = new();
    private readonly IAppSettingService _appSettingService;
    private readonly ITglJamDal _tglJamDal;
    private readonly IUserOftaDal _userOftaDal;
    private readonly ITilakaUserDal _tilakaUserDal;

    public TilakaUserBuilder(IAppSettingService appSettingService, ITglJamDal tglJamDal, IUserOftaDal userOftaDal, ITilakaUserDal tilakaUserDal)
    {
        _appSettingService = appSettingService;
        _tglJamDal = tglJamDal;
        _userOftaDal = userOftaDal;
        _tilakaUserDal = tilakaUserDal;
    }

    public TilakaUserModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public ITilakaUserBuilder Attach(TilakaUserModel model)
    {
        _aggregate = model;
        return this;
    }

    public ITilakaUserBuilder Create()
    {
        _aggregate = new TilakaUserModel
        {
            ExpiredDate = _tglJamDal.Now.AddDays(7), // khusus sandbox, saat live ganti ke baris code dibawa
            // ExpiredDate = _tglJamDal.Now.AddYears(_appSettingService.UserExpirationTime),
            UserState = TilakaUserState.Created,
            CertificateState = TilakaCertificateState.NoCertificate
        };

        return this;
    }

    public ITilakaUserBuilder Load(ITilakaRegistrationKey key)
    {
        _aggregate = _tilakaUserDal.GetData(key)
            ?? throw new KeyNotFoundException($"Tilaka Registration with id {key.RegistrationId} not found");
        return this;
    }

    public ITilakaUserBuilder Load(string email)
    {
        var userOfta = _userOftaDal.GetData(email)
            ?? throw new KeyNotFoundException($"User Ofta with email: {email} not found");
        
        _aggregate = _tilakaUserDal.GetData(userOfta)
            ?? throw new KeyNotFoundException($"Tilaka Registration with UserOftaId: {userOfta.UserOftaId} not found");

        return this;
    }

    public ITilakaUserBuilder RegistrationId(ITilakaRegistrationKey key)
    {
        _aggregate.RegistrationId = key.RegistrationId;
        return this;
    }

    public ITilakaUserBuilder UserOfta(IUserOftaKey key)
    {
        var userOfta = _userOftaDal.GetData(key)
            ?? throw new KeyNotFoundException($"User Ofta with id {key.UserOftaId} not found");
        
        _aggregate.UserOftaId = userOfta.UserOftaId;
        _aggregate.UserOftaName = userOfta.UserOftaName;
        _aggregate.Email = userOfta.Email;
        
        return this;
    }

    public ITilakaUserBuilder Identitas(string nomorIdentitas, string fotoKtpBase64)
    {
        _aggregate.NomorIdentitas = nomorIdentitas;
        _aggregate.FotoKtpBase64 = fotoKtpBase64;
        return this;
    }

    public ITilakaUserBuilder TilakaId(string id)
    {
        _aggregate.TilakaId = id;
        return this;
    }

    public ITilakaUserBuilder TilakaName(string name)
    {
        _aggregate.TilakaName = name;
        return this;
    }

    public ITilakaUserBuilder UserState(TilakaUserState state)
    {
        _aggregate.UserState = state;
        return this;
    }

    public ITilakaUserBuilder CertificateState(TilakaCertificateState state)
    {
        _aggregate.CertificateState = state;
        return this;
    }

    public ITilakaUserBuilder RevokeReason(string reason)
    {
        _aggregate.RevokeReason = reason;
        return this;
    }
}