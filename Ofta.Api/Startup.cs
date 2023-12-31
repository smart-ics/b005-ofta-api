using Microsoft.Extensions.Caching.Memory;
using Ners.Infrastructure.Helpers;
using Ners.Infrastructure.UserContext.Repos;
using Nuna.Lib.AutoNumberHelper;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.UserContext.Repos;
using Usman.Lib.NetStandard;
using Usman.Lib.NetStandard.Interfaces;

namespace Ofta.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //  USMAN
        services.AddScoped<IUsmanUserDal, UsmanUserDal>();
        services.AddScoped<IUsmanUserRoleDal, UsmanUserRoleDal>();
        services.AddScoped<IUsmanPegDal, UsmanPegDal>();
        services.AddScoped<CommandHandler, CommandHandler>();
        
        //  NUNA-LIB
        services.AddScoped<INunaCounterDal, ParamNoDal>();
        services.AddScoped<INunaCounterBL, NunaCounterBL>();
        
        //  SUPPORT
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IMemoryCache, MemoryCache>();
    }
}