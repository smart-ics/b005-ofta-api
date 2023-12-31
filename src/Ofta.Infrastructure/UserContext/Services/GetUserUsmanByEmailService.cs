using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.UserContext.Services;

public class GetUserUsmanByEmailService : IGetUserUsmanByEmailService
{
    private readonly UsmanOptions _opt;
    private readonly IUsmanGetTokenService _token;

    public GetUserUsmanByEmailService(IOptions<UsmanOptions> opt,
        IUsmanGetTokenService token)
    {
        _opt = opt.Value;
        _token = token;
    }

    public GetUserUsmanByEmailResponse Execute(string req)
    {
        var user = Task.Run(() => GetDataUser(req)).GetAwaiter().GetResult();
        if (user is null)
            throw new KeyNotFoundException("User not found");

        return user;
    }

    private async Task<GetUserUsmanByEmailResponse?> GetDataUser(string req)
    {
        //  BUILD
        var token = await _token.Get("Usman");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.BaseUrl} failed");

        var endpoint = $"{_opt.BaseUrl}/api/User/{req}";

        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);
        var request = new RestRequest();

        //  EXECUTE
        var response = await client.ExecuteGetAsync<JSend<GetUserUsmanByEmailResponse>>(request);

        return response.StatusCode != System.Net.HttpStatusCode.OK ?
            null :
            JSendResponse.Read(response);
    }
}
