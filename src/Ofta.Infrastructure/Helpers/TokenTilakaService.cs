using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Nuna.Lib.CleanArchHelper;
using RestSharp;
using System.Text.Json;

namespace Ofta.Infrastructure.Helpers;

public interface ITokenTilakaService : INunaService<Task<string?>, string>
{
}

public class TokenTilakaService : ITokenTilakaService
{
    private readonly TilakaProviderOptions _opt;
    private readonly IMemoryCache _cache;

    public TokenTilakaService(IOptions<TilakaProviderOptions> opt, IMemoryCache cache)
    {
        _opt = opt.Value;
        _cache = cache;
    }

    public async Task<string?> Execute(string provider)
    {
        var result = _cache.Get<string>($"Token{provider}");
        if (result is not null)
            return result;

        var endpoint = $"{_opt.TokenEndPoint}";
        var client = new RestClient(endpoint);

        
        var req = new RestRequest();
        req.Method = Method.Post;
        req.AddParameter("client_id", _opt.ClientID);         
        req.AddParameter("client_secret", _opt.SecretKey);    
        req.AddParameter("grant_type", "client_credentials"); 

        
        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        var response = await client.ExecutePostAsync<string>(req);

        if (response.Content is null)
            return null;

        var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response.Content);
        if (jsonResponse is not null && jsonResponse.TryGetValue("access_token", out var accessToken))
        {
            var token = accessToken?.ToString();

            var cacheOption = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            _cache.Set($"Token{provider}", token, cacheOption);

            return token;
        }

        return response.Data;
    }
}