using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Ofta.Application.UserContext.Contracts;
using Ofta.Infrastructure.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace Ofta.Infrastructure.UserContext.Services;

public class UsmanGetUserService : IUsmanGetUserService
{
    private readonly UsmanOptions _opt;
    private readonly IUsmanGetTokenService _token;
    public UsmanGetUserService(IOptions<UsmanOptions> opt,
        IUsmanGetTokenService token)
    {
        _opt = opt.Value;
        _token = token;
    }
    public UsmanGetUserResponse Execute(UsmanGetUserDto req)
    {
        var user = Task.Run(() => GetDataUser(req)).GetAwaiter().GetResult();
        if (user is null)
            throw new KeyNotFoundException("User not found");

        return user;
    }

    private async Task<UsmanGetUserResponse?> GetDataUser(UsmanGetUserDto req)
    {
        //  BUILD
        var token = await _token.Get("Usman");
        if (token is null)
            throw new ArgumentException($"Get Token {_opt.BaseUrl} failed");
        var requestBody = new GetUserUsmanReqDto
        {
            PegIdEmail = req.Email,
            AppId = _opt.ConsId,
            Pass = req.Pass
        };
        var body = JsonSerializer.Serialize(requestBody);

        var endpoint = $"{_opt.BaseUrl}/api/user/login";
        
        var client = new RestClient(endpoint);
        client.Authenticator = new JwtAuthenticator(token);
        var request = new RestRequest()
            .AddJsonBody(body);

        //  EXECUTE
        var response = await client.ExecutePostAsync<JSend<UsmanGetUserResponse>>(request);

        return response.StatusCode != System.Net.HttpStatusCode.OK ? 
            null :
            JSendResponse.Read(response);
    }
}

[PublicAPI]
public class GetUserUsmanReqDto
{
    public string PegIdEmail { get; set; }
    public string AppId { get; set; }
    public string Pass { get; set; }

}
