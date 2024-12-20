using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Ofta.Infrastructure.UserContext.TilakaAgg.Services;

public class RegisterUserTilakaService: IRegisterUserTilakaService
{
    private readonly TilakaProviderOptions _opt;
    private readonly ITokenTilakaService _tokenService;
    private readonly ITglJamDal _tglJamDal;

    public RegisterUserTilakaService(IOptions<TilakaProviderOptions> opt, ITokenTilakaService tokenService, ITglJamDal tglJamDal)
    {
        _opt = opt.Value;
        _tokenService = tokenService;
        _tglJamDal = tglJamDal;
    }

    public RegisterUserTilakaResponse Execute(RegisterUserTilakaRequest req)
    {
        var result = Task.Run(() => ExecuteRegisterUser(req)).GetAwaiter().GetResult();
        var response = new RegisterUserTilakaResponse(
            result?.Success == true,
            result?.Message ?? string.Empty,
            result?.Data is not null ? result.Data.First() : string.Empty
        );
        return response;
    }

    private async Task<RegisterUserTilakaDto?> ExecuteRegisterUser(RegisterUserTilakaRequest request)
    {
        // BUILD
        var tilakaToken = await _tokenService.Execute(TilakaProviderOptions.SECTION_NAME);
        if (tilakaToken is null)
            throw new ArgumentException($"Get tilaka token {_opt.TokenEndPoint} failed");
        
        var consentTimestamp = _tglJamDal.Now.ToString(DateFormatEnum.YMD_HMS);
        var consentHash = _opt.ClientID + _opt.ConsentText + _opt.Version + consentTimestamp;
        var reqBody = new
        {
            registration_id = request.Model.RegistrationId,
            email = request.Model.Email,
            name = request.Model.NamaKTP,
            company_name = _opt.CompanyName,
            date_expire = request.Model.ExpiredDate.ToString(DateFormatEnum.YMD_HM),
            nik = request.Model.NomorIdentitas,
            photo_ktp = request.Model.FotoKtpBase64,
            is_approved = true,
            consent_text = _opt.ConsentText,
            version = _opt.Version,
            hash_consent = GetConsentHash(consentHash),
            consent_timestamp = consentTimestamp
        };

        var options = new RestClientOptions(_opt.BaseApiUrl)
        {
            Authenticator = new JwtAuthenticator(tilakaToken)
        };
        
        var client = new RestClient(options);
        var req = new RestRequest("/registerForKycCheck")
            .AddBody(reqBody, ContentType.Json);

        // EXECUTE
        var response = await client.ExecutePostAsync(req);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // RETURN
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return new RegisterUserTilakaDto(false, "Forbidden access to Tilaka", null);
        
        var result = JsonSerializer.Deserialize<RegisterUserTilakaDto>(response.Content ?? string.Empty, jsonOptions);
        return result;
    }
    
    private string GetConsentHash(string message)
    {
        var encoding = new UTF8Encoding();
        var keyBytes = encoding.GetBytes(_opt.SecretKey);
        var messageBytes = encoding.GetBytes(message);
        var cryptographer = new HMACSHA256(keyBytes);

        var hash = cryptographer.ComputeHash(messageBytes);

        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
    
    private record RegisterUserTilakaDto(bool Success, string Message, List<string>? Data);
}