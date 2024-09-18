using Microsoft.Extensions.Caching.Memory;
using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Infrastructure;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.ParamContext.ConnectionAgg;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataAccessHelper;
using Ofta.Application.Helpers;
using Ofta.Infrastructure.ParamContext;
using Ofta.Infrastructure.UserContext.UserUsmanAgg.Repos;
using Ofta.Infrastructure.UserContext.UserUsmanAgg.Services;
using Scrutor;
using Usman.Lib.NetStandard;
using Usman.Lib.NetStandard.Interfaces;
using TglJamDal = Ofta.Infrastructure.ParamContext.TglJamDal;

namespace Ofta.Api.Configurations;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services
            .AddTransient<IUsmanGetTokenService, UsmanGetTokenService>()
            .AddScoped<INunaCounterDal, ParamNoDal>()
            .AddScoped<IDbConnectionDal, DbConnectionDal>()
            .AddScoped<ITglJamDal, TglJamDal>()
            .AddScoped<IUsmanUserDal, UsmanUserDal>()
            .AddScoped<IUsmanUserRoleDal, UsmanUserRoleDal>()
            .AddScoped<IUsmanPegDal, UsmanPegDal>()
            .AddScoped<CommandHandler, CommandHandler>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IMemoryCache, MemoryCache>()
            .AddScoped<INunaCounterDal, ParamNoDal>()
            .AddScoped<IAppSettingService, AppSettingService>()
            .AddMemoryCache();

        services
            .Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SECTION_NAME))
            .Configure<TekenAjaProviderOptions>(configuration.GetSection(TekenAjaProviderOptions.SECTION_NAME))
            .Configure<RemoteCetakOptions>(configuration.GetSection(RemoteCetakOptions.SECTION_NAME))
            .Configure<BillingOptions>(configuration.GetSection(BillingOptions.SECTION_NAME))
            .Configure<ICasterOptions>(configuration.GetSection(ICasterOptions.SECTION_NAME))
            .Configure<Emr25Options>(configuration.GetSection(Emr25Options.SECTION_NAME))
            .Configure<SmassOptions>(configuration.GetSection(SmassOptions.SECTION_NAME));

        services
            .Scan(selector => selector
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IInsert<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IUpdate<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IDelete<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IGetData<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IListData<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IListData<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IListData<,,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaService<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IRequestResponseService<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaService<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaService<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaServiceVoid<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            
            );
        return services;
    }

}