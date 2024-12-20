using JetBrains.Annotations;
using Mapster;
using Microsoft.Extensions.Options;
using Nuna.Lib.ValidationHelper;
using Ofta.Infrastructure.Helpers;
using RestSharp.Authenticators;
using RestSharp;
using Ofta.Application.RegContext.RegAgg.Contracts;
using Ofta.Domain.RegContext.RegAgg;

namespace Ofta.Infrastructure.ExternalContext.Services;

public class GetRegService : IGetRegService
{
    private readonly BillingOptions _opt;
    private readonly ITokenService _token;

    public GetRegService(IOptions<BillingOptions> opt, ITokenService token)
    {
        _opt = opt.Value;
        _token = token;
    }

    public RegModel? Execute(IRegKey regKey)
    {
        var reg = Task.Run(() => GetDataAsync(regKey)).GetAwaiter().GetResult();
        if (reg is null) return null;

        var result = reg.Adapt<RegModel>();
        result.RegDate = $"{reg.RegDate} {reg.RegTime}".ToDate("yyyy-MM-dd HH:mm:ss");
        result.RegOutDate = $"{reg.RegOutDate} {reg.RegOutTime}".ToDate("yyyy-MM-dd HH:mm:ss");
        return result;
    }

    private async Task<RegDto?> GetDataAsync(IRegKey key)
    {
        if (key.RegId.Trim().Length == 0)
            return null;

        //  BUILD
        var token = await _token.Execute("Billing");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.BaseApiUrl} failed");

        var options = new RestClientOptions(_opt.BaseApiUrl)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        
        var client = new RestClient(options);
        var req = new RestRequest("/api/Reg/{regId}")
            .AddUrlSegment("regId", key.RegId);

        //  EXECUTE
        var response = await client.ExecuteGetAsync<JSend<RegDto>>(req);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
            return null;
        //  RETURN
        return JSendResponse.Read(response);
    }

    [PublicAPI]
    private class RegDto
    {
        public string RegId { get; set; }

        public string RegDate { get; set; }
        public string RegTime { get; set; }
        public string RegOutDate { get; set; }
        public string RegOutTime { get; set; }

        public string PasienId { get; set; }
        public string PasienName { get; set; }
        public string LayananId { get; set; }
        public string LayananName { get; set; }
        public string DokterId { get; set; }
        public string DokterName { get; set; }
        public string JenisInap { get; set; }
        public string RegJenis { get; set; }
        public string TipeJaminanId { get; set; }
        public string TipeJaminanName { get; set; }
        public string PesertaJaminanId { get; set; }
        public string NoSep { get; set; }
        public string BookingId { get; set; }
    }
}
